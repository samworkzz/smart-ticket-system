using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartTicketApi.Data;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Services.Auth;
using SmartTicketApi.Services.Manager;
using SmartTicketApi.Services.TicketComments;
using SmartTicketApi.Services.Tickets;
using SmartTicketApi.Services.Admin;
using SmartTicketApi.Services.Notifications;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Authentication (JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

// Authorization
builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketCommentService, TicketCommentService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddHostedService<SLABackgroundService>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();


app.UseMiddleware<SmartTicketApi.Middleware.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAngularApp");


// Security middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// SEED ROLES 

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { RoleName = "Admin" },
            new Role { RoleName = "SupportManager" },
            new Role { RoleName = "SupportAgent" },
            new Role { RoleName = "EndUser" }
        );

        context.SaveChanges();
    }
}

//SEEDING TICKET STATUS
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.TicketStatuses.Any())
    {
        context.TicketStatuses.AddRange(
            new TicketStatus { StatusName = "Created" },
            new TicketStatus { StatusName = "Assigned" },
            new TicketStatus { StatusName = "In Progress" },
            new TicketStatus { StatusName = "Resolved" },
            new TicketStatus { StatusName = "Closed" }
        );

        context.SaveChanges();
    }
}

//SEEDING TICKET PRIORITIES

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.TicketPriorities.Any())
    {
        context.TicketPriorities.AddRange(
            new TicketPriority { PriorityName = "Low" },
            new TicketPriority { PriorityName = "Medium" },
            new TicketPriority { PriorityName = "High" }
        );

        context.SaveChanges();
    }
}

//SEED TICKET CATEGORIES
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.TicketCategories.Any())
    {
        context.TicketCategories.AddRange(
            new TicketCategory { CategoryName = "Software" },
            new TicketCategory { CategoryName = "Hardware" },
            new TicketCategory { CategoryName = "Network" }
        );

        context.SaveChanges();
    }
}
//SEED SLA DATA
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.SLAs.Any())
    {
        var low = context.TicketPriorities.First(p => p.PriorityName == "Low");
        var medium = context.TicketPriorities.First(p => p.PriorityName == "Medium");
        var high = context.TicketPriorities.First(p => p.PriorityName == "High");

        context.SLAs.AddRange(
            new SLA { TicketPriorityId = low.TicketPriorityId, ResponseHours = 48 },
            new SLA { TicketPriorityId = medium.TicketPriorityId, ResponseHours = 24 },
            new SLA { TicketPriorityId = high.TicketPriorityId, ResponseHours = 4 }
        );

        context.SaveChanges();
    }
}

//SEED USERS
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = new PasswordHasher<User>();

    void SeedUser(string name, string email, string roleName)
    {
        if (context.Users.Any(u => u.Email == email))
            return;

        var role = context.Roles.First(r => r.RoleName == roleName);

        var user = new User
        {
            Name = name,
            Email = email,
            RoleId = role.RoleId
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "Password@123");

        context.Users.Add(user);
        context.SaveChanges();
    }

    SeedUser("System Admin", "admin@system.com", "Admin");
    SeedUser("Support Manager", "manager@system.com", "SupportManager");
    SeedUser("Support Agent", "agent@system.com", "SupportAgent");
}


app.Run();

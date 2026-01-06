
# Chubb Ticket System

Chubb Ticket System is a Smart Ticket and Issue Management System developed as a full-stack enterprise web application using ASP.NET Core Web API and Angular.  
The system provides a centralized platform for managing support tickets with role-based access, SLA tracking, escalation handling, and reporting.

---

## Overview

Many organizations rely on emails and spreadsheets to manage support requests, which leads to poor visibility, delayed resolutions, and lack of accountability.  
Chubb Ticket System addresses these issues by providing a secure, scalable, and API-driven solution for managing tickets across multiple user roles.

---

## System Architecture

- Frontend: Angular (role-based dashboards)
- Backend: ASP.NET Core Web API
- Database: SQL Server
- Authentication: JWT-based authentication with Role-Based Access Control (RBAC)

---

## User Roles

- Admin
- Support Manager
- Support Agent
- End User

Each role has a dedicated dashboard and permissions based on responsibilities.

---

## Key Features

- User registration and login
- JWT-based authentication and authorization
- Role-based dashboards
- Complete ticket lifecycle management
- Ticket assignment and escalation handling
- SLA breach identification
- Ticket activity history and audit logs
- Role-specific reports and summary dashboards
- Modular Angular frontend
- Clean layered backend architecture

---

## Technology Stack

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- LINQ
- SQL Server
- JWT Authentication
- Swagger / OpenAPI

### Frontend
- Angular
- TypeScript
- Angular Material
- HTTPClient
- Route Guards and Interceptors

---

## Local Setup Instructions

### Prerequisites

Ensure the following are installed on your local machine:

- .NET SDK 8 or above
- Node.js (LTS version)
- Angular CLI
- SQL Server
- Visual Studio or Visual Studio Code

---

## Backend Setup (ASP.NET Core Web API)

1. Navigate to the backend project directory:
   ```bash
   cd backend


2. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

3. Update the SQL Server connection string in:

   ```text
   appsettings.json
   ```

4. Apply Entity Framework migrations:

   ```bash
   dotnet ef database update
   ```

5. Run the backend API:

   ```bash
   dotnet run
   ```

6. The API will be available at:

   ```text
   https://localhost:<port>
   ```

7. Swagger documentation can be accessed at:

   ```text
   https://localhost:<port>/swagger
   ```

---

## Frontend Setup (Angular)

1. Navigate to the frontend project directory:

   ```bash
   cd frontend
   ```

2. Install required dependencies:

   ```bash
   npm install
   ```

3. Update the API base URL in:

   ```text
   src/environments/environment.ts
   ```

4. Run the Angular application:

   ```bash
   ng serve
   ```

5. Open the application in the browser:

   ```text
   http://localhost:4200
   ```

---

## Default Login Credentials

Use the following credentials to access the system with predefined roles:

### Admin

* Email: [admin@system.com](mailto:admin@system.com)
* Password: Password@123

### Support Manager

* Email: [manager@system.com](mailto:manager@system.com)
* Password: Password@123

### Support Agent

* Email: [agent@system.com](mailto:agent@system.com)
* Password: Password@123

End users can register using the registration page.

---

## Project Highlights

* Secure JWT-based authentication and role-based authorization
* Role-driven dashboards with controlled access
* SLA breach detection and escalation support
* Clean layered backend architecture (Controllers, Services, Repositories, DTOs)
* Modular Angular frontend architecture
* LINQ-based filtering, aggregation, and reporting
* Deployment-ready project structure

---

## Author

Samridhi Jaiswal
samridhiworkzz@gmail.com
```



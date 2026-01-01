namespace SmartTicketApi.Models.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        //One single Role can act as a parent to a Collection of Users.
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

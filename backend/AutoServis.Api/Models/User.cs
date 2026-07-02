namespace AutoServis.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "user";

        public string? Phone { get; set; }

        public string? City { get; set; }

        public List<Vehicle> Vehicles { get; set; } = new();

        public List<Appointment> Appointments { get; set; } = new();
    }
}
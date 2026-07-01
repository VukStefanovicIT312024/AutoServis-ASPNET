namespace AutoServis.Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public string RegistrationNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public User? User { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}
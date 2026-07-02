namespace AutoServis.Api.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; } = string.Empty;

        public string? ProblemDescription { get; set; }

        public string Status { get; set; } = "zakazano";

        public int UserId { get; set; }

        public User? User { get; set; }

        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }

        public int ServiceId { get; set; }

        public Service? Service { get; set; }
    }
}
namespace AutoServis.Api.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public string Includes { get; set; } = string.Empty;

        public List<Appointment> Appointments { get; set; } = new();
    }
}
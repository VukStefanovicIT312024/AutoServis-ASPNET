using AutoServis.Api.Data;
using AutoServis.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServis.Api.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetServices()
        {
            var services = await _context.Services
                .OrderBy(service => service.Id)
                .Select(service => ToServiceResponse(service))
                .ToListAsync();

            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new { message = "Usluga nije pronađena." });
            }

            return Ok(ToServiceResponse(service));
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateService(ServiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Category) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                request.Price <= 0 ||
                request.Duration <= 0)
            {
                return BadRequest(new { message = "Naziv, kategorija, opis, cena i trajanje su obavezni." });
            }

            var service = new Service
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                Duration = request.Duration,
                Includes = string.Join("|", request.Includes ?? new List<string>())
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, ToServiceResponse(service));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateService(int id, ServiceRequest request)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new { message = "Usluga nije pronađena." });
            }

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Category) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                request.Price <= 0 ||
                request.Duration <= 0)
            {
                return BadRequest(new { message = "Naziv, kategorija, opis, cena i trajanje su obavezni." });
            }

            service.Name = request.Name;
            service.Category = request.Category;
            service.Description = request.Description;
            service.Price = request.Price;
            service.Duration = request.Duration;
            service.Includes = string.Join("|", request.Includes ?? new List<string>());

            await _context.SaveChangesAsync();

            return Ok(ToServiceResponse(service));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new { message = "Usluga nije pronađena." });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usluga je obrisana." });
        }

        private static object ToServiceResponse(Service service)
        {
            return new
            {
                _id = service.Id,
                id = service.Id,
                name = service.Name,
                category = service.Category,
                description = service.Description,
                price = service.Price,
                duration = service.Duration,
                includes = service.Includes
                    .Split("|", StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
            };
        }
    }

    public class ServiceRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public List<string>? Includes { get; set; }
    }
}
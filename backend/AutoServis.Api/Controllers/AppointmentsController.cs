using System.Security.Claims;
using AutoServis.Api.Data;
using AutoServis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServis.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static readonly string[] AllowedStatuses =
        {
            "zakazano",
            "u_obradi",
            "zavrseno",
            "otkazano"
        };

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAppointments()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var appointments = await _context.Appointments
                .Include(appointment => appointment.Service)
                .Include(appointment => appointment.Vehicle)
                .Where(appointment => appointment.UserId == userId.Value)
                .OrderByDescending(appointment => appointment.Date)
                .ThenBy(appointment => appointment.Time)
                .ToListAsync();

            var response = appointments
                .Select(appointment => ToAppointmentResponse(appointment, false))
                .ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateAppointment(AppointmentRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            if (string.IsNullOrWhiteSpace(request.Service) ||
                string.IsNullOrWhiteSpace(request.Vehicle) ||
                string.IsNullOrWhiteSpace(request.Date) ||
                string.IsNullOrWhiteSpace(request.Time))
            {
                return BadRequest(new { message = "Usluga, vozilo, datum i vreme su obavezni." });
            }

            if (!int.TryParse(request.Service, out var serviceId))
            {
                return BadRequest(new { message = "Neispravno odabrana usluga." });
            }

            if (!int.TryParse(request.Vehicle, out var vehicleId))
            {
                return BadRequest(new { message = "Neispravno odabrano vozilo." });
            }

            if (!DateTime.TryParse(request.Date, out var appointmentDate))
            {
                return BadRequest(new { message = "Datum nije ispravan." });
            }

            appointmentDate = appointmentDate.Date;

            if (appointmentDate < DateTime.Today.AddDays(1))
            {
                return BadRequest(new { message = "Termin može biti zakazan najranije za sutrašnji datum." });
            }

            var service = await _context.Services.FindAsync(serviceId);

            if (service == null)
            {
                return NotFound(new { message = "Usluga nije pronađena." });
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(vehicle => vehicle.Id == vehicleId && vehicle.UserId == userId.Value);

            if (vehicle == null)
            {
                return NotFound(new { message = "Vozilo nije pronađeno." });
            }

            var appointmentExists = await _context.Appointments.AnyAsync(appointment =>
                appointment.Date == appointmentDate &&
                appointment.Time == request.Time &&
                appointment.Status != "otkazano");

            if (appointmentExists)
            {
                return BadRequest(new { message = "Izabrani termin je već zauzet." });
            }

            var appointment = new Appointment
            {
                UserId = userId.Value,
                ServiceId = serviceId,
                VehicleId = vehicleId,
                Date = appointmentDate,
                Time = request.Time,
                ProblemDescription = request.Description,
                Status = "zakazano"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var createdAppointment = await _context.Appointments
                .Include(item => item.Service)
                .Include(item => item.Vehicle)
                .Include(item => item.User)
                .FirstAsync(item => item.Id == appointment.Id);

            return Ok(ToAppointmentResponse(createdAppointment, true));
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<object>> CancelAppointment(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(appointment => appointment.Id == id && appointment.UserId == userId.Value);

            if (appointment == null)
            {
                return NotFound(new { message = "Zakazivanje nije pronađeno." });
            }

            if (appointment.Status != "zakazano")
            {
                return BadRequest(new { message = "Samo zakazani termini mogu biti otkazani." });
            }

            appointment.Status = "otkazano";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                _id = appointment.Id,
                id = appointment.Id,
                status = appointment.Status,
                message = "Termin je otkazan."
            });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(appointment => appointment.User)
                .Include(appointment => appointment.Service)
                .Include(appointment => appointment.Vehicle)
                .OrderByDescending(appointment => appointment.Date)
                .ThenBy(appointment => appointment.Time)
                .ToListAsync();

            var response = appointments
                .Select(appointment => ToAppointmentResponse(appointment, true))
                .ToList();

            return Ok(response);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<object>> ChangeAppointmentStatus(int id, ChangeStatusRequest request)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound(new { message = "Zakazivanje nije pronađeno." });
            }

            if (string.IsNullOrWhiteSpace(request.Status) ||
                !AllowedStatuses.Contains(request.Status))
            {
                return BadRequest(new { message = "Status nije ispravan." });
            }

            appointment.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                _id = appointment.Id,
                id = appointment.Id,
                status = appointment.Status,
                message = "Status zakazivanja je promenjen."
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return null;
            }

            return userId;
        }

        private static object ToAppointmentResponse(Appointment appointment, bool includeUser)
        {
            return new
            {
                _id = appointment.Id,
                id = appointment.Id,
                date = appointment.Date.ToString("yyyy-MM-dd"),
                time = appointment.Time,
                description = appointment.ProblemDescription,
                status = appointment.Status,

                user = includeUser && appointment.User != null
                    ? new
                    {
                        _id = appointment.User.Id,
                        id = appointment.User.Id,
                        name = appointment.User.Name,
                        email = appointment.User.Email,
                        role = appointment.User.Role
                    }
                    : null,

                service = appointment.Service != null
                    ? new
                    {
                        _id = appointment.Service.Id,
                        id = appointment.Service.Id,
                        name = appointment.Service.Name,
                        category = appointment.Service.Category,
                        price = appointment.Service.Price,
                        duration = appointment.Service.Duration
                    }
                    : null,

                vehicle = appointment.Vehicle != null
                    ? new
                    {
                        _id = appointment.Vehicle.Id,
                        id = appointment.Vehicle.Id,
                        brand = appointment.Vehicle.Brand,
                        model = appointment.Vehicle.Model,
                        year = appointment.Vehicle.Year,
                        plateNumber = appointment.Vehicle.PlateNumber
                    }
                    : null
            };
        }
    }

    public class AppointmentRequest
    {
        public string Service { get; set; } = string.Empty;

        public string Vehicle { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;

        public string Time { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class ChangeStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
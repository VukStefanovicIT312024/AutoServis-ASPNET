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
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVehicles()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var vehicles = await _context.Vehicles
                .Where(vehicle => vehicle.UserId == userId.Value)
                .OrderBy(vehicle => vehicle.Id)
                .Select(vehicle => ToVehicleResponse(vehicle))
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateVehicle(VehicleRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            if (string.IsNullOrWhiteSpace(request.Brand) ||
                string.IsNullOrWhiteSpace(request.Model) ||
                string.IsNullOrWhiteSpace(request.PlateNumber))
            {
                return BadRequest(new { message = "Marka, model, godina i registracija su obavezni." });
            }

            if (request.Year < 1980 || request.Year > DateTime.Now.Year)
            {
                return BadRequest(new { message = $"Godina proizvodnje mora biti između 1980. i {DateTime.Now.Year}." });
            }

            var plateNumber = request.PlateNumber.Trim().ToUpper();

            var plateExists = await _context.Vehicles
                .AnyAsync(vehicle => vehicle.PlateNumber == plateNumber);

            if (plateExists)
            {
                return BadRequest(new { message = "Vozilo sa ovom registracijom već postoji." });
            }

            var vehicle = new Vehicle
            {
                UserId = userId.Value,
                Brand = request.Brand.Trim(),
                Model = request.Model.Trim(),
                Year = request.Year,
                PlateNumber = plateNumber
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicles), new { id = vehicle.Id }, ToVehicleResponse(vehicle));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateVehicle(int id, VehicleRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(vehicle => vehicle.Id == id && vehicle.UserId == userId.Value);

            if (vehicle == null)
            {
                return NotFound(new { message = "Vozilo nije pronađeno." });
            }

            if (string.IsNullOrWhiteSpace(request.Brand) ||
                string.IsNullOrWhiteSpace(request.Model) ||
                string.IsNullOrWhiteSpace(request.PlateNumber))
            {
                return BadRequest(new { message = "Marka, model, godina i registracija su obavezni." });
            }

            if (request.Year < 1980 || request.Year > DateTime.Now.Year)
            {
                return BadRequest(new { message = $"Godina proizvodnje mora biti između 1980. i {DateTime.Now.Year}." });
            }

            var plateNumber = request.PlateNumber.Trim().ToUpper();

            var plateExists = await _context.Vehicles
                .AnyAsync(otherVehicle => otherVehicle.Id != id && otherVehicle.PlateNumber == plateNumber);

            if (plateExists)
            {
                return BadRequest(new { message = "Vozilo sa ovom registracijom već postoji." });
            }

            vehicle.Brand = request.Brand.Trim();
            vehicle.Model = request.Model.Trim();
            vehicle.Year = request.Year;
            vehicle.PlateNumber = plateNumber;

            await _context.SaveChangesAsync();

            return Ok(ToVehicleResponse(vehicle));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(vehicle => vehicle.Id == id && vehicle.UserId == userId.Value);

            if (vehicle == null)
            {
                return NotFound(new { message = "Vozilo nije pronađeno." });
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vozilo je uspešno obrisano." });
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

        private static object ToVehicleResponse(Vehicle vehicle)
        {
            return new
            {
                _id = vehicle.Id,
                id = vehicle.Id,
                brand = vehicle.Brand,
                model = vehicle.Model,
                year = vehicle.Year,
                plateNumber = vehicle.PlateNumber,
                userId = vehicle.UserId
            };
        }
    }

    public class VehicleRequest
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public string PlateNumber { get; set; } = string.Empty;
    }
}
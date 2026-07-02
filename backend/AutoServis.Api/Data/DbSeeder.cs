using AutoServis.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServis.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            var adminExists = await context.Users.AnyAsync(user => user.Email == "admin@test.com");

            if (!adminExists)
            {
                var admin = new User
                {
                    Name = "Administrator",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "admin",
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
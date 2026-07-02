using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoServis.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehiclePlateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationNumber",
                table: "Vehicles",
                newName: "PlateNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_RegistrationNumber",
                table: "Vehicles",
                newName: "IX_Vehicles_PlateNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlateNumber",
                table: "Vehicles",
                newName: "RegistrationNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                newName: "IX_Vehicles_RegistrationNumber");
        }
    }
}

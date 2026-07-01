using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoServis.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Services",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "Activities",
                table: "Services",
                newName: "Includes");

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Category", "Description", "Duration", "Includes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Održavanje", "Zamena ulja, filtera ulja i osnovna provera vozila.", 60, "Zamena motornog ulja|Zamena filtera ulja|Provera tečnosti|Vizuelni pregled vozila", "Mali servis", 7500m },
                    { 2, "Održavanje", "Detaljan servis vozila koji obuhvata zamenu ključnih potrošnih delova.", 180, "Zamena zupčastog kaiša|Zamena španera|Zamena vodene pumpe|Kontrola sistema hlađenja", "Veliki servis", 28000m },
                    { 3, "Dijagnostika", "Računarska dijagnostika i očitavanje grešaka na vozilu.", 45, "Očitavanje grešaka|Analiza parametara|Brisanje grešaka|Preporuka za popravku", "Dijagnostika vozila", 3500m },
                    { 4, "Popravka", "Pregled i zamena kočionih pločica ili diskova po potrebi.", 90, "Pregled kočionog sistema|Zamena pločica|Kontrola diskova|Test kočenja", "Zamena kočnica", 12000m },
                    { 5, "Registracija", "Provera osnovne ispravnosti vozila pre tehničkog pregleda.", 60, "Provera svetlosne signalizacije|Provera kočnica|Provera pneumatika|Provera osnovne dokumentacije", "Priprema za tehnički pregled", 5000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.RenameColumn(
                name: "Includes",
                table: "Services",
                newName: "Activities");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Services",
                newName: "DurationMinutes");
        }
    }
}

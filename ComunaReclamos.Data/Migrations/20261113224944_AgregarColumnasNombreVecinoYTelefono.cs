using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComunaReclamos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColumnasNombreVecinoYTelefono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreVecino",
                table: "Reclamos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonoContacto",
                table: "Reclamos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreVecino",
                table: "Reclamos");

            migrationBuilder.DropColumn(
                name: "TelefonoContacto",
                table: "Reclamos");
        }
    }
}

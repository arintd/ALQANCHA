using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALQANCHA.Migrations
{
    /// <inheritdoc />
    public partial class Modelo_Jugador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaBloqueado",
                table: "Jugadores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaBloqueado",
                table: "Jugadores");
        }
    }
}

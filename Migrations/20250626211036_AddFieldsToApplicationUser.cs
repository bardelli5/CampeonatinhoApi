using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampeonatinhoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FavoriteTeam",
                table: "AspNetUsers",
                newName: "Password");

            migrationBuilder.AddColumn<int>(
                name: "FavoriteTeamId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteTeamId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "AspNetUsers",
                newName: "FavoriteTeam");
        }
    }
}

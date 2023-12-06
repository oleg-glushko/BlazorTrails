using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingTrails.Api.Persistence.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Trails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Trails");
        }
    }
}

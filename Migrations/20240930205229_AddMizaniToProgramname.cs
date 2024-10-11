using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyUni.Migrations
{
    public partial class AddMizaniToProgramname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
migrationBuilder.AddColumn<string>(
    name: "Mizani",
    table: "Programname",
    type: "nvarchar(max)",
    nullable: false,
    defaultValue: "" // Default value for existing rows
);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mizani",
                table: "Programname");
        }
    }
}


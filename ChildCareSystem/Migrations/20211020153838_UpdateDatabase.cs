using Microsoft.EntityFrameworkCore.Migrations;

namespace ChildCareSystem.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeAvailableId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TimeAvailable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAvailable", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TimeAvailableId",
                table: "Reservations",
                column: "TimeAvailableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_TimeAvailable_TimeAvailableId",
                table: "Reservations",
                column: "TimeAvailableId",
                principalTable: "TimeAvailable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_TimeAvailable_TimeAvailableId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "TimeAvailable");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TimeAvailableId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TimeAvailableId",
                table: "Reservations");
        }
    }
}

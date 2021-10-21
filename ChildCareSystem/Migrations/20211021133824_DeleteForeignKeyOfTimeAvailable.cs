using Microsoft.EntityFrameworkCore.Migrations;

namespace ChildCareSystem.Migrations
{
    public partial class DeleteForeignKeyOfTimeAvailable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_TimeAvailable_TimeAvailableId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TimeAvailableId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TimeAvailableId",
                table: "Reservations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeAvailableId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}

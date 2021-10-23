using Microsoft.EntityFrameworkCore.Migrations;

namespace ChildCareSystem.Migrations
{
    public partial class RemoveUniqueConstraintForServiceName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Service_ServiceName",
                table: "Service");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Service",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Service",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceName",
                table: "Service",
                column: "ServiceName",
                unique: true,
                filter: "[ServiceName] IS NOT NULL");
        }
    }
}

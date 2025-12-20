using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class UserMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymCenters_GymCenterId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "GymCenterId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymCenters_GymCenterId",
                table: "AspNetUsers",
                column: "GymCenterId",
                principalTable: "GymCenters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymCenters_GymCenterId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "GymCenterId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymCenters_GymCenterId",
                table: "AspNetUsers",
                column: "GymCenterId",
                principalTable: "GymCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

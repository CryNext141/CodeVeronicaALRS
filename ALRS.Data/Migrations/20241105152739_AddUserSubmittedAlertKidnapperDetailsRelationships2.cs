using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALRS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSubmittedAlertKidnapperDetailsRelationships2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VictimName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VictimAge = table.Column<int>(type: "int", nullable: false),
                    CrimeLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrimeDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrimeStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            

            migrationBuilder.CreateTable(
                name: "KidnapperDetailsAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KidnapperName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperAge = table.Column<int>(type: "int", nullable: false),
                    KidnapperSex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperLook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperVehicle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlertsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KidnapperDetailsAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KidnapperDetailsAlerts_Alerts_AlertsId",
                        column: x => x.AlertsId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubmittedAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrimeLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CrimeDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VictimLook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlertsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubmittedAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubmittedAlerts_Alerts_AlertsId",
                        column: x => x.AlertsId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

           

            migrationBuilder.CreateTable(
                name: "KidnapperDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KidnapperName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperAge = table.Column<int>(type: "int", nullable: false),
                    KidnapperSex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperLook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KidnapperVehicle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserSubmittedAlertId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KidnapperDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KidnapperDetails_UserSubmittedAlerts_UserSubmittedAlertId",
                        column: x => x.UserSubmittedAlertId,
                        principalTable: "UserSubmittedAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.DropTable(
                name: "KidnapperDetails");

            migrationBuilder.DropTable(
                name: "KidnapperDetailsAlerts");

          
            migrationBuilder.DropTable(
                name: "UserSubmittedAlerts");

            migrationBuilder.DropTable(
                name: "Alerts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ALRS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLookUpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VictimSex",
                table: "Victim");

            migrationBuilder.DropColumn(
                name: "VictimSkinColor",
                table: "Victim");

            migrationBuilder.DropColumn(
                name: "AbductorSex",
                table: "Abductor");

            migrationBuilder.DropColumn(
                name: "AbductorSkinColor",
                table: "Abductor");

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "Victim",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "SkinColorId",
                table: "Victim",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "Abductor",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "SkinColorId",
                table: "Abductor",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "SkinColors",
                columns: table => new
                {
                    SkinColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinColors", x => x.SkinColorId);
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "GenderId", "Code", "DisplayName" },
                values: new object[,]
                {
                    { 1, "M", "Male" },
                    { 2, "F", "Female" },
                    { 3, "U", "Unknown" }
                });

            migrationBuilder.InsertData(
                table: "SkinColors",
                columns: new[] { "SkinColorId", "Name" },
                values: new object[,]
                {
                    { 1, "Light" },
                    { 2, "Medium" },
                    { 3, "Dark" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Victim_GenderId",
                table: "Victim",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Victim_SkinColorId",
                table: "Victim",
                column: "SkinColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Abductor_GenderId",
                table: "Abductor",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Abductor_SkinColorId",
                table: "Abductor",
                column: "SkinColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abductor_Genders_GenderId",
                table: "Abductor",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "GenderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Abductor_SkinColors_SkinColorId",
                table: "Abductor",
                column: "SkinColorId",
                principalTable: "SkinColors",
                principalColumn: "SkinColorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Victim_Genders_GenderId",
                table: "Victim",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "GenderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Victim_SkinColors_SkinColorId",
                table: "Victim",
                column: "SkinColorId",
                principalTable: "SkinColors",
                principalColumn: "SkinColorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abductor_Genders_GenderId",
                table: "Abductor");

            migrationBuilder.DropForeignKey(
                name: "FK_Abductor_SkinColors_SkinColorId",
                table: "Abductor");

            migrationBuilder.DropForeignKey(
                name: "FK_Victim_Genders_GenderId",
                table: "Victim");

            migrationBuilder.DropForeignKey(
                name: "FK_Victim_SkinColors_SkinColorId",
                table: "Victim");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "SkinColors");

            migrationBuilder.DropIndex(
                name: "IX_Victim_GenderId",
                table: "Victim");

            migrationBuilder.DropIndex(
                name: "IX_Victim_SkinColorId",
                table: "Victim");

            migrationBuilder.DropIndex(
                name: "IX_Abductor_GenderId",
                table: "Abductor");

            migrationBuilder.DropIndex(
                name: "IX_Abductor_SkinColorId",
                table: "Abductor");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Victim");

            migrationBuilder.DropColumn(
                name: "SkinColorId",
                table: "Victim");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Abductor");

            migrationBuilder.DropColumn(
                name: "SkinColorId",
                table: "Abductor");

            migrationBuilder.AddColumn<string>(
                name: "VictimSex",
                table: "Victim",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VictimSkinColor",
                table: "Victim",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AbductorSex",
                table: "Abductor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AbductorSkinColor",
                table: "Abductor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

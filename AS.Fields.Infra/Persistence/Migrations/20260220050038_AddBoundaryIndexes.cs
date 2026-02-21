using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Fields.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBoundaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Fields_Boundary_Lat",
                table: "fields",
                columns: new[] { "Boundary_LatMin", "Boundary_LatMax" });

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Boundary_Long",
                table: "fields",
                columns: new[] { "Boundary_LongMin", "Boundary_LongMax" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fields_Boundary_Lat",
                table: "fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_Boundary_Long",
                table: "fields");
        }
    }
}

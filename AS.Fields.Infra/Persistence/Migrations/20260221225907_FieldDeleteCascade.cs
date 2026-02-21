using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Fields.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FieldDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fields_properties_PropertyId",
                table: "fields");

            migrationBuilder.AddForeignKey(
                name: "FK_fields_properties_PropertyId",
                table: "fields",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fields_properties_PropertyId",
                table: "fields");

            migrationBuilder.AddForeignKey(
                name: "FK_fields_properties_PropertyId",
                table: "fields",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id");
        }
    }
}

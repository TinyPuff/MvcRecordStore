using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcRecordStore.Migrations
{
    /// <inheritdoc />
    public partial class AddedRecordType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Records");
        }
    }
}

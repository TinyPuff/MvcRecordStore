using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcRecordStore.Migrations
{
    /// <inheritdoc />
    public partial class AddedInvoiceModelAndUpdatedOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_BuyerID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BuyerID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BuyerID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GatewayAccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GatewayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GatewayResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSucceed = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    TrackingNumber = table.Column<long>(type: "bigint", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_BuyerID",
                        column: x => x.BuyerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InvoiceID",
                table: "Orders",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BuyerID",
                table: "Invoices",
                column: "BuyerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Invoices_InvoiceID",
                table: "Orders",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Invoices_InvoiceID",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Orders_InvoiceID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "BuyerID",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TrackingNumber",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BuyerID",
                table: "Orders",
                column: "BuyerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_BuyerID",
                table: "Orders",
                column: "BuyerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

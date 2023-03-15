using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRestAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    DocumentNumber = table.Column<string>(type: "text", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.DocumentNumber);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    CustomerDocumentNumber = table.Column<string>(type: "text", nullable: false),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Account_Customer_CustomerDocumentNumber",
                        column: x => x.CustomerDocumentNumber,
                        principalTable: "Customer",
                        principalColumn: "DocumentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromBankName = table.Column<string>(type: "text", nullable: false),
                    FromCustomerDocumentNumber = table.Column<string>(type: "text", nullable: true),
                    FromBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToBankName = table.Column<string>(type: "text", nullable: false),
                    ToCustomerDocumentNumber = table.Column<string>(type: "text", nullable: true),
                    ToBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Bank_FromBankId",
                        column: x => x.FromBankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Bank_ToBankId",
                        column: x => x.ToBankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Customer_FromCustomerDocumentNumber",
                        column: x => x.FromCustomerDocumentNumber,
                        principalTable: "Customer",
                        principalColumn: "DocumentNumber");
                    table.ForeignKey(
                        name: "FK_Transfer_Customer_ToCustomerDocumentNumber",
                        column: x => x.ToCustomerDocumentNumber,
                        principalTable: "Customer",
                        principalColumn: "DocumentNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_BankId",
                table: "Account",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CustomerDocumentNumber",
                table: "Account",
                column: "CustomerDocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromAccountId",
                table: "Transfer",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromBankId",
                table: "Transfer",
                column: "FromBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromCustomerDocumentNumber",
                table: "Transfer",
                column: "FromCustomerDocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToAccountId",
                table: "Transfer",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToBankId",
                table: "Transfer",
                column: "ToBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToCustomerDocumentNumber",
                table: "Transfer",
                column: "ToCustomerDocumentNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfer");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}

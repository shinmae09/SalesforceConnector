using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceDataConnector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Salesforce_Service_Provider",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemModstamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salesforce_Service_Provider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salesforce_Opportunity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemModstamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceProviderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salesforce_Opportunity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salesforce_Opportunity_Salesforce_Service_Provider_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "Salesforce_Service_Provider",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Salesforce_Contact",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanReceiveAlert = table.Column<bool>(type: "bit", nullable: false),
                    SystemModstamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpportunityId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salesforce_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salesforce_Contact_Salesforce_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Salesforce_Opportunity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salesforce_Contact_OpportunityId",
                table: "Salesforce_Contact",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Salesforce_Opportunity_ServiceProviderId",
                table: "Salesforce_Opportunity",
                column: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salesforce_Contact");

            migrationBuilder.DropTable(
                name: "Salesforce_Opportunity");

            migrationBuilder.DropTable(
                name: "Salesforce_Service_Provider");
        }
    }
}

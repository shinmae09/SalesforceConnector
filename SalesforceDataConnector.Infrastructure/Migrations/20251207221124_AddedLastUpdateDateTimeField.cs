using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceDataConnector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastUpdateDateTimeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Salesforce_Service_Provider",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Salesforce_Opportunity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Salesforce_Contact",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Salesforce_Service_Provider");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Salesforce_Opportunity");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Salesforce_Contact");
        }
    }
}

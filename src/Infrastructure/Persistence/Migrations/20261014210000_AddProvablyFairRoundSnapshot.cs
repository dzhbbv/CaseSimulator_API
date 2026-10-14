using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseSimulator.Infrastructure.Persistence.Migrations;

public partial class AddProvablyFairRoundSnapshot : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Algorithm",
            table: "ProvablyFairRounds",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false,
            defaultValue: "legacy-sha256-v0");

        migrationBuilder.AddColumn<string>(
            name: "DropTableJson",
            table: "ProvablyFairRounds",
            type: "jsonb",
            nullable: false,
            defaultValue: "[]");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Algorithm", table: "ProvablyFairRounds");
        migrationBuilder.DropColumn(name: "DropTableJson", table: "ProvablyFairRounds");
    }
}

using CaseSimulator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseSimulator.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261017210000_AddUniqueProvablyFairRoundNonce")]
public partial class AddUniqueProvablyFairRoundNonce : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_ProvablyFairRounds_UserId_ServerSeedHash_Nonce",
            table: "ProvablyFairRounds",
            columns: new[] { "UserId", "ServerSeedHash", "Nonce" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ProvablyFairRounds_UserId_ServerSeedHash_Nonce",
            table: "ProvablyFairRounds");
    }
}

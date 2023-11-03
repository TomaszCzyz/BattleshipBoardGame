using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleshipBoardGame.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerInfo_Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerInfo_GuessingStrategy = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerInfo_ShipsPlacementStrategy = table.Column<int>(type: "INTEGER", nullable: false),
                    Ships = table.Column<string>(type: "TEXT", nullable: false),
                    Guesses = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Simulations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsFinished = table.Column<bool>(type: "INTEGER", nullable: false),
                    Player1Id = table.Column<Guid>(type: "TEXT", nullable: true),
                    Player2Id = table.Column<Guid>(type: "TEXT", nullable: true),
                    WinnerId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulations_PlayerDto_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "PlayerDto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Simulations_PlayerDto_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "PlayerDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_Player1Id",
                table: "Simulations",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_Player2Id",
                table: "Simulations",
                column: "Player2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Simulations");

            migrationBuilder.DropTable(
                name: "PlayerDto");
        }
    }
}

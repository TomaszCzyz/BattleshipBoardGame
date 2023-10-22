namespace BattleshipBoardGame.Models;

public class Simulation
{
    public required Guid Id { get; set; }

    public required bool IsFinished { get; set; }

    public PlayerDto? Player1 { get; set; }

    public PlayerDto? Player2 { get; set; }
}

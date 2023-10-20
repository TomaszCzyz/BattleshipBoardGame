namespace BattleshipBoardGame.Models;

public class Simulation
{
    public required Guid Id { get; set; }

    public required bool IsFinished { get; set; }

    public Player? Player1 { get; set; }

    public Player? Player2 { get; set; }
}

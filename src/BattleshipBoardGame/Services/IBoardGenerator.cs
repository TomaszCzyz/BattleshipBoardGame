namespace BattleshipBoardGame.Services;

public interface IBoardGenerator
{
    public sbyte[,] Generate(Strategy strategy = Strategy.Simple);
}

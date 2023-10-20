namespace BattleshipBoardGame.Services;

public interface IBattleshipGameSimulator
{
    Task StartNew();
}

public class BattleshipGameSimulator : IBattleshipGameSimulator
{
    public Task StartNew() => throw new NotImplementedException();
}

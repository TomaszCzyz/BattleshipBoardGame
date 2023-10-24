namespace BattleshipBoardGame.Helpers;

public static class Array2dHelpers
{
    public static sbyte[,] Initialize(int len, sbyte value = -1)
    {
        var board = new sbyte[len, len];

        foreach (var row in Enumerable.Range(0, len))
        {
            foreach (var col in Enumerable.Range(0, len))
            {
                board[row, col] = value;
            }
        }

        return board;
    }
}

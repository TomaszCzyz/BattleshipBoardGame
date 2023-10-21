namespace BattleshipBoardGame.Extensions;

public static class ArrayExtensions
{
    public static T[,] To2D<T>(this List<List<T>> source)
    {
        var firstDim = source.Count;
        var secondDim = source.Select(row => row.Count).FirstOrDefault();
        if (source.Any(row => row.Count != secondDim))
        {
            throw new InvalidOperationException();
        }

        var result = new T[firstDim, secondDim];
        for (var i = 0; i < firstDim; i++)
        {
            for (int j = 0, count = source[i].Count; j < count; j++)
            {
                result[i, j] = source[i][j];
            }
        }

        return result;
    }
}

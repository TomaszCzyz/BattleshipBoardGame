using System.Text;

namespace BattleshipBoardGame.Extensions;

public static class Byte2dArrayExtensions
{
    public static string PrintToString(this sbyte[,] array)
    {
        var sb = new StringBuilder();

        foreach (var row in EnumerateRows(array))
        {
            foreach (var c in row)
            {
                sb.Append(c == 1 ? '#' : '_');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static IEnumerable<IEnumerable<sbyte>> EnumerateRows(this sbyte[,] array)
        => Enumerable.Range(0, array.GetLength(0)).Select(i => Enumerable.Range(0, array.GetLength(1)).Select(j => array[i, j]));

    public static IEnumerable<sbyte> Enumerate(this sbyte[,] array)
        => Enumerable.Range(0, array.GetLength(0)).SelectMany(i => Enumerable.Range(0, array.GetLength(1)).Select(j => array[i, j]));
}

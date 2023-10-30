namespace BattleshipBoardGame.Models.Entities;

public struct Point : IEquatable<Point>
{
    public int Row { get; set; }

    public int Col { get; set; }

    public Point(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public bool Equals(Point other) => Row == other.Row && Col == other.Col;

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Row, Col);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !(left == right);

    public void Deconstruct(out int row, out int col)
    {
        row = Row;
        col = Col;
    }
}

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BattleshipBoardGame.Extensions;

namespace BattleshipBoardGame.Serialization;

public class ArraySByte2DConverter : JsonConverter<sbyte[,]>
{
    public override sbyte[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<List<List<sbyte>>>(ref reader, options)?.To2D();
    }

    public override void Write(Utf8JsonWriter writer, sbyte[,] value, JsonSerializerOptions options)
    {
        var rowsFirstIndex = value.GetLowerBound(0);
        var rowsLastIndex = value.GetUpperBound(0);
        var columnsFirstIndex = value.GetLowerBound(1);
        var columnsLastIndex = value.GetUpperBound(1);

        writer.WriteStartArray();
        for (var i = rowsFirstIndex; i <= rowsLastIndex; i++)
        {
            writer.WriteStartArray();
            for (var j = columnsFirstIndex; j <= columnsLastIndex; j++)
            {
                writer.WriteStringValue(value[i, j].ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}

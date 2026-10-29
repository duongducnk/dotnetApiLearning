using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyDotNetApi.Models;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = new[]
    {
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateStr = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateStr))
        {
            return default;
        }

        // Thử parse theo các định dạng thông dụng
        if (DateTime.TryParseExact(dateStr, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }

        // Fallback về cách parse mặc định nếu không khớp các format trên
        return DateTime.Parse(dateStr, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
       writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
    }
}
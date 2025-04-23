using Newtonsoft.Json;
using System;

public class UtcDateTimeConverterNewtonsoft : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var value = reader.Value;

        if (value == null)
            return DateTime.MinValue;

        var dt = DateTime.Parse(value.ToString());

        if (dt.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return dt.ToUniversalTime();
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToUniversalTime());
    }
}

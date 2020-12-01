using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public partial class FotoWareImageMetaDataModel
  {
    [JsonProperty("created")]
    public DateTimeOffset Created { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("modified")]
    public DateTimeOffset Modified { get; set; }

    [JsonProperty("modifiedBy")]
    public string ModifiedBy { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("builtinFields")]
    public List<BuiltinField> BuiltinFields { get; set; }
  }

  public partial class BuiltinField
  {
    [JsonProperty("field")]
    public string Field { get; set; }

    [JsonProperty("value")]
    [JsonConverter(typeof(ValueConverter))]
    public Value Value { get; set; }
  }

  public partial struct Value
  {
    public string String;
    public List<string> StringArray;

    public static implicit operator Value(string String) => new Value { String = String };
    public static implicit operator Value(List<string> StringArray) => new Value { StringArray = StringArray };
  }

  internal static class Converter
  {
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
      MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
      DateParseHandling = DateParseHandling.None,
      Converters =
            {
                ValueConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
  }

  internal class ValueConverter : JsonConverter
  {
    public override bool CanConvert(Type t) => t == typeof(Value) || t == typeof(Value?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
      switch (reader.TokenType)
      {
        case JsonToken.String:
        case JsonToken.Date:
          var stringValue = serializer.Deserialize<string>(reader);
          return new Value { String = stringValue };
        case JsonToken.StartArray:
          var arrayValue = serializer.Deserialize<List<string>>(reader);
          return new Value { StringArray = arrayValue };
      }
      throw new Exception("Cannot unmarshal type Value");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
      var value = (Value)untypedValue;
      if (value.String != null)
      {
        serializer.Serialize(writer, value.String);
        return;
      }
      if (value.StringArray != null)
      {
        serializer.Serialize(writer, value.StringArray);
        return;
      }
      throw new Exception("Cannot marshal type Value");
    }

    public static readonly ValueConverter Singleton = new ValueConverter();
  }
}

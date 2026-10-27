using System.Text.Json.Serialization;

namespace Wolfgang.Gardening.Calculator.Web;

// Source-generated so the trimmed WebAssembly app needs no reflection. Enums are saved by name.
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true)]
[JsonSerializable(typeof(SavedList))]
internal sealed partial class SavedBedsJson : JsonSerializerContext
{
}

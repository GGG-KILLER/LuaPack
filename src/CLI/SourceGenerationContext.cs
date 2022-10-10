using System.Text.Json.Serialization;
using LuaPack.Core;

namespace LuaPack;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    IgnoreReadOnlyProperties = true,
    IncludeFields = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
[JsonSerializable(typeof(ProjectFile))]
[JsonSerializable(typeof(NullableLuaSyntaxOptions))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

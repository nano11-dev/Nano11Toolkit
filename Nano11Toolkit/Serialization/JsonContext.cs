using System.Text.Json.Serialization;
using Nano11Toolkit.Models;

namespace Nano11Toolkit.Serialization
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ApplicationEntry))]
    [JsonSerializable(typeof(ApplicationEntry[]))]
    [JsonSerializable(typeof(TogglableEntry))]
    [JsonSerializable(typeof(TogglableEntry[]))]
    [JsonSerializable(typeof(ButtonEntry))]
    [JsonSerializable(typeof(ButtonEntry[]))]
    internal partial class Nano11JsonContext : JsonSerializerContext
    {
    }
}
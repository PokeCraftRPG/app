using PokeCraft.Domain.Speciez;

namespace PokeCraft.Infrastructure.Converters;

internal class SpeciesNumberConverter : JsonConverter<SpeciesNumber>
{
  public override SpeciesNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new SpeciesNumber(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, SpeciesNumber speciesNumber, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(speciesNumber.Value);
  }
}

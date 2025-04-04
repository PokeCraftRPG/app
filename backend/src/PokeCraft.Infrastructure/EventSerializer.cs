﻿using PokeCraft.Infrastructure.Converters;

namespace PokeCraft.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new AbilityIdConverter());
    SerializerOptions.Converters.Add(new AccuracyConverter());
    SerializerOptions.Converters.Add(new CatchRateConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new DisplayNameConverter());
    SerializerOptions.Converters.Add(new FriendshipConverter());
    SerializerOptions.Converters.Add(new MoveIdConverter());
    SerializerOptions.Converters.Add(new NotesConverter());
    SerializerOptions.Converters.Add(new PowerConverter());
    SerializerOptions.Converters.Add(new PowerPointsConverter());
    SerializerOptions.Converters.Add(new RegionIdConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
    SerializerOptions.Converters.Add(new SpeciesIdConverter());
    SerializerOptions.Converters.Add(new SpeciesNumberConverter());
    SerializerOptions.Converters.Add(new StorageIdConverter());
    SerializerOptions.Converters.Add(new UniqueNameConverter());
    SerializerOptions.Converters.Add(new UrlConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
    SerializerOptions.Converters.Add(new VolatileConditionConverter());
    SerializerOptions.Converters.Add(new WorldIdConverter());
  }
}

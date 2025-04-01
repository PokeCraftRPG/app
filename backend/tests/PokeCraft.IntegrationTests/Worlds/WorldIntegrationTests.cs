using PokeCraft.Application.Worlds.Commands;
using PokeCraft.Application.Worlds.Models;

namespace PokeCraft.Worlds;

[Trait(Traits.Category, Categories.Integration)]
public class WorldIntegrationTests : IntegrationTests
{
  public WorldIntegrationTests() : base()
  {
  }

  [Theory(DisplayName = "CreateOrReplace: it should create a new world.")]
  [InlineData(null)]
  [InlineData("81ed7d52-fdb5-440a-806e-d82538cadc0e")]
  public async Task Given_WorldDoesNotExist_When_CreateOrReplace_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    CreateOrReplaceWorldPayload payload = new()
    {
      UniqueSlug = "pokemon-world",
      DisplayName = " Pokémon World ",
      Description = "    "
    };
    CreateOrReplaceWorldCommand command = new(id, payload);
    CreateOrReplaceWorldResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    WorldModel model = result.World;
    if (id.HasValue)
    {
      Assert.Equal(id.Value, model.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, model.Id);
    }
    Assert.Equal(2, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(DateTime.Now, model.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(DateTime.Now, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(Actor, model.Owner);

    Assert.Equal(payload.UniqueSlug, model.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), model.DisplayName);
    Assert.Null(model.Description);
  }

  [Fact(DisplayName = "CreateOrReplace: it should replace an existing world.")]
  public Task Given_WorldExists_When_CreateOrReplace_Then_Replaced()
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}

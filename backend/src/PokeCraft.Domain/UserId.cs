using Logitar.EventSourcing;

namespace PokeCraft.Domain;

public readonly struct UserId
{
  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public UserId(string value)
  {
    ActorId = new(value);
  }
  public UserId(Guid id)
  {
    ActorId = new(id);
  }
  public UserId(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Guid ToGuid() => ActorId.ToGuid();

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

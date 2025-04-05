﻿using FluentValidation;

namespace PokeCraft.Domain;

public record Description
{
  public string Value { get; }
  public int Size => Value.Length;

  public Description(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Description? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Description>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Description();
    }
  }
}

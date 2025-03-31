﻿namespace PokeCraft.Domain;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public Dictionary<string, object?> Data { get; set; } = [];

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message, IEnumerable<KeyValuePair<string, object?>>? data = null)
  {
    Code = code;
    Message = message;

    if (data is not null)
    {
      foreach (KeyValuePair<string, object?> item in data)
      {
        Data[item.Key] = item.Value;
      }
    }
  }
}

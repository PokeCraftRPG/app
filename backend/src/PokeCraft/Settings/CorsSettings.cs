namespace PokeCraft.Settings;

internal record CorsSettings
{
  public bool AllowAnyOrigin { get; set; }
  public string[] AllowedOrigins { get; set; } = [];

  public bool AllowAnyMethod { get; set; }
  public string[] AllowedMethods { get; set; } = [];

  public bool AllowAnyHeader { get; set; }
  public string[] AllowedHeaders { get; set; } = [];

  public bool AllowCredentials { get; set; }

  public static CorsSettings Initialize(IConfiguration configuration) => configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
}

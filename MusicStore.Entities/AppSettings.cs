namespace MusicStore.Entities;

public class AppSettings
{
    public Jwt Jwt { get; set; } = default!;
    public StorageConfiguration StorageConfiguration { get; set; } = default!;
}

public class StorageConfiguration
{
    public string PublicUrl { get; set; } = default!;
    public string Path { get; set; } = default!;
}

public class Jwt
{
    public string Audience { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string SecretKey { get; set; } = default!;

}
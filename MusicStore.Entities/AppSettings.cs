namespace MusicStore.Entities;

public class AppSettings
{
    public StorageConfiguration StorageConfiguration { get; set; } = default!;
}

public class StorageConfiguration
{
    public string PublicUrl { get; set; } = default!;
    public string Path { get; set; } = default!;
}
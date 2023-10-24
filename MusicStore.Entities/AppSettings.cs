namespace MusicStore.Entities;

public class AppSettings
{
    public Jwt Jwt { get; set; } = default!;
    public SmtpConfiguration SmtpConfiguration { get; set; } = default!;
    public StorageConfiguration StorageConfiguration { get; set; } = default!;
}

public class StorageConfiguration
{
    public string PublicUrl { get; set; } = default!;
    public string Path { get; set; } = default!;
}

public class SmtpConfiguration
{
    public string UserName { get; set; } = default!;
    public string Server { get; set; } = default!;
    public int PortNumber { get; set; }
    public string Password { get; set; } = default!;
    public string FromName { get; set; } = default!;
    public bool EnableSsl { get; set; }
    
}


public class Jwt
{
    public string Audience { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string SecretKey { get; set; } = default!;

}
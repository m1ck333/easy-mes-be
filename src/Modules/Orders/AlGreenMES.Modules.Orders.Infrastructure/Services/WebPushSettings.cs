namespace AlGreenMES.Modules.Orders.Infrastructure.Services;

public class WebPushSettings
{
    public string VapidPublicKey { get; set; } = string.Empty;
    public string VapidPrivateKey { get; set; } = string.Empty;
    public string VapidSubject { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}

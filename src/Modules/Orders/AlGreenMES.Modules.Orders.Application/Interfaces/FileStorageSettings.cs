namespace AlGreenMES.Modules.Orders.Application.Interfaces;

public class FileStorageSettings
{
    public string BasePath { get; set; } = "./uploads";
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB
    public int MaxFilesPerOrder { get; set; } = 10;
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".pdf"];
    public string[] AllowedContentTypes { get; set; } =
    [
        "image/jpeg",
        "image/png",
        "application/pdf"
    ];
}

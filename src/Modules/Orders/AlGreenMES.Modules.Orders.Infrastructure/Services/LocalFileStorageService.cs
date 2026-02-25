using AlGreenMES.Modules.Orders.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace AlGreenMES.Modules.Orders.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IOptions<FileStorageSettings> settings)
    {
        _basePath = Path.GetFullPath(settings.Value.BasePath);
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(string relativePath, Stream fileStream, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafePath(relativePath);
        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        await using var fileStreamOut = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

        return relativePath;
    }

    public Task<Stream?> GetFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafePath(relativePath);
        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafePath(relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Task DeleteDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafePath(relativePath);
        if (Directory.Exists(fullPath))
            Directory.Delete(fullPath, recursive: true);

        return Task.CompletedTask;
    }

    private string GetSafePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));
        if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Invalid file path.");
        return fullPath;
    }
}

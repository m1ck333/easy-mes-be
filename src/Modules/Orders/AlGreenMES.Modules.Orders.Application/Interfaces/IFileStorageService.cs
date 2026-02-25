namespace AlGreenMES.Modules.Orders.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(string relativePath, Stream fileStream, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string relativePath, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
    Task DeleteDirectoryAsync(string relativePath, CancellationToken cancellationToken = default);
}

namespace Phronesis.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string directory, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUri, CancellationToken cancellationToken = default);
}

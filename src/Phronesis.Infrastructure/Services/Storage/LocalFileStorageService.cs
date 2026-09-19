using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Infrastructure.Services.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string directory, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("Stream is empty or null.", nameof(fileStream));

        var basePath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(basePath, directory);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream, cancellationToken);
        }

        // Return the relative URI to save in the DB (e.g. "uploads/documents/xxx.pdf")
        return $"{directory}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUri, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUri)) return Task.CompletedTask;

        var basePath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var fullPath = Path.Combine(basePath, fileUri.Replace("/", "\\"));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}

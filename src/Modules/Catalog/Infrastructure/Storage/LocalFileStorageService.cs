using Microsoft.AspNetCore.Hosting;
using SwiftScale.BuildingBlocks.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Catalog.Infrastructure.Storage
{
    internal sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
    {
        public async Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default)
        {
            var uploadsFolder = Path.Combine(environment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique name to prevent overwriting
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var outputStream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(outputStream, ct);

            return $"/images/{uniqueFileName}"; // The relative URL for the browser
        }

        public Task DeleteAsync(string filePath, CancellationToken ct = default)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}

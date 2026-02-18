using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.BuildingBlocks.Storage
{
    public interface IFileStorageService
    {
        // Returns the public URL or file path
        Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default);
        Task DeleteAsync(string filePath, CancellationToken ct = default);
    }
}

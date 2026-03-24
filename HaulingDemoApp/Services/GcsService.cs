using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace HaulingDemoApp.Services;

public class GcsService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    private readonly ILogger<GcsService> _logger;

    public GcsService(IConfiguration configuration, ILogger<GcsService> logger)
    {
        _logger = logger;

        // Setup Google Cloud credentials
        var credentialPath = configuration["GoogleCloudStorage:CredentialPath"];
        if (string.IsNullOrEmpty(credentialPath))
        {
            throw new InvalidOperationException("Google Cloud Storage credential path is not configured");
        }

        // Set environment variable for Google credentials
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        _bucketName = configuration["GoogleCloudStorage:BucketName"]
            ?? throw new InvalidOperationException("Google Cloud Storage bucket name is not configured");

        try
        {
            _storageClient = StorageClient.Create();
            _logger.LogInformation("GCS Service initialized successfully for bucket: {BucketName}", _bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize GCS Service");
            throw;
        }
    }

    /// <summary>
    /// Upload file ke Google Cloud Storage
    /// </summary>
    /// <param name="filePath">Local path file yang akan diupload</param>
    /// <param name="objectName">Nama object di GCS (bisa termasuk folder structure)</param>
    /// <param name="contentType">Content type (optional)</param>
    /// <returns>Public URL dari file yang diupload</returns>
    public async Task<string> UploadFileAsync(string filePath, string objectName, string? contentType = null)
    {
        try
        {
            _logger.LogInformation("Uploading file {FilePath} to GCS as {ObjectName}", filePath, objectName);

            using var fileStream = File.OpenRead(filePath);
            var uploadObject = await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                contentType ?? "application/octet-stream",
                fileStream
            );

            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
            _logger.LogInformation("File uploaded successfully: {Url}", publicUrl);

            return publicUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FilePath} to GCS", filePath);
            throw;
        }
    }

    /// <summary>
    /// Upload stream ke Google Cloud Storage
    /// </summary>
    /// <param name="stream">Stream yang akan diupload</param>
    /// <param name="objectName">Nama object di GCS</param>
    /// <param name="contentType">Content type (optional)</param>
    /// <returns>Public URL dari file yang diupload</returns>
    public async Task<string> UploadStreamAsync(Stream stream, string objectName, string? contentType = null)
    {
        try
        {
            _logger.LogInformation("Uploading stream to GCS as {ObjectName}", objectName);

            var uploadObject = await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                contentType ?? "application/octet-stream",
                stream
            );

            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
            _logger.LogInformation("Stream uploaded successfully: {Url}", publicUrl);

            return publicUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload stream to GCS as {ObjectName}", objectName);
            throw;
        }
    }

    /// <summary>
    /// Download file dari Google Cloud Storage
    /// </summary>
    /// <param name="objectName">Nama object di GCS</param>
    /// <param name="localPath">Local path untuk menyimpan file</param>
    public async Task DownloadFileAsync(string objectName, string localPath)
    {
        try
        {
            _logger.LogInformation("Downloading {ObjectName} from GCS to {LocalPath}", objectName, localPath);

            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fileStream = File.OpenWrite(localPath);
            await _storageClient.DownloadObjectAsync(_bucketName, objectName, fileStream);

            _logger.LogInformation("File downloaded successfully to {LocalPath}", localPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download {ObjectName} from GCS", objectName);
            throw;
        }
    }

    /// <summary>
    /// Download file dari GCS ke memory stream
    /// </summary>
    /// <param name="objectName">Nama object di GCS</param>
    /// <returns>MemoryStream berisi file</returns>
    public async Task<MemoryStream> DownloadToStreamAsync(string objectName)
    {
        try
        {
            _logger.LogInformation("Downloading {ObjectName} from GCS to stream", objectName);

            var memoryStream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(_bucketName, objectName, memoryStream);
            memoryStream.Position = 0; // Reset position for reading

            _logger.LogInformation("File downloaded to stream successfully");
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download {ObjectName} from GCS to stream", objectName);
            throw;
        }
    }

    /// <summary>
    /// Delete file dari Google Cloud Storage
    /// </summary>
    /// <param name="objectName">Nama object di GCS</param>
    public async Task DeleteFileAsync(string objectName)
    {
        try
        {
            _logger.LogInformation("Deleting {ObjectName} from GCS", objectName);

            await _storageClient.DeleteObjectAsync(_bucketName, objectName);

            _logger.LogInformation("File deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete {ObjectName} from GCS", objectName);
            throw;
        }
    }

    /// <summary>
    /// List semua files dalam bucket dengan prefix tertentu
    /// </summary>
    /// <param name="prefix">Prefix untuk filtering (misal: "fuel-receipts/")</param>
    /// <returns>List of object names</returns>
    public async Task<List<string>> ListFilesAsync(string? prefix = null)
    {
        try
        {
            _logger.LogInformation("Listing files in bucket {BucketName} with prefix {Prefix}", _bucketName, prefix);

            var files = new List<string>();
            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);

            await foreach (var obj in objects)
            {
                files.Add(obj.Name);
            }

            _logger.LogInformation("Found {Count} files", files.Count);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list files in bucket {BucketName}", _bucketName);
            throw;
        }
    }

    /// <summary>
    /// Cek apakah object ada di GCS
    /// </summary>
    /// <param name="objectName">Nama object di GCS</param>
    /// <returns>True jika object ada, false jika tidak</returns>
    public async Task<bool> FileExistsAsync(string objectName)
    {
        try
        {
            await _storageClient.GetObjectAsync(_bucketName, objectName);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    /// Generate object name dengan folder structure
    /// </summary>
    /// <param name="site">Site name</param>
    /// <param name="dataType">Data type (fuel-receipts/fuel-usage)</param>
    /// <param name="fileName">Original filename</param>
    /// <returns>Formatted object name</returns>
    public string GenerateObjectName(string site, string dataType, string fileName)
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var timestamp = DateTime.UtcNow.Ticks;
        var extension = Path.GetExtension(fileName);
        var sanitizedName = Path.GetFileNameWithoutExtension(fileName)
            .Replace(" ", "_")
            .Replace("/", "_")
            .Replace("\\", "_");

        return $"{dataType}/{site}/{date}/{sanitizedName}_{timestamp}{extension}";
    }
}

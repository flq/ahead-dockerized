using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Ahead.Web.Infrastructure;

public interface IBlobStorage
{
    Task<string> UploadFile(string bucketName, IFormFile formFile, CancellationToken cancellationToken = default);
    Task<string> GetPreSignedUrl(string bucketName, string objectName);
    IAsyncEnumerable<FileLink> ListLinks(string bucketName);
}

public record struct FileLink(string Id);

public class MinioConfig
{
    public required string Endpoint { get; init; }
    public int Port { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
}

public class MinioBlobStorage(IOptions<MinioConfig> config) : IBlobStorage
{
    private IMinioClient? client;
    private static readonly FileExtensionContentTypeProvider FileExtensionContentTypeProvider = new();

    public async Task<string> UploadFile(string bucketName, IFormFile file, CancellationToken cancellationToken = default)
    {
        await EnsureBucketExists(bucketName);
        var c = GetClient();
        var identifiedContentType = FileExtensionContentTypeProvider.TryGetContentType(file.FileName, out var contentType);
        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(Guid.NewGuid().ToString())
            .WithContentType(identifiedContentType ? contentType : MediaTypeNames.Application.Octet)
            .WithStreamData(file.OpenReadStream())
            .WithObjectSize(file.Length);
        var response = await c.PutObjectAsync(args, cancellationToken);
        return response.ObjectName;
    }

    public async Task<string> GetPreSignedUrl(string bucketName, string objectName)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(3600);

        var presignedGetObjectAsync = await GetClient().PresignedGetObjectAsync(args);
        return presignedGetObjectAsync;
    }
    public async IAsyncEnumerable<FileLink> ListLinks(string bucketName)
    {
        var c = GetClient();
        await foreach (var item in c.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(bucketName)))
        {
            yield return new FileLink(item.Key);
        }
    }

    private async Task EnsureBucketExists(string bucketName)
    {
        var c = GetClient();
        var exists = await c.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!exists)
        {
            await c.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
    }

    private MinioConfig Config => config.Value;

    private IMinioClient GetClient() =>
        client ??= new MinioClient()
            .WithEndpoint(Config.Endpoint, Config.Port)
            .WithCredentials(Config.AccessKey, Config.SecretKey)
            .Build();
}



using Minio;
using Minio.DataModel.Args;

namespace CareerPoint.Web.HostedServices;

/// <summary>
/// Инициализация бакета в Minio при его отсутствии
/// </summary>
public class MinioBucketInitializerHostedService : IHostedService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioBucketInitializerHostedService(IMinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        bool isExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName), cancellationToken);

        if (isExists)
            return;

        await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

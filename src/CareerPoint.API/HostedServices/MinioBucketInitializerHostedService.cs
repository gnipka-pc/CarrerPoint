
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
    private readonly ILogger<MinioBucketInitializerHostedService> _logger;

    public MinioBucketInitializerHostedService(IMinioClient minioClient, string bucketName, ILogger<MinioBucketInitializerHostedService> logger)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            bool isExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName), cancellationToken);

            if (isExists)
                return;

            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName), cancellationToken);
        }
        catch
        {
            _logger.LogWarning("Minio бакет для аватарок не проинициализирован");
        }
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

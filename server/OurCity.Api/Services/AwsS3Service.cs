using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

public class AwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public AwsS3Service(IConfiguration config)
    {
        var region = config["AWS__Region"];
        var accessKey = config["AWS__AccessKey"];
        var secretKey = config["AWS__SecretKey"];
        var bucketName = config["AWS__BucketName"];

        Console.WriteLine($"AWS__Region: {region}");
        Console.WriteLine($"AWS__AccessKey: {accessKey}");
        Console.WriteLine($"AWS__SecretKey: {secretKey}");
        Console.WriteLine($"AWS__BucketName: {bucketName}");

        if (string.IsNullOrEmpty(region))
            throw new Exception("AWS__Region is missing!");
            
        _s3Client = new AmazonS3Client(
            config["AWS__AccessKey"],
            config["AWS__SecretKey"],
            Amazon.RegionEndpoint.GetBySystemName(config["AWS__Region"])
        );
        _bucketName = config["AWS__BucketName"];
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = fileName,
            BucketName = _bucketName,
            ContentType = "application/octet-stream"
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
    }
}
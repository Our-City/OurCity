using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

public class AwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public AwsS3Service(IOptions<AwsS3Options> options)
    {
        var awsOptions = options.Value;

        if (string.IsNullOrEmpty(awsOptions.Region))
            throw new Exception("AWS Region is missing!");
        if (string.IsNullOrEmpty(awsOptions.AccessKey))
            throw new Exception("AWS AccessKey is missing!");
        if (string.IsNullOrEmpty(awsOptions.SecretKey))
            throw new Exception("AWS SecretKey is missing!");
        if (string.IsNullOrEmpty(awsOptions.BucketName))
            throw new Exception("AWS BucketName is missing!");
            
        // Using the properties from our options object to create the S3 client.
        _s3Client = new AmazonS3Client(
            awsOptions.AccessKey,
            awsOptions.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(awsOptions.Region)
        );
        _bucketName = awsOptions.BucketName;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        // Generating a unique file name to prevent overwrites and collisions.
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

        var s3Key = $"media/{uniqueFileName}";          //storing in the media folder in S3 bucket

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = s3Key,                            //using unique filename with the required path
            BucketName = _bucketName,
            ContentType = "application/octet-stream",   
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        // Returning the full public URL of the uploaded file.
        return $"https://{_bucketName}.s3.amazonaws.com/{s3Key}";
    }

    public async Task DeleteFileAsync(string key)
    {
        await _s3Client.DeleteObjectAsync(_bucketName, key);
    }
}

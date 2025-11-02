public class AwsS3Options
{
    // This constant is used as a single, reliable name for the config section.
    public const string AWS = "AWS";

    // These property names (Region, BucketName, etc.) need to match the names
    // in the .env file after the "AWS__" prefix.
    public required string Region { get; set; }
    public required string BucketName { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
}

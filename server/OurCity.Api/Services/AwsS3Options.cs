public class AwsS3Options
{
    // This constant is used as a single, reliable name for the config section.
    public const string AWS = "AWS";

    // These property names (Region, BucketName, etc.) need to match the names
    // in the .env file after the "AWS__" prefix.
    public string Region { get; set; }
    public string BucketName { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
}
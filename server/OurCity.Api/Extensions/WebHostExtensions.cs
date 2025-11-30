namespace OurCity.Api.Extensions;

public static class WebHostExtensions
{
    private static readonly string TestEnvironment = "Testweb";

    public static bool IsTestEnvironment(this IWebHostEnvironment environment)
    {
        return environment.IsEnvironment(TestEnvironment);
    }

    public static IWebHostBuilder UseTestEnvironment(this IWebHostBuilder builder)
    {
        builder.UseEnvironment(TestEnvironment);

        return builder;
    }
}

namespace OurCity.Api.Infrastructure.Database;

public interface ITenantProvider
{
    string TenantName { get; set; }
}

public class TenantProvider : ITenantProvider
{
    public required string TenantName { get; set; }
}

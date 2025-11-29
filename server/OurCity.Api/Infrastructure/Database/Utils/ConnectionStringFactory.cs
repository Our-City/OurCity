namespace OurCity.Api.Infrastructure.Database.Utils;

public interface ITenantConnectionStringFactory
{
    string GetConnectionString(string tenantName);
}

public class TenantConnectionStringFactory : ITenantConnectionStringFactory
{
    private readonly HostDbContext _hostDbContext;

    public TenantConnectionStringFactory(HostDbContext hostDbContext)
    {
        _hostDbContext = hostDbContext;
    }

    public string GetConnectionString(string tenantName)
    {
        var tenant = _hostDbContext
            .Tenants.AsNoTracking()
            .FirstOrDefault(tenant => tenant.Name == tenantName);

        //TODO: exception..
        if (tenant == null)
        {
            throw new Exception($"{tenantName} was not found");
        }

        return $"Host={tenant.DbServer};Username={tenant.DbUser};Password={tenant.DbPassword};Database={tenant.DbName}";
    }
}

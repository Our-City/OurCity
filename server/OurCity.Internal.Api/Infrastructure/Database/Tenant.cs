namespace OurCity.Internal.Api.Infrastructure.Database;

public class Tenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }
    public required string DbServer { get; set; }
    public required string DbName { get; set; }
    public required string DbUser { get; set; }
    public required string DbPassword { get; set; }
}

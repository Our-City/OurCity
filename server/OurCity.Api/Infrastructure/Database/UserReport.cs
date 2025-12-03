namespace OurCity.Api.Infrastructure.Database;

public class UserReport
{
    public Guid Id { get; set; }

    public Guid TargetUserId { get; set; }

    public Guid ReporterId { get; set; }

    public required string Reason { get; set; }

    public DateTime ReportedAt { get; set; }

    // Navigation Properties
    public User? TargetUser { get; set; }

    public User? Reporter { get; set; }
}

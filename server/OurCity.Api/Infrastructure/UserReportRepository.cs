using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IUserReportRepository
{
    Task<UserReport?> GetReportByReporterAndTargetUserId(Guid reporterId, Guid targetUserId);
    Task<IEnumerable<UserReport>> GetReportsByTargetUser(Guid targetUserId);
    Task<int> GetReportsCountByTargetUser(Guid targetUserId);
    Task Add(UserReport report);
    Task SaveChangesAsync();
}

public class UserReportRepository : IUserReportRepository
{
    private readonly AppDbContext _appDbContext;

    public UserReportRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<UserReport?> GetReportByReporterAndTargetUserId(
        Guid reporterId,
        Guid targetUserId
    )
    {
        return await _appDbContext.UserReports.FirstOrDefaultAsync(r =>
            r.ReporterId == reporterId && r.TargetUserId == targetUserId
        );
    }

    public async Task<IEnumerable<UserReport>> GetReportsByTargetUser(Guid targetUserId)
    {
        return await _appDbContext
            .UserReports.Where(r => r.TargetUserId == targetUserId)
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();
    }

    public async Task<int> GetReportsCountByTargetUser(Guid targetUserId)
    {
        return await _appDbContext.UserReports.CountAsync(r => r.TargetUserId == targetUserId);
    }

    public async Task Add(UserReport report)
    {
        await _appDbContext.UserReports.AddAsync(report);
    }

    public async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}

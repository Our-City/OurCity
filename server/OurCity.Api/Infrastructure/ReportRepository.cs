using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IReportRepository
{
    Task<Report?> GetReportByReporterAndTargetId(Guid reporterId, Guid targetId);
    Task<IEnumerable<Report>> GetReportsByTargetUser(Guid targetId);
    Task<int> GetReportsCountByTargetUser(Guid targetId);
    Task Add(Report report);
    Task Remove(Report report);
    Task SaveChangesAsync();
}

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _appDbContext;

    public ReportRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Report?> GetReportByReporterAndTargetId(Guid reporterId, Guid targetId)
    {
        return await _appDbContext.Reports
        .FirstOrDefaultAsync(r => r.ReporterId == reporterId && r.TargetId == targetId);
    }

    public async Task<IEnumerable<Report>> GetReportsByTargetUser(Guid targetId)
    {
        return await _appDbContext
            .Reports.Where(r => r.TargetId == targetId)
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();
    }

    public async Task<int> GetReportsCountByTargetUser(Guid targetId)
    {
        return await _appDbContext.Reports.CountAsync(r => r.TargetId == targetId);
    }

    public async Task Add(Report report)
    {
        await _appDbContext.Reports.AddAsync(report);
    }

    public Task Remove(Report report)
    {
        _appDbContext.Reports.Remove(report);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}

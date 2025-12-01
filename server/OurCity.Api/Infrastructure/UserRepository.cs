using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers(UserFilter? userFilter);
    Task<User> BanUser(User user);
    Task<User> UnbanUser(User user);
    Task<User> DeleteUser(User user);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<User>> GetUsers(UserFilter? userFilter)
    {
        var cursor = userFilter?.Cursor;
        var limit = userFilter?.Limit ?? 25;

        IQueryable<User> query = _appDbContext.Users;

        if (userFilter?.IsBanned is not null)
            query = query.Where(user => user.IsBanned);

        if (userFilter?.MinimumNumReports is not null)
            query = query.Where(user => user.ReceivedReports.Count >= userFilter.MinimumNumReports);

        query = (userFilter?.SortBy?.ToLower(), userFilter?.SortOrder) switch
        {
            ("reportCount", SortOrder.Asc) => query.OrderBy(u => u.ReceivedReports.Count),
            ("reportCount", SortOrder.Desc) => query.OrderByDescending(u =>
                u.ReceivedReports.Count
            ),
            _ => query.OrderByDescending(u => u.CreatedAt).ThenByDescending(p => p.Id),
        };

        if (cursor.HasValue)
        {
            var cursorUser = await _appDbContext.Users.FindAsync(cursor.Value);
            if (cursorUser != null)
            {
                query = query.Where(p =>
                    p.CreatedAt < cursorUser.CreatedAt
                    || (p.CreatedAt == cursorUser.CreatedAt && p.Id.CompareTo(cursorUser.Id) < 0)
                );
            }
        }

        return await query.Take(limit).ToListAsync();
    }

    public async Task<User> BanUser(User user)
    {
        user.IsBanned = true;

        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> UnbanUser(User user)
    {
        user.IsBanned = false;

        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        return user;
    }
    
    public async Task<User> DeleteUser(User user)
    {
        user.IsDeleted = true;

        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();

        return user;
    }
}

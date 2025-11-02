/// Generative AI - CoPilot GPT was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard repository functions
/// based on common patterns in C# for repository implementations
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(int id);
    Task<User?> GetUserByUsername(string username);
    Task<User> CreateUser(User user);
    Task<User> UpdateUser(User user);
    Task DeleteUser(User user);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _appDbContext
            .Users.Where(u => !u.IsDeleted)
            .Include(u => u.Posts)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _appDbContext
            .Users.Where(u => u.Id == id && !u.IsDeleted)
            .Include(u => u.Posts)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _appDbContext
            .Users.Where(u => u.Username == username && !u.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateUser(User user)
    {
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUser(User user)
    {
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUser(User user)
    {
        // soft deletion in db  (mark User as deleted)
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        return;
    }
}

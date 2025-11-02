/// Generative AI - CoPilot was used to assist in the creation of this file,
/// as it was largely based off of PostIntegrationTests.cs (see that file)
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services;
using Testcontainers.PostgreSql;

namespace OurCity.Api.Test.IntegrationTests;

/// Generative AI - CoPilot was used to assist in the creation of this file,
/// as it was largely based off of PostIntegrationTests.cs (see that file)
[Trait("Type", "Integration")]
[Trait("Domain", "User")]
public class UserIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16.10")
        .Build();
    private AppDbContext _dbContext = null!;
    private User _testUser = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _dbContext = new AppDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _testUser = new User
        {
            Username = "Test",
            DisplayName = "Display Test",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _dbContext.Users.Add(_testUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task GetCreatedUserShouldSucceed()
    {
        var userService = new UserService(new UserRepository(_dbContext));

        var retrievedUser = await userService.GetUserById(_testUser.Id);
        Assert.True(retrievedUser.IsSuccess);
        Assert.NotNull(retrievedUser.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal(_testUser.Username, retrievedUser.Data.Username);
            Assert.Equal(_testUser.DisplayName, retrievedUser.Data.DisplayName);
        });
    }

    [Fact]
    public async Task CreateUserShouldAddAndReturnUser()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var createDto = new UserCreateRequestDto { Username = "Test Username" };

        var createdUser = await userService.CreateUser(createDto);
        Assert.True(createdUser.IsSuccess);
        Assert.NotNull(createdUser.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal("Test Username", createdUser.Data.Username);
            Assert.Null(createdUser.Data.DisplayName);
            Assert.False(createdUser.Data.IsDeleted);
        });
    }

    [Fact]
    public async Task CreateUserWithExistingUsernameShouldFail()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var createDto = new UserCreateRequestDto { Username = _testUser.Username };

        var createdUser = await userService.CreateUser(createDto);

        Assert.False(createdUser.IsSuccess);
        Assert.Equal("Username already exists.", createdUser.Error);
    }

    [Fact]
    public async Task UpdateUserShouldModifyAndReturnUser()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var updateDto = new UserUpdateRequestDto
        {
            Username = "Updated Username",
            DisplayName = "Updated Display Name",
        };
        var updatedUser = await userService.UpdateUser(_testUser.Id, updateDto);

        Assert.True(updatedUser.IsSuccess);
        Assert.NotNull(updatedUser.Data);
        Assert.Equal("Updated Display Name", updatedUser.Data.DisplayName);
        Assert.Equal("Updated Username", updatedUser.Data.Username);
    }

    [Fact]
    public async Task UpdateNonExistentUserShouldFail()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var updateDto = new UserUpdateRequestDto
        {
            Username = "NonExistent",
            DisplayName = "Non Existent",
        };
        var updatedUser = await userService.UpdateUser(67, updateDto);

        Assert.False(updatedUser.IsSuccess);
        Assert.Equal("User not found.", updatedUser.Error);
    }

    [Fact]
    public async Task GetUserByUsernameShouldReturnCreatedUser()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var retrievedUser = await userService.GetUserByUsername(_testUser.Username);

        Assert.True(retrievedUser.IsSuccess);
        Assert.NotNull(retrievedUser.Data);
        Assert.Equal(_testUser.Username, retrievedUser.Data.Username);
        Assert.Equal(_testUser.DisplayName, retrievedUser.Data.DisplayName);
    }

    [Fact]
    public async Task GetUserByNonExistentUsernameShouldFail()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var retrievedUser = await userService.GetUserByUsername("NonExistentUsername");

        Assert.False(retrievedUser.IsSuccess);
        Assert.Equal("User not found.", retrievedUser.Error);
        Assert.Null(retrievedUser.Data);
    }

    [Fact]
    public async Task GetUsersShouldReturnMultipleUsers()
    {
        var userService = new UserService(new UserRepository(_dbContext));

        var new_user = new User
        {
            Username = "AnotherUser",
            DisplayName = "Another Display",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _dbContext.Users.Add(new_user);
        await _dbContext.SaveChangesAsync();

        var users = await userService.GetUsers();

        Assert.NotNull(users);
        Assert.True(users.Count() >= 2);
        Assert.Contains(users, u => u.Username == _testUser.Username);
        Assert.Contains(users, u => u.Username == "AnotherUser");
    }

    [Fact]
    public async Task GetUsersWhenNoneExistShouldReturnEmptyList()
    {
        // clear existing users
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();

        var userService = new UserService(new UserRepository(_dbContext));
        var users = await userService.GetUsers();

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task DeleteUserShouldMarkUserAsDeleted()
    {
        var userService = new UserService(new UserRepository(_dbContext));

        var deleteResult = await userService.DeleteUser(_testUser.Id);
        Assert.True(deleteResult.IsSuccess);

        // verify user is marked as deleted (database shouldn't return deleted users)
        var retrievedUser = await userService.GetUserById(_testUser.Id);
        Assert.False(retrievedUser.IsSuccess);
        Assert.Null(retrievedUser.Data);
        Assert.Equal("User not found.", retrievedUser.Error);
    }

    [Fact]
    public async Task DeleteNonExistentUserShouldFail()
    {
        var userService = new UserService(new UserRepository(_dbContext));
        var deleteResult = await userService.DeleteUser(999);

        Assert.False(deleteResult.IsSuccess);
        Assert.Equal("User not found.", deleteResult.Error);
    }
}

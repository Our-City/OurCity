using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Identity;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IUserService
{
    Task<Result<UserResponseDto>> GetUserById(Guid id);
    Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> UpdateUser(Guid id, UserUpdateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> DeleteUser(Guid id);
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserResponseDto>> GetUserById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return Result<UserResponseDto>.Failure("User not found.");

        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userCreateRequestDto)
    {
        //check if the user already exists
        var existentUser = await _userManager.FindByNameAsync(userCreateRequestDto.Username);
        if (existentUser is not null)
            return Result<UserResponseDto>.Failure("User already exists");

        var newUser = new User { UserName = userCreateRequestDto.Username };

        var createResult = await _userManager.CreateAsync(newUser, userCreateRequestDto.Password);

        if (!createResult.Succeeded)
            return Result<UserResponseDto>.Failure(
                string.Join(",", createResult.Errors.Select(e => e.Description))
            );

        return Result<UserResponseDto>.Success(newUser.ToDto());
    }

    public async Task<Result<UserResponseDto>> UpdateUser(
        Guid id,
        UserUpdateRequestDto userUpdateRequestDto
    )
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return Result<UserResponseDto>.Failure("User not found.");

        var updateResult = await _userManager.SetUserNameAsync(user, userUpdateRequestDto.Username);

        if (!updateResult.Succeeded)
            return Result<UserResponseDto>.Failure(
                string.Join(",", updateResult.Errors.Select(e => e.Description))
            );

        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<UserResponseDto>> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return Result<UserResponseDto>.Failure("User not found.");

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
            return Result<UserResponseDto>.Failure(
                string.Join(",", deleteResult.Errors.Select(e => e.Description))
            );

        return Result<UserResponseDto>.Success(user.ToDto());
    }
}

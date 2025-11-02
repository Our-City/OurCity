using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsers();
    Task<Result<UserResponseDto>> GetUserById(int id);
    Task<Result<UserResponseDto>> GetUserByUsername(string username);
    Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> UpdateUser(int id, UserUpdateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsers()
    {
        return (await _userRepository.GetAllUsers()).ToDtos();
    }

    public async Task<Result<UserResponseDto>> GetUserById(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            return Result<UserResponseDto>.Failure("User not found.");
        }
        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<UserResponseDto>> GetUserByUsername(string username)
    {
        var user = await _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            return Result<UserResponseDto>.Failure("User not found.");
        }
        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userCreateRequestDto)
    {
        // check if the username already exists in db
        var existingUser = await _userRepository.GetUserByUsername(userCreateRequestDto.Username);
        if (existingUser != null)
        {
            return Result<UserResponseDto>.Failure("Username already exists.");
        }

        var createdUser = await _userRepository.CreateUser(
            userCreateRequestDto.CreateDtoToEntity()
        );

        return Result<UserResponseDto>.Success(createdUser.ToDto());
    }

    public async Task<Result<UserResponseDto>> UpdateUser(
        int id,
        UserUpdateRequestDto userUpdateRequestDto
    )
    {
        var existingUser = await _userRepository.GetUserById(id);

        if (existingUser == null)
        {
            return Result<UserResponseDto>.Failure("User not found.");
        }

        var updatedUser = await _userRepository.UpdateUser(
            userUpdateRequestDto.UpdateDtoToEntity(existingUser)
        );
        return Result<UserResponseDto>.Success(updatedUser.ToDto());
    }

    public async Task<Result<UserResponseDto>> DeleteUser(int id)
    {
        var existingUser = await _userRepository.GetUserById(id);

        if (existingUser == null)
        {
            return Result<UserResponseDto>.Failure("User not found.");
        }

        await _userRepository.DeleteUser(existingUser);
        return Result<UserResponseDto>.Success(existingUser.ToDto());
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Authorization;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IUserService
{
    Task<Result<UserResponseDto>> GetUserById(Guid id);
    Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> UpdateUser(Guid id, UserUpdateRequestDto userRequestDto);
    Task<Result<bool>> ReportUser(Guid id, UserReportRequestDto userReportRequestDto);
    Task<Result<UserResponseDto>> DeleteUser(Guid id);
}

public class UserService : IUserService
{
    private readonly ICurrentUser _requestingUser;
    private readonly IPolicyService _policyService;
    private readonly UserManager<User> _userManager;
    private readonly IReportRepository _reportRepository;
    
    public UserService(
        ICurrentUser requestingUser,
        IPolicyService policyService,
        IReportRepository reportRepository,
        UserManager<User> userManager
    )
    {
        _requestingUser = requestingUser;
        _policyService = policyService;
        _reportRepository = reportRepository;
        _userManager = userManager;
    }

    public async Task<Result<UserResponseDto>> GetUserById(Guid id)
    {
        var user = await _userManager
            .Users.Include(u => u.Posts)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userCreateRequestDto)
    {
        //check if the user already exists
        var existentUser = await _userManager.FindByNameAsync(userCreateRequestDto.Username);
        if (existentUser is not null)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserAlreadyExists);

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
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        var updateResult = await _userManager.SetUserNameAsync(user, userUpdateRequestDto.Username);

        if (!updateResult.Succeeded)
            return Result<UserResponseDto>.Failure(
                string.Join(",", updateResult.Errors.Select(e => e.Description))
            );

        return Result<UserResponseDto>.Success(user.ToDto());
    }

    public async Task<Result<bool>> ReportUser(Guid id, UserReportRequestDto userReportRequestDto)
    {
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanParticipateInForum())
        {
            return Result<bool>.Failure(
                ErrorMessages.Unauthorized
            );
        }

        if (_requestingUser.UserId.Value == id)
        {
            return Result<bool>.Failure(
                ErrorMessages.CantReportSelf
            );
        }

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return Result<bool>.Failure(ErrorMessages.UserNotFound);

        var existingReport = await _reportRepository.GetReportById(id);
        if (existingReport != null)
        {
            await _reportRepository.Remove(existingReport);
        }
        else
        {
            await _reportRepository.Add(
                new Report
                {
                    Id = Guid.NewGuid(),
                    TargetId = id,
                    ReporterId = _requestingUser.UserId.Value,
                    Reason = userReportRequestDto.Reason,
                    ReportedAt = DateTime.UtcNow,
                }
            );
        }
        
        await _reportRepository.SaveChangesAsync();

        return Result<bool>.Success(true);   
    }

    public async Task<Result<UserResponseDto>> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
            return Result<UserResponseDto>.Failure(
                string.Join(",", deleteResult.Errors.Select(e => e.Description))
            );

        return Result<UserResponseDto>.Success(user.ToDto());
    }
}

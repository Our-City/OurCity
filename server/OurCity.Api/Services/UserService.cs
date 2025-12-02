using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Authorization;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IUserService
{
    Task<Result<PaginatedResponseDto<UserResponseDto>>> GetUsers(UserFilter? userFilter = null);
    Task<Result<UserResponseDto>> GetUserById(Guid id);
    Task<Result<UserResponseDto>> CreateUser(UserCreateRequestDto userRequestDto);
    Task<Result<UserResponseDto>> UpdateUser(Guid id, UserUpdateRequestDto userRequestDto);
    Task<Result<bool>> ReportUser(Guid id, UserReportRequestDto userReportRequestDto);
    Task<Result<UserResponseDto>> BanUser(Guid id);
    Task<Result<UserResponseDto>> UnbanUser(Guid id);
    Task<Result<UserResponseDto>> DeleteUser(Guid id);
    Task<Result<bool>> PromoteUserToAdmin(Guid id);
}

public class UserService : IUserService
{
    private readonly ICurrentUser _requestingUser;
    private readonly IPolicyService _policyService;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserReportRepository _userReportRepository;

    public UserService(
        ICurrentUser requestingUser,
        IPolicyService policyService,
        IUserRepository userRepository,
        IUserReportRepository userReportRepository,
        UserManager<User> userManager
    )
    {
        _requestingUser = requestingUser;
        _policyService = policyService;
        _userRepository = userRepository;
        _userReportRepository = userReportRepository;
        _userManager = userManager;
    }

    public async Task<Result<PaginatedResponseDto<UserResponseDto>>> GetUsers(
        UserFilter? userFilter = null
    )
    {
        //Check if user is allowed to use the filters
        if (
            !await _policyService.CanAdministrateForum()
            && (
                userFilter?.IsBanned is not null
                || userFilter?.MinimumNumReports is not null
                || userFilter?.SortBy == "reportCount"
            )
        )
        {
            return Result<PaginatedResponseDto<UserResponseDto>>.Failure(
                ErrorMessages.Unauthorized
            );
        }

        var limit = userFilter?.Limit ?? 25;

        // Fetch one extra item to determine if there's a next page.
        var userFilterPlusOne = userFilter is not null
            ? userFilter with
            {
                Limit = limit + 1,
            }
            : new UserFilter { Limit = limit + 1 };
        var users = await _userRepository.GetUsers(userFilterPlusOne);

        var hasNextPage = users.Count() > limit;
        var pageItems = users.Take(limit);

        var response = new PaginatedResponseDto<UserResponseDto>
        {
            Items = pageItems.ToDtos(),
            NextCursor = hasNextPage ? users.LastOrDefault()?.Id : null,
        };

        return Result<PaginatedResponseDto<UserResponseDto>>.Success(response);
    }

    public async Task<Result<UserResponseDto>> GetUserById(Guid id)
    {
        var user = await _userManager
            .Users.Include(u => u.Posts)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null || user.IsDeleted)
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

        if (user == null || user.IsDeleted)
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
            return Result<bool>.Failure(ErrorMessages.Unauthorized);
        }

        if (_requestingUser.UserId.Value == id)
        {
            return Result<bool>.Failure(ErrorMessages.CantReportSelf);
        }

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null || user.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.UserNotFound);

        var existingReport = await _userReportRepository.GetReportByReporterAndTargetUserId(
            _requestingUser.UserId.Value,
            id
        );
        if (existingReport != null)
        {
            await _userReportRepository.Remove(existingReport);
        }
        else
        {
            await _userReportRepository.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    TargetUserId = id,
                    ReporterId = _requestingUser.UserId.Value,
                    Reason = userReportRequestDto.Reason,
                    ReportedAt = DateTime.UtcNow,
                }
            );
        }

        await _userReportRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<UserResponseDto>> BanUser(Guid id)
    {
        //Check if user has permissions to ban people
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanAdministrateForum())
            return Result<UserResponseDto>.Failure(ErrorMessages.Unauthorized);

        //Check if user is trying to ban themselves
        if (_requestingUser.UserId.Value == id)
            return Result<UserResponseDto>.Failure(ErrorMessages.CantBanSelf);

        var user = await _userManager.FindByIdAsync(id.ToString());

        //Check if user even exists
        if (user is null || user.IsDeleted)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        //Check if user already banned
        if (user.IsBanned)
            return Result<UserResponseDto>.Success(user.ToDto());

        var userAfterBan = await _userRepository.BanUser(user);

        return Result<UserResponseDto>.Success(userAfterBan.ToDto());
    }

    public async Task<Result<UserResponseDto>> UnbanUser(Guid id)
    {
        //Check if user has permissions to unban people
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanAdministrateForum())
            return Result<UserResponseDto>.Failure(ErrorMessages.Unauthorized);

        //Check if user is trying to unban themselves
        if (_requestingUser.UserId.Value == id)
            return Result<UserResponseDto>.Failure(ErrorMessages.CantUnbanSelf);

        var user = await _userManager.FindByIdAsync(id.ToString());

        //Check if user does not exist
        if (user is null || user.IsDeleted)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        //Check if user already is not banned
        if (!user.IsBanned)
            return Result<UserResponseDto>.Success(user.ToDto());

        var userAfterUnban = await _userRepository.UnbanUser(user);

        return Result<UserResponseDto>.Success(userAfterUnban.ToDto());
    }

    public async Task<Result<UserResponseDto>> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null || user.IsDeleted)
            return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);

        var userAfterDelete = await _userRepository.DeleteUser(user);

        return Result<UserResponseDto>.Success(userAfterDelete.ToDto());
    }

    public async Task<Result<bool>> PromoteUserToAdmin(Guid id)
    {
        if (!await _policyService.CanAdministrateForum())
            return Result<bool>.Failure(ErrorMessages.Unauthorized);

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null || user.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.UserNotFound);

        await _userManager.AddToRoleAsync(user, UserRoles.Admin);

        return Result<bool>.Success(true);
    }
}

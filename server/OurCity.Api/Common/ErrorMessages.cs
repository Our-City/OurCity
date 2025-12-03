namespace OurCity.Api.Common;

public static class ErrorMessages
{
    // General
    public const string ResourceNotFound = "Resource not found";
    public const string Unauthorized = "You are not authorized to perform this action";
    public const string InternalServerError = "An unexpected error occurred";

    // Authentication
    public const string InvalidCredentials = "Invalid username or password";
    public const string UserNotAuthenticated = "User not authenticated";
    public const string TokenExpired = "Your session has expired. Please log in again";

    // Posts
    public const string PostNotFound = "Post not found";
    public const string PostUnauthorized = "You do not have permission to modify this post";

    // Comments
    public const string CommentNotFound = "Comment not found";
    public const string CommentUnauthorized = "You do not have permission to modify this Comment";

    // Posts
    public const string MediaNotFound = "Media attachment not found";
    public const string MediaUnauthorized =
        "You do not have permission to modify this media attachment";

    // Users
    public const string UserNotFound = "User not found";
    public const string UserUnauthorized = "You do not have permission to modify this user";
    public const string UserAlreadyExists = "User with the given ID already exists";
    public const string CantBanSelf = "You cannot ban yourself";
    public const string CantUnbanSelf = "You cannot unban yourself";

    // Tags
    public const string TagNotFound = "Tag not found";

    // Validation
    public const string TitleRequired = "Title is required";
    public const string TitleTooLong = "Title cannot exceed 50 characters";
    public const string DescriptionRequired = "Description is required";
    public const string DescriptionTooLong = "Description cannot exceed 500 characters";

    // Votes
    public const string InvalidVoteType = "Invalid vote type";

    // User Reports
    public const string CantReportSelf = "You cannot report yourself";
}

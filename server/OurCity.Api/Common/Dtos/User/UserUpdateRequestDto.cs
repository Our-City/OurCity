using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.User;

public class UserUpdateRequestDto
{
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public required string Username { get; set; }
}

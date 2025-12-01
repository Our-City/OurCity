using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.User;

public class UserReportRequestDto
{
    [Required(ErrorMessage = "Reason for reporting is required")]
    [StringLength(
        200,
        MinimumLength = 1,
        ErrorMessage = "Reason for reporting must be between 1 and 200 characters"
    )]
    public required string Reason { get; set; }
}

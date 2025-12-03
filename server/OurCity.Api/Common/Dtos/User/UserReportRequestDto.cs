using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.User;

public class UserReportRequestDto
{
    [StringLength(200, ErrorMessage = "Reason for reporting must be less than 200 characters")]
    public string? Reason { get; set; }
}

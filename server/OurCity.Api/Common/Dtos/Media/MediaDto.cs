using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.Media;

public class MediaDto
{
    [Required(ErrorMessage = "Url is required")]
    public required string Url { get; set; }
}

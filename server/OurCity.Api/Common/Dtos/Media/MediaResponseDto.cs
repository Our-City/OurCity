using System;

namespace OurCity.Api.Common.Dtos.Media;

public class MediaResponseDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Post;

public class PostGetAllRequestDto
{
    //pagination
    public Guid? Cursor { get; set; }

    [Range(1, 50)]
    public int Limit { get; set; } = 25;

    //filtering
    public string? SearchTerm { get; set; }
    public IEnumerable<Guid>? Tags { get; set; }

    //sorting
    public string? SortBy { get; set; }
    public SortOrder? SortOrder { get; set; }
}

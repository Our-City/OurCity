using System.ComponentModel.DataAnnotations;
using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common;

public record UserFilter
{
    //pagination
    public Guid? Cursor { get; init; }

    [Range(1, 50)]
    public int Limit { get; init; } = 25;

    //filtering
    public int? MinimumNumReports { get; init; }
    public bool? IsBanned { get; init; }

    //sorting
    public string? SortBy { get; init; }
    public SortOrder? SortOrder { get; init; }
}

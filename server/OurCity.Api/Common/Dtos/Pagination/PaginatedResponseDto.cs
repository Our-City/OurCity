namespace OurCity.Api.Common.Dtos;

public class PaginatedResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public Guid? NextCursor { get; set; }
}
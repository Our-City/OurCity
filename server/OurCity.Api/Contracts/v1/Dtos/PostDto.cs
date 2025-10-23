namespace OurCity.Api.Contracts.v1.Dtos;

public record PostDto
{
    public required int Id { get; init; }
    public required int? AuthorId { get; init; } //null if deleted
    public required string Title { get; init; }
    public required string? Description { get; init; } //null if deleted
    public required int Votes { get; init; }
    public required string? Location { get; init; } //null if no location
    public required bool IsDeleted { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; } //null if never updated
}
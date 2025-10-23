namespace OurCity.Api.Contracts.v1.Dtos;

public record PostCommentDto
{
    public required int Id { get; init; }
    public required int PostId { get; init; }
    public required int? AuthorId { get; init; } //null if comment deleted
    public required int? ParentCommentId { get; init; } //null if root comment
    public required string? Content { get; init; } //null if comment deleted
    public required int Votes { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; } //null if never updated
}
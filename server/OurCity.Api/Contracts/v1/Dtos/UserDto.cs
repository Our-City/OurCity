namespace OurCity.Api.Contracts.v1.Dtos;

public record UserDto
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string? DisplayName { get; init; }
    public required bool IsDeleted { get; init; }
}
namespace OurCity.Api.Contracts.v1.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string? DisplayName { get; set; }
    public required bool IsDeleted { get; set; }
}
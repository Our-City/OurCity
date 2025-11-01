namespace OurCity.Api.Common.Dtos.Comments;

public class CommentListResponseDto
{
    public required List<CommentResponseDto> Comments { get; set; }
    //pagination stuff here later
}

using Microsoft.EntityFrameworkCore;

public class GetTaskCommentsQuery : IRequest<List<TaskCommentDto>>
{
    public int TaskId { get; set; }
}

public class GetTaskCommentsQueryHandler : IRequestHandler<GetTaskCommentsQuery, List<TaskCommentDto>>
{
    private readonly IRepository<TaskComment> _commentRepository;
    public GetTaskCommentsQueryHandler(IRepository<TaskComment> commentRepository) => _commentRepository = commentRepository;

    public Task<List<TaskCommentDto>> Handle(GetTaskCommentsQuery request, CancellationToken ct) =>
        _commentRepository.ReadWhere(c => c.TaskId == request.TaskId && !c.IsDeleted)
            .OrderBy(c => c.CreatedDate)
            .Select(c => new TaskCommentDto(c.CommentId, c.TaskId, c.UserId, c.User.FirstName + " " + c.User.LastName, c.CommentText, c.CreatedDate, c.UpdatedDate))
            .ToListAsync(ct);
}

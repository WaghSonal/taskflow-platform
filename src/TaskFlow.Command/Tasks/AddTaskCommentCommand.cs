public class AddTaskCommentCommand : IRequest<int>
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}

public class AddTaskCommentCommandHandler : IRequestHandler<AddTaskCommentCommand, int>
{
    private readonly IRepository<TaskComment> _commentRepository;
    public AddTaskCommentCommandHandler(IRepository<TaskComment> commentRepository) => _commentRepository = commentRepository;

    public async Task<int> Handle(AddTaskCommentCommand request, CancellationToken ct)
    {
        var comment = new TaskComment { TaskId = request.TaskId, UserId = request.UserId, CommentText = request.CommentText };
        await _commentRepository.CreateAsync(comment);
        return comment.CommentId;
    }
}

public class UpdateTaskCommentCommand : IRequest
{
    public int CommentId { get; set; }
    public int RequestingUserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}

public class UpdateTaskCommentCommandHandler : IRequestHandler<UpdateTaskCommentCommand>
{
    private readonly IRepository<TaskComment> _commentRepository;
    public UpdateTaskCommentCommandHandler(IRepository<TaskComment> commentRepository) => _commentRepository = commentRepository;

    public async Task Handle(UpdateTaskCommentCommand request, CancellationToken ct)
    {
        var comment = await _commentRepository.ReadFirstOrDefaultAsync(c => c.CommentId == request.CommentId && !c.IsDeleted)
            ?? throw new NotFoundException($"Comment {request.CommentId} not found.");

        // "Edit own comment" is enforced here, in the handler, not as a route attribute —
        // the rule depends on data (whose comment is this), not on the caller's role.
        if (comment.UserId != request.RequestingUserId)
            throw new UnauthorizedException("You can only edit your own comments.");

        comment.CommentText = request.CommentText;
        comment.UpdatedDate = DateTime.UtcNow;
        await _commentRepository.UpdateAsync(comment);
    }
}

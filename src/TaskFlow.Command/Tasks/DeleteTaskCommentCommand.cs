public class DeleteTaskCommentCommand : IRequest
{
    public int CommentId { get; set; }
    public int RequestingUserId { get; set; }
}

public class DeleteTaskCommentCommandHandler : IRequestHandler<DeleteTaskCommentCommand>
{
    private readonly IRepository<TaskComment> _commentRepository;
    public DeleteTaskCommentCommandHandler(IRepository<TaskComment> commentRepository) => _commentRepository = commentRepository;

    public async Task Handle(DeleteTaskCommentCommand request, CancellationToken ct)
    {
        var comment = await _commentRepository.ReadFirstOrDefaultAsync(c => c.CommentId == request.CommentId && !c.IsDeleted)
            ?? throw new NotFoundException($"Comment {request.CommentId} not found.");

        if (comment.UserId != request.RequestingUserId)
            throw new UnauthorizedException("You can only delete your own comments.");

        // Soft delete — matches the IsDeleted column already on TaskComments.
        comment.IsDeleted = true;
        comment.UpdatedDate = DateTime.UtcNow;
        await _commentRepository.UpdateAsync(comment);
    }
}

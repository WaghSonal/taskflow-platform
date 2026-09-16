public class DeleteTaskAttachmentCommand : IRequest
{
    public int AttachmentId { get; set; }
}

public class DeleteTaskAttachmentCommandHandler : IRequestHandler<DeleteTaskAttachmentCommand>
{
    private readonly IRepository<TaskAttachment> _attachmentRepository;
    public DeleteTaskAttachmentCommandHandler(IRepository<TaskAttachment> attachmentRepository) => _attachmentRepository = attachmentRepository;

    public async Task Handle(DeleteTaskAttachmentCommand request, CancellationToken ct)
    {
        var attachment = await _attachmentRepository.ReadFirstOrDefaultAsync(a => a.AttachmentId == request.AttachmentId)
            ?? throw new NotFoundException($"Attachment {request.AttachmentId} not found.");

        await _attachmentRepository.DeleteAsync(attachment);
    }
}

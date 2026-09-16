// Metadata only — matches the frontend's "no backend file storage yet" (Attachments UI is mock-only).
public class AddTaskAttachmentCommand : IRequest<int>
{
    public int TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public int UploadedBy { get; set; }
}

public class AddTaskAttachmentCommandHandler : IRequestHandler<AddTaskAttachmentCommand, int>
{
    private readonly IRepository<TaskAttachment> _attachmentRepository;
    public AddTaskAttachmentCommandHandler(IRepository<TaskAttachment> attachmentRepository) => _attachmentRepository = attachmentRepository;

    public async Task<int> Handle(AddTaskAttachmentCommand request, CancellationToken ct)
    {
        var attachment = new TaskAttachment
        {
            TaskId = request.TaskId,
            FileName = request.FileName,
            FilePath = request.FilePath,
            FileSize = request.FileSize,
            ContentType = request.ContentType,
            UploadedBy = request.UploadedBy,
        };
        await _attachmentRepository.CreateAsync(attachment);
        return attachment.AttachmentId;
    }
}

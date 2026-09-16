using Microsoft.EntityFrameworkCore;

public class GetTaskAttachmentsQuery : IRequest<List<TaskAttachmentDto>>
{
    public int TaskId { get; set; }
}

public class GetTaskAttachmentsQueryHandler : IRequestHandler<GetTaskAttachmentsQuery, List<TaskAttachmentDto>>
{
    private readonly IRepository<TaskAttachment> _attachmentRepository;
    public GetTaskAttachmentsQueryHandler(IRepository<TaskAttachment> attachmentRepository) => _attachmentRepository = attachmentRepository;

    public Task<List<TaskAttachmentDto>> Handle(GetTaskAttachmentsQuery request, CancellationToken ct) =>
        _attachmentRepository.ReadWhere(a => a.TaskId == request.TaskId)
            .OrderByDescending(a => a.UploadedDate)
            .Select(a => new TaskAttachmentDto(a.AttachmentId, a.TaskId, a.FileName, a.FilePath, a.FileSize, a.ContentType, a.UploadedBy, a.Uploader.FirstName + " " + a.Uploader.LastName, a.UploadedDate))
            .ToListAsync(ct);
}

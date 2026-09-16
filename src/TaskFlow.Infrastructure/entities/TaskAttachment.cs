public class TaskAttachment
{
    public int AttachmentId { get; set; }
    public int TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public User Uploader { get; set; } = null!;
}

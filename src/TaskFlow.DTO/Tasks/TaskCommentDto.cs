public record TaskCommentDto(int CommentId, int TaskId, int UserId, string UserName, string CommentText, DateTime CreatedDate, DateTime? UpdatedDate);

public record TaskAttachmentDto(int AttachmentId, int TaskId, string FileName, string FilePath, long? FileSize, string? ContentType, int UploadedBy, string UploadedByName, DateTime UploadedDate);

public class ProjectMember
{
    public int ProjectMemberId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public DateOnly JoinedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}

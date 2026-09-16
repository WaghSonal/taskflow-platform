// Named TaskItem, not Task — "Task" would collide with System.Threading.Tasks.Task, which is
// implicitly in scope everywhere via ImplicitUsings.
public class TaskItem
{
    public int TaskId { get; set; }
    public int ProjectId { get; set; }
    public int? ParentTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssignedTo { get; set; }
    public int CreatedBy { get; set; }
    public int StatusId { get; set; }
    public int PriorityId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal ActualHours { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public Project Project { get; set; } = null!;
    public TaskItem? ParentTask { get; set; }
    public User? Assignee { get; set; }
    public User Creator { get; set; } = null!;
    public TaskStatusLookup Status { get; set; } = null!;
    public TaskPriorityLookup Priority { get; set; } = null!;
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}

// Named TaskStatusLookup, not TaskStatus — "TaskStatus" would collide with the
// System.Threading.Tasks.TaskStatus enum, implicitly in scope via ImplicitUsings.
public class TaskStatusLookup
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TaskDependency
{
    public int TaskDependencyId { get; set; }
    public int TaskId { get; set; }
    public int DependsOnTaskId { get; set; }
    public string DependencyType { get; set; } = "Blocks";
}

// TaskFlow.Utility/TaskStatusIds.cs
//
// WHAT: The fixed TaskStatuses.StatusId values seeded in the real database (see
//       "Task Management Queries/SQLQuery9.sql"). Lets handlers compare against a named constant
//       instead of a bare magic number like `== 7`.
// WHERE USED: UpdateTaskStatusCommandHandler sets TaskItem.CompletedDate when the new status is Completed.
public static class TaskStatusIds
{
    public const int Backlog = 1;
    public const int ToDo = 2;
    public const int InProgress = 3;
    public const int Blocked = 4;
    public const int CodeReview = 5;
    public const int Testing = 6;
    public const int Completed = 7;
    public const int Cancelled = 8;
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    public TasksController(ITaskService taskService) => _taskService = taskService;

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTasksQuery query)
        => Ok(await _taskService.GetTasks(query));

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses() => Ok(await _taskService.GetStatuses());

    [HttpGet("priorities")]
    public async Task<IActionResult> GetPriorities() => Ok(await _taskService.GetPriorities());

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags() => Ok(await _taskService.GetTags());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _taskService.GetTaskById(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command)
        => Ok(await _taskService.CreateTask(command));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskCommand command)
    {
        command.TaskId = id;
        await _taskService.UpdateTask(command);
        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusId)
    {
        await _taskService.UpdateTaskStatus(id, statusId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteTask(id);
        return NoContent();
    }

    [HttpPost("{id:int}/tags/{tagId:int}")]
    public async Task<IActionResult> AddTag(int id, int tagId)
    {
        await _taskService.AddTag(id, tagId);
        return NoContent();
    }

    [HttpDelete("{id:int}/tags/{tagId:int}")]
    public async Task<IActionResult> RemoveTag(int id, int tagId)
    {
        await _taskService.RemoveTag(id, tagId);
        return NoContent();
    }

    [HttpGet("{id:int}/dependencies")]
    public async Task<IActionResult> GetDependencies(int id) => Ok(await _taskService.GetDependencies(id));

    [HttpPost("{id:int}/dependencies")]
    public async Task<IActionResult> AddDependency(int id, AddTaskDependencyCommand command)
    {
        command.TaskId = id;
        return Ok(await _taskService.AddDependency(command));
    }

    [HttpDelete("dependencies/{dependencyId:int}")]
    public async Task<IActionResult> RemoveDependency(int dependencyId)
    {
        await _taskService.RemoveDependency(dependencyId);
        return NoContent();
    }

    [HttpGet("{id:int}/comments")]
    public async Task<IActionResult> GetComments(int id) => Ok(await _taskService.GetComments(id));

    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] string commentText)
        => Ok(await _taskService.AddComment(new AddTaskCommentCommand { TaskId = id, UserId = CurrentUserId(), CommentText = commentText }));

    [HttpPut("comments/{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] string commentText)
    {
        await _taskService.UpdateComment(new UpdateTaskCommentCommand { CommentId = commentId, RequestingUserId = CurrentUserId(), CommentText = commentText });
        return NoContent();
    }

    [HttpDelete("comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        await _taskService.DeleteComment(commentId, CurrentUserId());
        return NoContent();
    }

    [HttpGet("{id:int}/attachments")]
    public async Task<IActionResult> GetAttachments(int id) => Ok(await _taskService.GetAttachments(id));

    [HttpPost("{id:int}/attachments")]
    public async Task<IActionResult> AddAttachment(int id, AddTaskAttachmentCommand command)
    {
        command.TaskId = id;
        command.UploadedBy = CurrentUserId();
        return Ok(await _taskService.AddAttachment(command));
    }

    [HttpDelete("attachments/{attachmentId:int}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        await _taskService.DeleteAttachment(attachmentId);
        return NoContent();
    }
}

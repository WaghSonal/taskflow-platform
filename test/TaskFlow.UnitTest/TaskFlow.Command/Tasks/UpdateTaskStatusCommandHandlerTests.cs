using System.Linq.Expressions;
using FluentAssertions;
using Moq;

namespace TaskFlow.UnitTest.TaskFlow.Command.Tasks;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepository = new();

    [Fact]
    public async Task Handle_ExistingTask_UpdatesStatusAndSetsCompletedDateWhenCompleted()
    {
        var task = new TaskItem { TaskId = 1, StatusId = TaskStatusIds.InProgress };
        _taskRepository.Setup(r => r.ReadFirstOrDefaultAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                       .ReturnsAsync(task);

        var handler = new UpdateTaskStatusCommandHandler(_taskRepository.Object);
        await handler.Handle(new UpdateTaskStatusCommand { TaskId = 1, StatusId = TaskStatusIds.Completed }, CancellationToken.None);

        task.StatusId.Should().Be(TaskStatusIds.Completed);
        task.CompletedDate.Should().NotBeNull();
        _taskRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t => t.StatusId == TaskStatusIds.Completed)), Times.Once);
    }

    [Fact]
    public async Task Handle_MovingAwayFromCompleted_ClearsCompletedDate()
    {
        var task = new TaskItem { TaskId = 1, StatusId = TaskStatusIds.Completed, CompletedDate = DateTime.UtcNow };
        _taskRepository.Setup(r => r.ReadFirstOrDefaultAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                       .ReturnsAsync(task);

        var handler = new UpdateTaskStatusCommandHandler(_taskRepository.Object);
        await handler.Handle(new UpdateTaskStatusCommand { TaskId = 1, StatusId = TaskStatusIds.InProgress }, CancellationToken.None);

        task.CompletedDate.Should().BeNull();
    }

    [Fact]
    public async Task Handle_UnknownTaskId_ThrowsNotFoundException()
    {
        _taskRepository.Setup(r => r.ReadFirstOrDefaultAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                       .ReturnsAsync((TaskItem?)null);

        var handler = new UpdateTaskStatusCommandHandler(_taskRepository.Object);
        var act = () => handler.Handle(new UpdateTaskStatusCommand { TaskId = 999 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(x => x.TaskId);
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ActualHours).HasColumnType("decimal(10,2)");
        builder.Property(x => x.EstimatedHours).HasColumnType("decimal(10,2)");

        builder.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ParentTask).WithMany().HasForeignKey(x => x.ParentTaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Assignee).WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Priority).WithMany().HasForeignKey(x => x.PriorityId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TaskStatusLookupConfiguration : IEntityTypeConfiguration<TaskStatusLookup>
{
    public void Configure(EntityTypeBuilder<TaskStatusLookup> builder)
    {
        builder.ToTable("TaskStatuses");
        builder.HasKey(x => x.StatusId);
    }
}

public class TaskPriorityLookupConfiguration : IEntityTypeConfiguration<TaskPriorityLookup>
{
    public void Configure(EntityTypeBuilder<TaskPriorityLookup> builder)
    {
        builder.ToTable("TaskPriorities");
        builder.HasKey(x => x.PriorityId);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasKey(x => x.TagId);
        builder.HasIndex(x => x.TagName).IsUnique();
    }
}

public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
{
    public void Configure(EntityTypeBuilder<TaskTag> builder)
    {
        builder.ToTable("TaskTags");
        builder.HasKey(x => new { x.TaskId, x.TagId });
        builder.HasOne(x => x.Task).WithMany(x => x.TaskTags).HasForeignKey(x => x.TaskId);
        builder.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");
        builder.HasKey(x => x.TaskDependencyId);
        builder.Property(x => x.DependencyType).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.TaskId, x.DependsOnTaskId }).IsUnique();
    }
}

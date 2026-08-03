using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Models;

namespace ToDoApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            // SQL Server collation makes uniqueness checks case-insensitive.
            entity.Property(x => x.Email).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Role).HasDefaultValue(RoleNames.Member);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            // One user owns many lists; deleting a user is restricted to avoid accidental data loss.
            entity.HasMany(x => x.TaskLists)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskList>(entity =>
        {
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            // One list contains many tasks; deleting a list deletes its child tasks.
            entity.HasMany(x => x.TaskItems)
                .WithOne(x => x.TaskList)
                .HasForeignKey(x => x.TaskListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(x => x.IsCompleted).HasDefaultValue(false);

            // TaskTag is the explicit join entity between TaskItem and Tag.
            entity.HasMany(x => x.TaskTags)
                .WithOne(x => x.TaskItem)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(x => x.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.HasIndex(x => x.Name).IsUnique();

            entity.HasMany(x => x.TaskTags)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.Property(x => x.TaggedAt).HasDefaultValueSql("GETDATE()");

            // The same tag can be attached to many tasks, but only once per specific task.
            entity.HasIndex(x => new { x.TaskItemId, x.TagId }).IsUnique();
        });
    }
}

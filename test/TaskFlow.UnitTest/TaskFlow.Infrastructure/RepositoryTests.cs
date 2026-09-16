// Runs against a throwaway in-memory SQLite database (schema generated from the EF model via
// EnsureCreated), not the real TaskManagementDB on .\SQLEXPRESS — this test never touches real
// data, and still exercises the same unique-index rule UserConfiguration declares.
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.UnitTest.TaskFlow.Infrastructure;

public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TaskFlowDbContext _context;

    public RepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open(); // keep the connection open for the test's lifetime, or SQLite drops the in-memory db

        var options = new DbContextOptionsBuilder<TaskFlowDbContext>().UseSqlite(_connection).Options;
        _context = new TaskFlowDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ViolatesUniqueIndex()
    {
        var repository = new Repository<User>(_context);
        await repository.CreateAsync(new User { Email = "dup@taskflow.com", FirstName = "A", LastName = "A", PasswordHash = "x" });

        var act = () => repository.CreateAsync(new User { Email = "dup@taskflow.com", FirstName = "B", LastName = "B", PasswordHash = "y" });

        await act.Should().ThrowAsync<DbUpdateException>(); // the unique index from UserConfiguration, exercised against a real engine
    }

    [Fact]
    public async Task ReadFirstOrDefaultAsync_NoMatch_ReturnsNull()
    {
        var repository = new Repository<User>(_context);
        var result = await repository.ReadFirstOrDefaultAsync(u => u.Email == "nobody@taskflow.com");

        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

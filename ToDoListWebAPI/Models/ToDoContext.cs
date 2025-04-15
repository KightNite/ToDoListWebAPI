using Microsoft.EntityFrameworkCore;

namespace ToDoListWebAPI.Models;

public class ToDoContext : DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoList>()
            .HasMany(t => t.ToDoItems)
            .WithOne(t => t.ToDoList);
    }

    public DbSet<ToDoItem> TodoItems { get; set; } = null!;
    public DbSet<ToDoList> TodoLists { get; set; } = null!;
}
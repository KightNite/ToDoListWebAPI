using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoListWebAPI.Controllers;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Services;
using Xunit.Abstractions;

namespace ToDoListWebAPITests;

public class UnitTestToDoListController
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ToDoListController _toDoListController;
    private readonly ToDoContext _context;
    
    public UnitTestToDoListController(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // Set up mock InMemory database.
        var optionsBuilder = new DbContextOptionsBuilder<ToDoContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new ToDoContext(optionsBuilder.Options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = logFactory.CreateLogger<ToDoListController>();

        // Set up SUT.
        _toDoListController = new ToDoListController(new ToDoListRepository(_context), logger);
    }
    
    private async Task SeedDataAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _context.TodoLists.Add(new ToDoList
            {
                Title = "Title " + i
            });    
        }

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task Test_GetLists_Returns_All_Lists()
    {
        await SeedDataAsync(5);
        var items = await _toDoListController.GetTodoLists();
        
        Assert.Equal(5, items.Count());
    }
    
    [Fact]
    public async Task Test_GetListById_Returns_List()
    {
        string title = "Test Title";
        var data = _context.TodoLists.Add(new ToDoList
        {
            Title = title
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;
        
        _testOutputHelper.WriteLine($"Created id: {data.Id}");
        
        var response = await _toDoListController.GetToDoList(data.Id);
        
        Assert.NotNull(response.Value);
        Assert.Equal(data.Id, response.Value.Id);
        Assert.Equal(title, response.Value.Title);
    }
    
    [Fact]
    public async Task Test_GetListById_Return_NotFound()
    {
        var response = await _toDoListController.GetToDoList(-1);
        
        Assert.Null(response.Value);
        Assert.IsType<NotFoundResult>(response.Result);
    }
    
    [Fact]
    public async Task Test_PostList_Creates_List()
    {
        var data = new CreateToDoListDto("Title");
        
        var postData = await _toDoListController.PostToDoList(data);
        
        
        Assert.IsType<CreatedAtActionResult>(postData.Result);
        ToDoListDto resultDate = (ToDoListDto) ((CreatedAtActionResult)postData.Result).Value!;
        var response = await _toDoListController.GetToDoList(resultDate.Id);
        Assert.NotNull(response.Value);
        Assert.Equal(data.Title, response.Value.Title);
    }
    
    [Fact]
    public async Task Test_PutList_Updates_List()
    {
        string title = "Test Title";
        var data = _context.TodoLists.Add(new ToDoList
        {
            Title = title
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;
        
        string newTitle = "New Title";
        await _toDoListController.PutToDoList(
            data.Id,
            new UpdateToDoListDto(data.Id, newTitle)
        );
        
        var response = await _toDoListController.GetToDoList(data.Id);

        Assert.NotNull(response.Value);
        Assert.Equal(newTitle, response.Value.Title);
    }
    
    [Fact]
    public async Task Test_DeleteList_Deletes_List()
    {
        var data = _context.TodoLists.Add(new ToDoList
        {
            Title = "Test Title"
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;

        var responseDelete = await _toDoListController.DeleteToDoList(data.Id);
        var responseGet = await _toDoListController.GetToDoList(data.Id);

        Assert.IsType<NoContentResult>(responseDelete);
        Assert.IsType<NotFoundResult>(responseGet.Result);
    }
}
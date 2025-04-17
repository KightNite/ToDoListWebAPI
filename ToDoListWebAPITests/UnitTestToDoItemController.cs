using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoListWebAPI.Controllers;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Services;
using Xunit.Abstractions;

namespace ToDoListWebAPITests;

public class UnitTestToDoItemController
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ToDoItemController _toDoItemController;
    private readonly ToDoContext _context;
    
    public UnitTestToDoItemController(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // Set up mock InMemory database.
        var optionsBuilder = new DbContextOptionsBuilder<ToDoContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new ToDoContext(optionsBuilder.Options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = logFactory.CreateLogger<ToDoItemController>();

        // Set up SUT.
        _toDoItemController = new ToDoItemController(new ToDoItemRepository(_context), logger);
    }

    private async Task SeedDataAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _context.TodoItems.Add(new ToDoItem
            {
                Title = "Title " + i
            });    
        }

        await _context.SaveChangesAsync();
    }
    
    [Fact]
    public async Task Test_GetItems_Returns_All_Items()
    {
        await SeedDataAsync(5);
        var items = await _toDoItemController.GetTodoItems();
        
        Assert.Equal(5, items.Count());
    }

    [Fact]
    public async Task Test_GetItemById_Returns_Item()
    {
        string title = "Test Title";
        var data = _context.TodoItems.Add(new ToDoItem
        {
            Title = title
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;
        
        _testOutputHelper.WriteLine($"Created id: {data.Id}");
        
        var response = await _toDoItemController.GetToDoItem(data.Id);
        
        Assert.NotNull(response.Value);
        Assert.Equal(data.Id, response.Value.Id);
        Assert.Equal(title, response.Value.Title);
    }

    [Fact]
    public async Task Test_GetItemById_Returns_NotFound()
    {
        var response = await _toDoItemController.GetToDoItem(-1);
        
        Assert.Null(response.Value);
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Test_GetItemsHistory_Returns_Sorted_Items()
    {
        var data = new List<ToDoItem>
        {
            new() { Title = "1", IsDone = false },
            new() { Title = "2", IsDone = false },
            new() { Title = "3", IsDone = false }
        };
        foreach (var item in data)
        {
            int id = _context.Add(item).Entity.Id;
            item.Id = id;
        }
        await _context.SaveChangesAsync();

        data.Reverse();
        foreach (var item in data)
        {
            await _toDoItemController.MarkDoneToDoItem(item.Id);
        }
        
        var response = await _toDoItemController.GetTodoItemsHistory();
        var toDoItemDtos = response.ToList();
        Assert.Equal(data.Count, toDoItemDtos.Count);
        Assert.Equal("3", toDoItemDtos[0].Title);
        Assert.Equal("2", toDoItemDtos[1].Title);
        Assert.Equal("1", toDoItemDtos[2].Title);
    }
    
    [Fact]
    public async Task Test_PutItem_Updates_Item()
    {
        string title = "Test Title";
        var data = _context.TodoItems.Add(new ToDoItem
        {
            Title = title
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;
        
        string newTitle = "New Title";
        await _toDoItemController.PutToDoItem(
            data.Id,
            new UpdateToDoItemDto(
                data.Id,
                newTitle,
                null,
                true,
                null)
            );
        
        var response = await _toDoItemController.GetToDoItem(data.Id);

        Assert.NotNull(response.Value);
        Assert.Equal(newTitle, response.Value.Title);
        Assert.True(data.IsDone != response.Value.IsDone);
    }
    
    [Fact]
    public async Task Test_PutItemInvalidListId_Returns_BadRequest()
    {
        var data = _context.TodoItems.Add(new ToDoItem
        {
            Title = "Test Title"
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;
        
        var response = await _toDoItemController.PutToDoItem(
            data.Id,
            new UpdateToDoItemDto(
                data.Id,
                data.Title,
                null,
                true,
                -1)
        );

        Assert.IsType<BadRequestObjectResult>(response.Result);
    }
    
    [Fact]
    public async Task Test_PostItem_Creates_Item()
    {
        var data = new CreateToDoItemDto("Title", null, null);
        
        var postData = await _toDoItemController.PostToDoItem(data);
        
        
        Assert.IsType<CreatedAtActionResult>(postData.Result);
        ToDoItemDto resultDate = (ToDoItemDto) ((CreatedAtActionResult)postData.Result).Value!;
        var response = await _toDoItemController.GetToDoItem(resultDate.Id);
        Assert.NotNull(response.Value);
        Assert.Equal(data.Title, response.Value.Title);
    }
    
    [Fact]
    public async Task Test_PostItemInvalidListId_Returns_BadRequest()
    {
        var data = new CreateToDoItemDto("Title", null, -1);
        
        var response = await _toDoItemController.PostToDoItem(data);

        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task Test_PostMarkDoneItem_Updates_Item()
    {
        var data = _context.TodoItems.Add(new ToDoItem
        {
            Title = "Test Title",
            IsDone = false,
            DoneDate = null
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;

        var response = await _toDoItemController.MarkDoneToDoItem(data.Id);
        Assert.IsType<NoContentResult>(response.Result);
        
        response = await _toDoItemController.GetToDoItem(data.Id);
        Assert.NotNull(response.Value);
        Assert.NotEqual(data.IsDone, response.Value.IsDone);
    }

    [Fact]
    public async Task Test_DeleteItem_Deletes_Item()
    {
        var data = _context.TodoItems.Add(new ToDoItem
        {
            Title = "Test Title",
            IsDone = false,
            DoneDate = null
        }).Entity;
        await _context.SaveChangesAsync();
        _context.Entry(data).State = EntityState.Detached;

        var responseDelete = await _toDoItemController.DeleteToDoItem(data.Id);
        var responseGet = await _toDoItemController.GetToDoItem(data.Id);

        Assert.IsType<NoContentResult>(responseDelete);
        Assert.IsType<NotFoundResult>(responseGet.Result);
    }
}
using Microsoft.AspNetCore.Mvc;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDoItemController : ControllerBase
{
    private readonly IToDoItemRepository _toDoItemRepository;
    private readonly ILogger<ToDoItemController> _logger;

    public ToDoItemController(IToDoItemRepository toDoItemRepository, ILogger<ToDoItemController> logger)
    {
        _toDoItemRepository = toDoItemRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all ToDoItems.
    /// </summary>
    /// <returns>ToDoItems.</returns>
    /// <response code="200">Returned ToDoItems.</response>
    // GET: api/ToDoItem
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<ToDoItemDto>> GetTodoItems()
    {
        return await _toDoItemRepository.GetAllAsync();
    }
        
    /// <summary>
    /// Gets the ToDoItems marked as Done and sorted by completion time.
    /// </summary>
    /// <returns>ToDoItems sorted by completion time.</returns>
    /// <response code="200">Returned ToDoItems.</response>
    // GET: api/ToDoItem/history
    [HttpGet("history")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<ToDoItemDto>> GetTodoItemsHistory()
    {
        return await _toDoItemRepository.GetHistoryAsync();
    }

    /// <summary>
    /// Gets the specified ToDoItem.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>ToDoItem with specified id.</returns>
    /// <response code="200">ToDoItem was found.</response>
    /// <response code="404">If the ToDoItem with specified id was not found.</response>
    // GET: api/ToDoItem/5
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemDto>> GetToDoItem(int id)
    {
        var toDoItem = await _toDoItemRepository.FirstOrDefaultAsync(id);

        if (toDoItem == null)
        {
            return NotFound();
        }

        return toDoItem;
    }

    /// <summary>
    /// Updates the specified ToDoItem.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="toDoItemDto"></param>
    /// <returns>The updated ToDoItem.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT 
    ///     {
    ///        "id": 1,
    ///        "title": "Item #1",
    ///        "description": "Item description",
    ///        "isDone": false,
    ///        "toDoListId": null
    ///     }
    ///
    /// </remarks>
    /// <response code="200">ToDoItem was successfully updated.</response>
    /// <response code="400">If the ToDoList with specified id was not found.</response>
    /// <response code="404">If the ToDoItem with specified id was not found.</response>
    // PUT: api/ToDoItem/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemDto>> PutToDoItem(int id, UpdateToDoItemDto toDoItemDto)
    {
        if (id != toDoItemDto.Id)
        {
            return BadRequest();
        }

        ToDoItemDto? toDoItem;

        try
        {
            toDoItem = await _toDoItemRepository.UpdateAsync(new ToDoItemDto(
                toDoItemDto.Id,
                toDoItemDto.Title,
                toDoItemDto.Description,
                toDoItemDto.IsDone,
                null,
                toDoItemDto.ToDoListId));
        }
        catch (NullReferenceException e)
        {
            _logger.LogWarning("Unable create ToDoItem for a ToDoList with id {id}.", toDoItemDto.ToDoListId);
            return BadRequest(e.Message);
        }

        if (toDoItem == null)
        {
            return NotFound();
        }
            
        _logger.LogInformation("Updated ToDoItem with id {id}.", id);

        return toDoItem;
    }
        
    /// <summary>
    /// Marks specified ToDoItem as done.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">If ToDoItem was marked as done.</response>
    /// <response code="404">If the ToDoItem with specified id was not found.</response>
    // PUT: api/ToDoItem/markDone/5
    [HttpPut("markDone/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemDto>> MarkDoneToDoItem(int id)
    {
        var toDoItem = await _toDoItemRepository.MarkDoneAsync(id);
        if (toDoItem == null)
        {
            return NotFound();
        }

        _logger.LogInformation("Changed ToDoItem with id {id} state to Done.", toDoItem.Id);
        return NoContent();
    }

    /// <summary>
    /// Creates a TodoItem.
    /// </summary>
    /// <param name="toDoItemDto"></param>
    /// <returns>A newly created TodoItem</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST 
    ///     {
    ///        "title": "Item #1",
    ///        "description": "Item description",
    ///        "toDoListId": null
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the Sublist with toDoListId doesn't exist.</response>
    // POST: api/ToDoItem
    [HttpPost]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ToDoItemDto>> PostToDoItem(CreateToDoItemDto toDoItemDto)
    {
        ToDoItemDto toDoItem;
        try
        {
            toDoItem = await _toDoItemRepository.AddAsync(new ToDoItemDto(
                0,
                toDoItemDto.Title,
                toDoItemDto.Description,
                false,
                null,
                toDoItemDto.ToDoListId));
        }
        catch (NullReferenceException e)
        {
            _logger.LogWarning("Unable create ToDoItem for a ToDoList with id {id}.", toDoItemDto.ToDoListId);
            return BadRequest(e.Message);
        }
            
        _logger.LogInformation("Created ToDoItem with id {id}.", toDoItem.Id);
        return CreatedAtAction(
            nameof(GetToDoItem), 
            new { id = toDoItem.Id }, 
            toDoItem);
    }

    /// <summary>
    /// Deletes specified ToDoItem.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">If ToDoItem was successfully deleted.</response>
    /// <response code="404">If no ToDoItem with specified id was found.</response>
    // DELETE: api/ToDoItem/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteToDoItem(int id)
    {
        if (!ToDoItemExists(id))
        {
            return NotFound();
        }
            
        await _toDoItemRepository.RemoveAsync(id);

        _logger.LogInformation("Deleted ToDoItem with id {id}.", id);
        return NoContent();
    }

    private bool ToDoItemExists(int id)
    {
        return _toDoItemRepository.Exists(id);
    }
}
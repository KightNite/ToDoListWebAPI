using Microsoft.AspNetCore.Mvc;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDoListController : ControllerBase
{
    private readonly IToDoListRepository _toDoListRepository;
    private readonly ILogger<ToDoListController> _logger;

    public ToDoListController(IToDoListRepository toDoListRepository, ILogger<ToDoListController> logger)
    {
        _toDoListRepository = toDoListRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets the ToDoLists and their tasks.
    /// </summary>
    /// <returns>ToDoLists.</returns>
    /// <response code="200">Returned ToDoLists.</response>
    // GET: api/ToDoList
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<ToDoListDto>> GetTodoLists()
    {
        return await _toDoListRepository.GetAllAsync();
    }

    /// <summary>
    /// Gets the specified ToDoList and it's tasks.
    /// </summary>
    /// <returns>ToDoLists.</returns>
    /// <response code="200">Returned ToDoLists.</response>
    /// <response code="404">If ToDoLists was not found.</response>
    // GET: api/ToDoList/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoListDto>> GetToDoList(int id)
    {
        var toDoList = await _toDoListRepository.FirstOrDefaultAsync(id);

        if (toDoList == null)
        {
            return NotFound();
        }

        return toDoList;
    }

    /// <summary>
    /// Updates the specified ToDoList.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="toDoListDto"></param>
    /// <returns>The updated ToDoList.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT 
    ///     {
    ///        "id": 1,
    ///        "title": "List #1"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">ToDoItem was successfully updated.</response>
    /// <response code="400">Id mismatch.</response>
    /// <response code="404">If the ToDoList with specified id was not found.</response>
    // PUT: api/ToDoList/5
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutToDoList(int id, UpdateToDoListDto toDoListDto)
    {
        if (id != toDoListDto.Id)
        {
            return BadRequest();
        }

            
        var toDoList = await _toDoListRepository.UpdateAsync(
            new ToDoListDto(
                toDoListDto.Id, 
                toDoListDto.Title, 
                null)
        );

        if (toDoList == null)
        {
            return NotFound();
        }
            
        _logger.LogInformation("Updated ToDoList with id {id}.", id);

        return NoContent();
    }

    /// <summary>
    /// Creates ToDoList.
    /// </summary>
    /// <param name="toDoListDto"></param>
    /// <returns>The created ToDoList.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST 
    ///     {
    ///        "title": "List #1"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">ToDoItem was successfully created.</response>
    // POST: api/ToDoList
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateToDoListDto>> PostToDoList(CreateToDoListDto toDoListDto)
    {
        ToDoListDto toDoList = await _toDoListRepository.AddAsync(
            new ToDoListDto(
                0, 
                toDoListDto.Title, 
                null)
        );

        _logger.LogInformation("Created ToDoList with id {id}.", toDoList.Id);
        return CreatedAtAction(
            nameof(GetToDoList),
            new { id = toDoList.Id }, 
            toDoList);
    }

    /// <summary>
    /// Deletes specified ToDoList.
    /// ToDoItems assigned to this list will be assigned to the default list.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">If ToDoItem was successfully deleted.</response>
    /// <response code="404">If no ToDoItem with specified id was found.</response>
    // DELETE: api/ToDoList/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToDoList(int id)
    {
        if (!ToDoListExists(id))
        {
            return NotFound();
        }
            
        await _toDoListRepository.RemoveAsync(id);
        _logger.LogInformation("Deleted ToDoList with id {id}.", id);

        return NoContent();
    }

    private bool ToDoListExists(int id)
    {
        return _toDoListRepository.Exists(id);
    }
}
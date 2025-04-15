using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemController : ControllerBase
    {
        private readonly IToDoItemRepository _toDoItemRepository;

        public ToDoItemController(IToDoItemRepository toDoItemRepository)
        {
            _toDoItemRepository = toDoItemRepository;
        }

        // GET: api/ToDoItem
        [HttpGet]
        [Produces("application/json")]
        public async Task<IEnumerable<ToDoItemDto>> GetTodoItems()
        {
            return await _toDoItemRepository.GetAllAsync();
        }
        
        // GET: api/ToDoItem/history
        [HttpGet("history")]
        [Produces("application/json")]
        public async Task<IEnumerable<ToDoItemDto>> GetTodoItemsHistory()
        {
            return await _toDoItemRepository.GetHistoryAsync();
        }

        // GET: api/ToDoItem/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ToDoItemDto>> GetToDoItem(int id)
        {
            var toDoItem = await _toDoItemRepository.FirstOrDefaultAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }

        // PUT: api/ToDoItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Consumes("application/json")]
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
                return BadRequest(e.Message);
            }

            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }
        
        // PUT: api/ToDoItem/markDone/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("markDone/{id}")]
        public async Task<ActionResult<ToDoItemDto>> MarkDoneToDoItem(int id)
        {
            var toDoItem = await _toDoItemRepository.MarkDoneAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }

        // POST: api/ToDoItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<CreateToDoItemDto>> PostToDoItem(CreateToDoItemDto toDoItemDto)
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
                return BadRequest(e.Message);
            }
            
            return CreatedAtAction(
                nameof(GetToDoItem), 
                new { id = toDoItem.Id }, 
                toDoItem);
        }

        // DELETE: api/ToDoItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            if (!ToDoItemExists(id))
            {
                return NotFound();
            }
            
            await _toDoItemRepository.RemoveAsync(id);

            return NoContent();
        }

        private bool ToDoItemExists(int id)
        {
            return _toDoItemRepository.Exists(id);
        }
    }
}

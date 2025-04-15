using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListRepository _toDoListRepository;

        public ToDoListController(IToDoListRepository toDoListRepository)
        {
            _toDoListRepository = toDoListRepository;
        }

        // GET: api/ToDoList
        [HttpGet]
        public async Task<IEnumerable<ToDoListDto>> GetTodoLists()
        {
            return await _toDoListRepository.GetAllAsync();
        }

        // GET: api/ToDoList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoListDto>> GetToDoList(int id)
        {
            var toDoList = await _toDoListRepository.FirstOrDefaultAsync(id);

            if (toDoList == null)
            {
                return NotFound();
            }

            return toDoList;
        }

        // PUT: api/ToDoList/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoList(int id, UpdateToDoListDto toDoListDto)
        {
            if (id != toDoListDto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _toDoListRepository.UpdateAsync(new ToDoListDto(toDoListDto.Id, toDoListDto.Title, null));
            }
            catch (NullReferenceException e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        // POST: api/ToDoList
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CreateToDoListDto>> PostToDoList(CreateToDoListDto toDoListDto)
        {
            ToDoListDto toDoList = await _toDoListRepository.AddAsync(new ToDoListDto(0, toDoListDto.Title, null));

            return CreatedAtAction(
                nameof(GetToDoList),
                new { id = toDoList.Id }, 
                toDoList);
        }

        // DELETE: api/ToDoList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoList(int id)
        {
            if (!ToDoListExists(id))
            {
                return NotFound();
            }
            
            await _toDoListRepository.RemoveAsync(id);

            return NoContent();
        }

        private bool ToDoListExists(int id)
        {
            return _toDoListRepository.Exists(id);
        }
    }
}

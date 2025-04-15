using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Repository.Services;

public class ToDoListRepository: IToDoListRepository
{
    protected readonly ToDoContext _context;
    protected readonly DbSet<ToDoList> RepoDbSet;

    public ToDoListRepository(ToDoContext toDoContext)
    {
        _context = toDoContext;
        RepoDbSet = _context.Set<ToDoList>();
    }
    
    protected virtual IQueryable<ToDoList> CreateQuery(bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();
        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    
    public ToDoListDto Add(ToDoListDto entity)
    {
        return DtoConverter.ToToDoListDto(RepoDbSet.Add(
                new ToDoList{
                    Title = entity.Title
                }
            ).Entity
        );
    }

    public ToDoListDto? Update(ToDoListDto entity)
    {
        var result = CreateQuery(false).FirstOrDefault(item => item.Id == entity.Id);
        if (result != null)
        {
            result.Title = entity.Title;
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();

            return DtoConverter.ToToDoListDto(result);
        }

        return null;
    }

    public ToDoListDto Remove(ToDoList entity)
    {
        var toDoItem = RepoDbSet.Remove(entity).Entity;
        if (!toDoItem.ToDoItems.IsNullOrEmpty())
        {
            toDoItem.ToDoItems!.Clear();
        }
        
        _context.SaveChanges();
        return DtoConverter.ToToDoListDto(toDoItem);
    }

    public ToDoListDto Remove(int id)
    {
        var result = CreateQuery().FirstOrDefault(item => item.Id == id);
        if (result == null)
        {
            throw new NullReferenceException($"Entity {nameof(ToDoList)} with id {id} was not found");
        }
        return Remove(result);
    }

    public ToDoListDto? FirstOrDefault(int id, bool noTracking = true)
    {
        var result = CreateQuery(noTracking)
            .Include(list => list.ToDoItems)
            .FirstOrDefault(item => item.Id == id);
        if (result == null) return null;
        
        return DtoConverter.ToToDoListDto(result);
    }

    public IEnumerable<ToDoListDto> GetAll(bool noTracking = true)
    {
        return CreateQuery(noTracking)
            .Include(list => list.ToDoItems)
            .Select(item => DtoConverter.ToToDoListDto(item))
            .ToList();
    }

    public bool Exists(int id)
    {
        return RepoDbSet.Any(e => e.Id == id);
    }

    public async Task<ToDoListDto> AddAsync(ToDoListDto entity)
    {

        var result = (await RepoDbSet.AddAsync(
            new ToDoList
            {
                Title = entity.Title,
            }
        )).Entity;
        await _context.SaveChangesAsync();
        return DtoConverter.ToToDoListDto(result);
    }

    public async Task<ToDoListDto?> UpdateAsync(ToDoListDto entity)
    {
        var result = await CreateQuery(false).FirstOrDefaultAsync(item => item.Id == entity.Id);
        if (result != null)
        {
            result.Title = entity.Title;

            RepoDbSet.Entry(result).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return DtoConverter.ToToDoListDto(result);
        }

        return null;
    }

    public async Task<ToDoListDto?> FirstOrDefaultAsync(int id, bool noTracking = true)
    {
        var result = await CreateQuery(noTracking)
            .Include(list => list.ToDoItems)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (result == null) return null;
        
        return DtoConverter.ToToDoListDto(result);
    }

    public async Task<IEnumerable<ToDoListDto>> GetAllAsync(bool noTracking = true)
    {
        return await CreateQuery(noTracking)
            .Include(list => list.ToDoItems)
            .Select(item => DtoConverter.ToToDoListDto(item))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await RepoDbSet.AnyAsync(e => e.Id == id);
    }

    public async Task<ToDoListDto> RemoveAsync(int id)
    {
        var result = await CreateQuery()
            .Include(list => list.ToDoItems)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (result == null)
        {
            throw new NullReferenceException($"Entity {nameof(ToDoList)} with id {id} was not found");
        }
        return Remove(result);
    }
}
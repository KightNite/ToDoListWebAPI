namespace ToDoListWebAPI.Repository.Interfaces;

public interface IEntityRepository<TDtoEntity, TDomainEntity, TKey>
    where TKey : IEquatable<TKey>
{
    // sync
    TDtoEntity Add(TDtoEntity entity);
    TDtoEntity? Update(TDtoEntity entity);
    TDtoEntity Remove(TDomainEntity entity);
    TDtoEntity Remove(TKey id);
    TDtoEntity? FirstOrDefault(TKey id, bool noTracking = true);
    IEnumerable<TDtoEntity> GetAll(bool noTracking = true);
    bool Exists(TKey id);

    // async
    Task<TDtoEntity> AddAsync(TDtoEntity entity);
    Task<TDtoEntity?> UpdateAsync(TDtoEntity entity);
    Task<TDtoEntity?> FirstOrDefaultAsync(TKey id, bool noTracking = true);
    Task<IEnumerable<TDtoEntity>> GetAllAsync(bool noTracking = true);
    Task<bool> ExistsAsync(TKey id);
    Task<TDtoEntity> RemoveAsync(TKey id);
}
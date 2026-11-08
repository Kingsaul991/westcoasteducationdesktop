using WestcoastEducation.Desktop.Models;
 
namespace WestcoastEducation.Desktop.Services;
 
public interface IRestDbService<T> where T : IEntity
{
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(string id);
}
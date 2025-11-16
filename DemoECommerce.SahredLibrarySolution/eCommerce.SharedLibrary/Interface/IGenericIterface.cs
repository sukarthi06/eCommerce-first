using eCommerce.SharedLibrary.Responses;
using System.Linq.Expressions;
namespace eCommerce.SharedLibrary.Interface;

public interface IGenericIterface<T> where T : class
{
    Task<Response> CreateAsync(T entity);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);
    Task<T?> FindByIdAsync(int id);
    Task<IEnumerable<T?>> GetAllAsync();
    Task<T?> GetByAsync(Expression<Func<T, bool>> filter);

}

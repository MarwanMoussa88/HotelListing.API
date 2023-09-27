using HotelListing.API.Models.PagedResult;
using HotelListing.API.Models.QueryParameters;

namespace HotelListing.API.Repository.IRepository
{

    /*
     * Generic Repository Design Pattern Implementation
     * For CRUD Operation**/
    public interface IGenericRepository<T> where T : class
    {
        Task<TResult> Add<TSource, TResult>(TSource entity);

        Task Update<TSource>(int id, TSource source);

        Task<IEnumerable<TResult>> GetAll<TResult>();

        Task<TResult> Get<TResult>(int id);
        Task Add(T entity);

        Task Update(T entity);


        Task<IEnumerable<T>> GetAll();


        Task<PagedResult<TResult>> GetAll<TResult>(QueryParameters queryParameters);
        Task Delete(int? id);
        Task<T> Get(int? id);


        Task<bool> Exists(int id);

    }
}

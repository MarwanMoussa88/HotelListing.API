namespace HotelListing.API.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<T> Get(int? id);
        Task<IEnumerable<T>> GetAll();
        Task<bool> Exists(int id);

    }
}

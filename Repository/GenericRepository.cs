using HotelListing.API.Data;
using HotelListing.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        public GenericRepository(HotelListingDbContext context) 
        {
            _context = context;
        }
        public async Task Add(T entity)
        {
            await _context.AddAsync<T>(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _context.Remove<T>(entity);
            await _context.SaveChangesAsync();     
        }

        public async Task<bool> Exists(int id)
        {
            var entity= await _context.FindAsync<T>(id);
            return entity is null ? false : true;
        }

        public async Task<T> Get(int? id)
        {
            if(id is null)
            {
                return null;
            }

            return await _context.FindAsync<T>(id); ;

        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();

        }

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            
        }
        
    }
}

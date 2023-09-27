using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Models.PagedResult;
using HotelListing.API.Models.QueryParameters;
using HotelListing.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    /*
     * Implementation for the Generic Repository class CRUD Operations**/
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        //Dependency Injection for the Database
        public GenericRepository(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        //Create
        public async Task Add(T entity)
        {
            await _context.AddAsync<T>(entity);
            await _context.SaveChangesAsync();

        }

        public async Task<TResult> Add<TSource, TResult>(TSource entity)
        {
            //map to entity details to orignal entity
            var genericEntity = _mapper.Map<T>(entity);
            // add to database
            await _context.AddAsync(genericEntity);
            //save
            await _context.SaveChangesAsync();
            //map orignal back to details
            return _mapper.Map<TResult>(genericEntity);
        }

        //Delete
        public async Task Delete(int? id)
        {
            var entity = await _context.FindAsync<T>(id);
            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        //Does it Exisit

        public async Task<bool> Exists(int id)
        {
            var entity = await _context.FindAsync<T>(id);
            return entity is null ? false : true;
        }
        //Retrieve one by id

        public async Task<T> Get(int? id)
        {
            if (id is null)
            {
                return null;
            }

            return await _context.FindAsync<T>(id);

        }

        public async Task<TResult> Get<TResult>(int id)
        {

            var entity = await _context.FindAsync<T>(id);
            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }
            return _mapper.Map<TResult>(entity);
        }

        //Retrieve all

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();

        }

        public async Task<PagedResult<TResult>> GetAll<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _context.Set<T>().CountAsync();
            var items = await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PagedResult<TResult>
            {
                records = items,
                PageIndex = queryParameters.PageNumber,
                TotalCount = totalSize,
                RecordNumber = queryParameters.PageSize
            };
        }

        public async Task<IEnumerable<TResult>> GetAll<TResult>()
        {
            var entities = await _context.Set<T>().ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync();
            return entities;
        }

        //Update

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

        }
        // dto->orignal
        public async Task Update<TSource>(int id, TSource source)
        {
            //Get Orignal from id
            var entity = await Get(id);
            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }
            //Map Dto Object to orignal object
            _mapper.Map(source, entity);
            //Update
            _context.Update(entity);
            //Save
            await _context.SaveChangesAsync();

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Domain;
using System.Linq.Expressions;
namespace Shared.Infrastructure.Database
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected IDbContextTransaction _transaction;

        public BaseRepository(DbContext appDbContext)
        {
            this._context = appDbContext;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public IEnumerable<T> GetAll(string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.ToList();

        }




        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.ToListAsync(cancellationToken);

        }

        public T GetById(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken);
        }

        public T Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.SingleOrDefault(criteria);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.SingleOrDefaultAsync(criteria, cancellationToken);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(criteria).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int skip, int take)
        {
            return _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int take, int skip, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return await query.ToListAsync();
        }
        public IEnumerable<T> Paginate(int take, int skip)
        {
            return _context.Set<T>().Skip(skip).Take(take).ToList();
        }
        public async Task<IEnumerable<T>> PaginateAsync(int take, int skip)
        {
            return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().CountAsync(criteria, cancellationToken);
        }



        public bool Any()
        {
            return _context.Set<T>().Any();
        }

        public bool Any(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Any(criteria);
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AnyAsync(cancellationToken);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AnyAsync(criteria, cancellationToken);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> criteria)
        {
            
            return _context.Set<T>().Where(criteria);
        }



        //// transaction
        //public async Task BeginTransactionAsync()
        //{
        //    // Ensure that no transaction is active
        //    if (_transaction != null)
        //        throw new InvalidOperationException("A transaction is already in progress.");

        //    // Start a new transaction
        //    _transaction = await _context.Database.BeginTransactionAsync();
        //}

        //public async Task CommitTransactionAsync()
        //{
        //    if (_transaction == null)
        //        throw new InvalidOperationException("No transaction is in progress.");

        //    // Commit the transaction
        //    await _transaction.CommitAsync();
        //    _transaction.Dispose();
        //    _transaction = null;
        //}

        //public async Task RollbackTransactionAsync()
        //{
        //    if (_transaction == null)
        //        throw new InvalidOperationException("No transaction is in progress.");

        //    // Rollback the transaction
        //    await _transaction.RollbackAsync();
        //    _transaction.Dispose();
        //    _transaction = null;
        //}


    }
}

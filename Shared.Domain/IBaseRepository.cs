using System.Linq.Expressions;

namespace Shared.Domain
{
    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }

    public interface IBaseRepository<T> where T : class
    {
        T GetById(Guid id,CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(string[] includes = null); // my code

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null, CancellationToken cancellationToken = default); // mycode

        // 🟢 الدالة الجديدة (Where)
        // بترجع IQueryable عشان تسمح بتركيب شروط إضافية قبل التنفيذ في الداتابيز
        IQueryable<T> Where(Expression<Func<T, bool>> criteria);

        T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null, CancellationToken cancellationToken = default);

        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int take, int skip);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int skip, int take, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

        IEnumerable<T> Paginate(int take, int skip);
        Task<IEnumerable<T>> PaginateAsync(int take, int skip);

        T Add(T entity);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        T Update(T entity);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);

        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        bool Any();
        bool Any(Expression<Func<T, bool>> criteria);
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        //// Transaction Management

        //// Begin a new transaction
        //Task BeginTransactionAsync();
        //// Commit the current transaction
        //Task CommitTransactionAsync();
        //// Rollback the current transaction
        //Task RollbackTransactionAsync();
    }
}
using System.Linq;
using System.Threading.Tasks;

namespace Base.Domain.Common
{
    public interface IRepository<T> where T : class
    {
        IUnitOfWork UnitOfWork { get; }
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task<T> FindAsync(string id);
        IQueryable<T> Get(ISpecification<T> specification);
        IQueryable<T> Paging(IPagingSpecification<T> specification);
    }
}

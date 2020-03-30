using Base.Domain.Common;
using Base.Infrastructure;
using IDS.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IDS.Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        public IDSAppContext context { get; }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return context;
            }
        }

        public IdentityRepository(IDSAppContext context)
        {
            this.context = context;
        }

        public async Task<Identity> FindAsync(string id)
        {
            return await context.Set<Identity>().FindAsync(id);
        }

        public IQueryable<Identity> Get(ISpecification<Identity> specification)
        {
            var queryableResult = specification.Includes.Aggregate(context.Set<Identity>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).AsNoTracking();
        }

        public IQueryable<Identity> Paging(IPagingSpecification<Identity> specification)
        {
            var noIdentity = string.IsNullOrWhiteSpace(specification.OrderBy);
            var queryableResult = specification.Includes.Aggregate(context.Set<Identity>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).OrderBy(noIdentity ? "ModifiedTime" : specification.OrderBy, noIdentity ? true : specification.Desc).Skip((specification.Page - 1) * specification.PageSize).Take(specification.PageSize).AsNoTracking();
        }

        public async Task AddAsync(Identity entity)
        {
            context.Set<Identity>().Add(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task UpdateAsync(Identity entity)
        {
            context.Set<Identity>().Update(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var data = await FindAsync(id);
            if (data == null) return;
            context.Set<Identity>().Remove(data);
            await context.SaveEntitiesAsync(false);
        }
    }
}

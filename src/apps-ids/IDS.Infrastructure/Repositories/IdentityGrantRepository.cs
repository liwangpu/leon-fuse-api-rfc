using Base.Domain.Common;
using Base.Infrastructure;
using IDS.Domain.AggregateModels.IdentityServerAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IDS.Infrastructure.Repositories
{
    public class IdentityGrantRepository : IIdentityGrantRepository
    {
        public IDSAppContext context { get; }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return context;
            }
        }

        public IdentityGrantRepository(IDSAppContext context)
        {
            this.context = context;
        }

        public async Task<IdentityGrant> FindAsync(string id)
        {
            return await context.Set<IdentityGrant>().FindAsync(id);
        }

        public IQueryable<IdentityGrant> Get(ISpecification<IdentityGrant> specification)
        {
            var queryableResult = specification.Includes.Aggregate(context.Set<IdentityGrant>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).AsNoTracking();
        }

        public IQueryable<IdentityGrant> Paging(IPagingSpecification<IdentityGrant> specification)
        {
            var noIdentityGrant = string.IsNullOrWhiteSpace(specification.OrderBy);
            var queryableResult = specification.Includes.Aggregate(context.Set<IdentityGrant>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).OrderBy(noIdentityGrant ? "ModifiedTime" : specification.OrderBy, noIdentityGrant ? true : specification.Desc).Skip((specification.Page - 1) * specification.PageSize).Take(specification.PageSize).AsNoTracking();
        }

        public async Task AddAsync(IdentityGrant entity)
        {
            context.Set<IdentityGrant>().Add(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task UpdateAsync(IdentityGrant entity)
        {
            context.Set<IdentityGrant>().Update(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var data = await FindAsync(id);
            if (data == null) return;
            context.Set<IdentityGrant>().Remove(data);
            await context.SaveEntitiesAsync(false);
        }
    }
}

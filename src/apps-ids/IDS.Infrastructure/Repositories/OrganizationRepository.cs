using Base.Domain.Common;
using Base.Infrastructure;
using IDS.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IDS.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        public IDSAppContext context { get; }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return context;
            }
        }

        public OrganizationRepository(IDSAppContext context)
        {
            this.context = context;
        }

        public async Task<Organization> FindAsync(string id)
        {
            return await context.Set<Organization>().FindAsync(id);
        }

        public IQueryable<Organization> Get(ISpecification<Organization> specification)
        {
            var queryableResult = specification.Includes.Aggregate(context.Set<Organization>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).AsNoTracking();
        }

        public IQueryable<Organization> Paging(IPagingSpecification<Organization> specification)
        {
            var noOrganization = string.IsNullOrWhiteSpace(specification.OrderBy);
            var queryableResult = specification.Includes.Aggregate(context.Set<Organization>().AsQueryable(), (current, include) => current.Include(include));
            return queryableResult.Where(specification.Criteria).OrderBy(noOrganization ? "ModifiedTime" : specification.OrderBy, noOrganization ? true : specification.Desc).Skip((specification.Page - 1) * specification.PageSize).Take(specification.PageSize).AsNoTracking();
        }

        public async Task AddAsync(Organization entity)
        {
            context.Set<Organization>().Add(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task UpdateAsync(Organization entity)
        {
            context.Set<Organization>().Update(entity);
            await context.SaveEntitiesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var data = await FindAsync(id);
            if (data == null) return;
            context.Set<Organization>().Remove(data);
            await context.SaveEntitiesAsync(false);
        }
    }
}

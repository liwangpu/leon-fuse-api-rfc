using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;

namespace IDS.Infrastructure.Specifications.OrganizationSpecifications
{
    public class OrganizationPagingSpecification : PagingBaseSpecification<Organization>
    {
        public OrganizationPagingSpecification(int page, int pageSize, string orderBy, bool desc, string search)
        {
            Page = page;
            PageSize = pageSize;
            OrderBy = orderBy;
            Desc = desc;
            Criteria = organ => string.IsNullOrWhiteSpace(search) ? true : organ.Name.Contains(search);
        }
    }
}

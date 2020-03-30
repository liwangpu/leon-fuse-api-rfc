using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;

namespace IDS.Infrastructure.Specifications.IdentitySpecifications
{
    public class IdentityPagingSpecification : PagingBaseSpecification<Identity>
    {
        public IdentityPagingSpecification(int page, int pageSize, string orderBy, bool desc, string search)
        {
            Page = page;
            PageSize = pageSize;
            OrderBy = orderBy;
            Desc = desc;
            Criteria = ids => string.IsNullOrWhiteSpace(search) ? true : ids.Name.Contains(search);
        }
    }
}

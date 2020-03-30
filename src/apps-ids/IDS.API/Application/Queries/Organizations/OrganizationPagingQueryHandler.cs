using AutoMapper;
using Base.API;
using IDS.Domain.AggregateModels.UserAggregate;
using IDS.Infrastructure.Specifications.OrganizationSpecifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.API.Application.Queries.Organizations
{
    public class OrganizationPagingQueryHandler : IRequestHandler<OrganizationPagingQuery, PagingQueryResult<OrganizationPagingQueryDTO>>
    {
        private readonly IOrganizationRepository organizationRepository;
        private readonly IMapper mapper;

        public OrganizationPagingQueryHandler(IOrganizationRepository  organizationRepository, IMapper mapper)
        {
            this.organizationRepository = organizationRepository;
            this.mapper = mapper;
        }

        public async Task<PagingQueryResult<OrganizationPagingQueryDTO>> Handle(OrganizationPagingQuery request, CancellationToken cancellationToken)
        {
            var result = new PagingQueryResult<OrganizationPagingQueryDTO>();
            request.CheckPagingParam();
            var specification = new OrganizationPagingSpecification(request.Page, request.PageSize, request.OrderBy, request.Desc, request.Search);
            result.Count = await organizationRepository.Get(specification).CountAsync();
            var datas = await organizationRepository.Paging(specification).ToListAsync();
            result.Items = mapper.Map<List<OrganizationPagingQueryDTO>>(datas);
            return result;
        }
    }
}

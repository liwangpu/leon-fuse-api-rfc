using AutoMapper;
using Base.API;
using IDS.Domain.AggregateModels.UserAggregate;
using IDS.Infrastructure.Specifications.IdentitySpecifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.API.Application.Queries.Identities
{
    public class IdentityPagingQueryHandler : IRequestHandler<IdentityPagingQuery, PagingQueryResult<IdentityPagingQueryDTO>>
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IMapper mapper;

        public IdentityPagingQueryHandler(IIdentityRepository identityRepository, IMapper mapper)
        {
            this.identityRepository = identityRepository;
            this.mapper = mapper;
        }

        public async Task<PagingQueryResult<IdentityPagingQueryDTO>> Handle(IdentityPagingQuery request, CancellationToken cancellationToken)
        {
            var result = new PagingQueryResult<IdentityPagingQueryDTO>();
            request.CheckPagingParam();
            var specification = new IdentityPagingSpecification(request.Page, request.PageSize, request.OrderBy, request.Desc, request.Search);
            result.Count = await identityRepository.Get(specification).CountAsync();
            var datas = await identityRepository.Paging(specification).ToListAsync();
            result.Items = mapper.Map<List<IdentityPagingQueryDTO>>(datas);
            return result;
        }
    }
}

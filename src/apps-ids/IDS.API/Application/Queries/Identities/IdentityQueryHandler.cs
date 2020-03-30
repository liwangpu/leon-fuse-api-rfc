using AutoMapper;
using Base.API;
using Base.API.Exceptions;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.API.Application.Queries.Identities
{
    public class IdentityQueryHandler : IRequestHandler<IdentityQuery, IdentityQueryDTO>
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IStringLocalizer<CommonTranslation> localizer;
        private readonly IMapper mapper;

        public IdentityQueryHandler(IIdentityRepository identityRepository, IStringLocalizer<CommonTranslation> localizer, IMapper mapper)
        {
            this.identityRepository = identityRepository;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<IdentityQueryDTO> Handle(IdentityQuery request, CancellationToken cancellationToken)
        {
            var identity = await identityRepository.FindAsync(request.Id);
            if (identity == null)
                throw new HttpResourceNotFoundException(localizer["HttpRespond.NotFound", "Identity", request.Id]);
            return mapper.Map<IdentityQueryDTO>(identity);
        }
    }
}

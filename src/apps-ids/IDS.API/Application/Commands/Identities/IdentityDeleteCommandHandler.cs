using Base.API;
using Base.API.Exceptions;
using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.API.Application.Commands.Identities
{
    public class IdentityDeleteCommandHandler : IRequestHandler<IdentityDeleteCommand>
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IStringLocalizer<CommonTranslation> localizer;
        private readonly IProfileContext profileContext;

        public IdentityDeleteCommandHandler(IIdentityRepository identityRepository, IStringLocalizer<CommonTranslation> localizer, IProfileContext profileContext)
        {
            this.identityRepository = identityRepository;
            this.localizer = localizer;
            this.profileContext = profileContext;
        }

        public async Task<Unit> Handle(IdentityDeleteCommand request, CancellationToken cancellationToken)
        {
            var identity = await identityRepository.FindAsync(request.Id);
            if (identity == null)
                throw new HttpResourceNotFoundException(localizer["HttpRespond.NotFound", "Identity", request.Id]);

            await identityRepository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}

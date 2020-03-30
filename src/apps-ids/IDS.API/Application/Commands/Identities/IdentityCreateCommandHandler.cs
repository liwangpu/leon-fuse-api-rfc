using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.API.Application.Commands.Identities
{
    public class IdentityCreateCommandHandler : IRequestHandler<IdentityCreateCommand, string>
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IProfileContext profileContext;

        public IdentityCreateCommandHandler(IIdentityRepository identityRepository, IProfileContext profileContext)
        {
            this.identityRepository = identityRepository;
            this.profileContext = profileContext;
        }

        public async Task<string> Handle(IdentityCreateCommand request, CancellationToken cancellationToken)
        {
            var identity = new Identity(request.Username, request.Password, request.Name, request.Email, request.Phone, profileContext.OrganizationId, profileContext.IdentityId);
            await identityRepository.AddAsync(identity);
            return identity.Id;
        }
    }
}

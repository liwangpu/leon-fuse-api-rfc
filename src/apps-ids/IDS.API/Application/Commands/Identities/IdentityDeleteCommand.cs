using MediatR;

namespace IDS.API.Application.Commands.Identities
{
    public class IdentityDeleteCommand : IRequest
    {
        public string Id { get; protected set; }
        public IdentityDeleteCommand(string id)
        {
            Id = id;
        }
    }
}

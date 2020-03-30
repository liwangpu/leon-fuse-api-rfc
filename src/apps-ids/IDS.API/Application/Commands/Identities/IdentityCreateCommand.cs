using MediatR;

namespace IDS.API.Application.Commands.Identities
{
    public class IdentityCreateCommand : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

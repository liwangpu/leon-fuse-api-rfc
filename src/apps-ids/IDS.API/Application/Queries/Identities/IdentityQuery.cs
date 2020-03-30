using AutoMapper;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;

namespace IDS.API.Application.Queries.Identities
{
    public class IdentityQuery : IRequest<IdentityQueryDTO>
    {
        public string Id { get; protected set; }
        public IdentityQuery(string id)
        {
            Id = id;
        }
    }

    public class IdentityQueryDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public long CreatedTime { get; set; }
        public long ModifiedTime { get; set; }
    }

    public class IdentityQueryProfile : Profile
    {
        public IdentityQueryProfile()
        {
            CreateMap<Identity, IdentityQueryDTO>()
             .ForAllOtherMembers(_ => _.Condition((source, destination, sValue, dValue, context) =>
             {
                 if (sValue == null)
                 {
                     return false;
                 }

                 return true;
             }));
        }
    }
}

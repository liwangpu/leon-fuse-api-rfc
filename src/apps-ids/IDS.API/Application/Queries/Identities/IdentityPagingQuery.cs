using AutoMapper;
using Base.API;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;

namespace IDS.API.Application.Queries.Identities
{
    public class IdentityPagingQuery : PagingQueryRequest, IRequest<PagingQueryResult<IdentityPagingQueryDTO>>
    {

    }

    public class IdentityPagingQueryDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public long CreatedTime { get; set; }
        public long ModifiedTime { get; set; }
    }

    public class IdentityPagingQueryProfile : Profile
    {
        public IdentityPagingQueryProfile()
        {
            CreateMap<Identity, IdentityPagingQueryDTO>()
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

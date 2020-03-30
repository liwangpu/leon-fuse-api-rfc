using AutoMapper;
using Base.API;
using IDS.Domain.AggregateModels.UserAggregate;
using MediatR;
using System;

namespace IDS.API.Application.Queries.Organizations
{
    public class OrganizationPagingQuery : PagingQueryRequest, IRequest<PagingQueryResult<OrganizationPagingQueryDTO>>
    {

    }

    public class OrganizationPagingQueryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
        public string Icon { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string OwnerId { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime ActivationTime { get; set; }
        public string Type { get; set; }
    }

    public class OrganizationPagingQueryProfile : Profile
    {
        public OrganizationPagingQueryProfile()
        {
            CreateMap<Organization, OrganizationPagingQueryDTO>()
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

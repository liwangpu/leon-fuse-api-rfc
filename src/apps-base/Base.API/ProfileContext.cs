using Base.Domain.Common;
using System.Collections.Generic;

namespace Base.API
{
    internal class ProfileContext : IProfileContext
    {
        public string IdentityId { get; }
        public string ClientId { get; }

        public string OrganizationId { get; }

        public string UserId { get; }

        public string UserName { get; }

        public IDictionary<string, object> Properties { get; }

        public ProfileContext(string identityId, string userId, string userName, string organizationId, string clientId)
        {
            IdentityId = identityId;
            UserId = userId;
            UserName = userName;
            OrganizationId = organizationId;
            ClientId = clientId;
            Properties = new Dictionary<string, object>();
        }

    }
}

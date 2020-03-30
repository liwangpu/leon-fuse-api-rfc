using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Domain.Common
{
    public interface IProfileContext
    {
        string ClientId { get; }

        string OrganizationId { get; }

        string UserId { get; }
        string IdentityId { get; }

        string UserName { get; }

        IDictionary<string, object> Properties { get; }
    }
}

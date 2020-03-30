using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;

namespace IDS.Infrastructure.Specifications.IdentitySpecifications
{
    public class GetIdentityByUsernameAndPassword : BaseSpecification<Identity>
    {
        public GetIdentityByUsernameAndPassword(string username, string password)
        {
            var aaa = MD5Gen.CalcString(password);
            Criteria = m => (m.Username == username || m.Email == username || m.Phone == username) && m.Password == MD5Gen.CalcString(password);
        }
    }
}

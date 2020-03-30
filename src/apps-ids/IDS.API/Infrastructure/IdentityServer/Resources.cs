using IdentityServer4.Models;
using System.Collections.Generic;

namespace IDS.API.Infrastructure.IdentityServer
{
    public class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var ids = new IdentityResource[]
                {
                        new IdentityResources.OpenId()

                };

            return ids;
        }

        public static List<ApiResource> GetApiResources()
        {
            var apis = new List<ApiResource>();

            //apis.Add(new ApiResource("omsApi", "omsApi"));
            //apis.Add(new ApiResource("smsApi", "smsApi"));

            //apis.Add(new ApiResource("omsApi", "omsApi") { ApiSecrets = { new Secret("123456".Sha256()) } });
            //apis.Add(new ApiResource("smsApi", "smsApi") { ApiSecrets = { new Secret("654321".Sha256()) } });
            //apis.Add(new ApiResource("testApi", "testApi")
            //{
            //    UserClaims = new[] { "aa" }
            //});
            apis.Add(new ApiResource("serverApi", "serverApi")
            {
                ApiSecrets = { new Secret("88888888".Sha256()) }
                //UserClaims = new[] { "user.action" }
            });
            return apis;
        }
    }
}

using IdentityServer4.Models;
using System.Collections.Generic;

namespace IDS.API.Infrastructure.IdentityServer
{
    public class Clients
    {
        public static IEnumerable<Client> GetClients()
        {
            var clients = new List<Client>();
            var ocelotapp = new Client
            {
                ClientId = "server",
                AllowedGrantTypes = { "password", "wetchatUniqueId", "captchaCode" },
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequireClientSecret = false,
                AllowedScopes = { "serverApi" },
                AccessTokenType = AccessTokenType.Reference,
                AccessTokenLifetime = 3600 * 24,
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true
            };
            clients.Add(ocelotapp);
            return clients;
        }
    }
}

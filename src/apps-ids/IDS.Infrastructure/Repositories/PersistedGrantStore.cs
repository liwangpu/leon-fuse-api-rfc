using IdentityServer4.Models;
using IdentityServer4.Stores;
using IDS.Domain.AggregateModels.IdentityServerAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDS.Infrastructure.Repositories
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IIdentityGrantRepository identityGrantRepository;

        public PersistedGrantStore(IIdentityGrantRepository identityGrantRepository)
        {
            this.identityGrantRepository = identityGrantRepository;
        }

        public static IdentityGrant Transfer(PersistedGrant grant)
        {
            return new IdentityGrant(grant.Key, grant.Type, grant.SubjectId, grant.ClientId, grant.CreationTime, grant.Data, grant.Expiration);
        }

        public static PersistedGrant Transfer(IdentityGrant idsGrant)
        {
            var grant = new PersistedGrant();
            grant.Key = idsGrant.Id;
            grant.Type = idsGrant.Type;
            grant.SubjectId = idsGrant.SubjectId;
            grant.ClientId = idsGrant.ClientId;
            grant.CreationTime = idsGrant.CreationTime;
            grant.Data = idsGrant.Data;
            grant.Expiration = idsGrant.Expiration;
            return grant;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            throw new NotImplementedException();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var idsGrant = await identityGrantRepository.FindAsync(key);
            return Transfer(idsGrant);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(string key)
        {
            await identityGrantRepository.DeleteAsync(key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await identityGrantRepository.AddAsync(Transfer(grant));
        }
    }
}

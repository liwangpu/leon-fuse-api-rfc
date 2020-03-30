using Base.Domain.Common;
using System;

namespace IDS.Domain.AggregateModels.IdentityServerAggregate
{
    public class IdentityGrant : Entity
    {
        public string Id { get; protected set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get; protected set; }
        /// <summary>
        ///  Gets the subject identifier.
        /// </summary>
        public string SubjectId { get; protected set; }
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        public string ClientId { get; protected set; }
        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        public DateTime CreationTime { get; protected set; }
        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        public DateTime? Expiration { get; protected set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public string Data { get; protected set; }

        #region ctor
        protected IdentityGrant()
        {

        }
        public IdentityGrant(string key, string type, string subjejctId, string clientId, DateTime creationTime, string data, DateTime? expiration)
        {
            Id = key;
            Type = type;
            SubjectId = subjejctId;
            ClientId = clientId;
            CreationTime = creationTime;
            Data = data;
            Expiration = expiration;
        }
        #endregion
    }
}

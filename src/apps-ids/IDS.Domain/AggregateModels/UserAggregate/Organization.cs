using Base.Domain;
using Base.Domain.Common;
using System;
using System.Collections.Generic;

namespace IDS.Domain.AggregateModels.UserAggregate
{
    public class Organization : Entity
    {
        private readonly List<Identity> _ownAccounts;
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string ParentId { get; protected set; }
        public string Icon { get; protected set; }
        public string Mail { get; protected set; }
        public string Phone { get; protected set; }
        public string Location { get; protected set; }
        public string OwnerId { get; protected set; }
        public DateTime ExpireTime { get; protected set; }
        public DateTime ActivationTime { get; protected set; }
        public string Type { get; protected set; }
        public string Creator { get; protected set; }
        public string Modifier { get; protected set; }
        public DateTime CreatedTime { get; protected set; }
        public DateTime ModifiedTime { get; protected set; }
        public IReadOnlyCollection<Identity> OwnAccounts => _ownAccounts;

        #region ctor
        private Organization()
        {

        }
        public Organization(string name, string description, string parentId, string mail, string phone, string location, string ownerId, DateTime expireTime, DateTime activationTime, string creator)
        {
            Id = GuidGen.NewGUID();
            Name = name;
            Description = description;
            ParentId = parentId;
            Mail = mail;
            Phone = phone;
            Location = location;
            OwnerId = ownerId;
            ParentId = parentId;
            ExpireTime = expireTime;
            ActivationTime = activationTime;
            Creator = creator;
            Modifier = Creator;
            CreatedTime = DateTime.UtcNow;
            ModifiedTime = CreatedTime;
        }
        #endregion

        public void InitializeId(string id)
        {
            Id = id;
        }
    }
}

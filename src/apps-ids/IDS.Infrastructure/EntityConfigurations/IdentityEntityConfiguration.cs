using Base.Domain.Common;
using IDS.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace IDS.Infrastructure.EntityConfigurations
{
    public class IdentityEntityConfiguration : IEntityTypeConfiguration<Identity>
    {
        public void Configure(EntityTypeBuilder<Identity> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Ignore(b => b.DomainEvents);
            builder.Property(x => x.Username);
            builder.Property(x => x.Password);
            builder.Property(b => b.Name);
            builder.Property(x => x.Email);
            builder.Property(x => x.Phone);
            builder.Property(x => x.Creator);
            builder.Property(x => x.Modifier);
            builder.Property(x => x.CreatedTime);
            builder.Property(x => x.ModifiedTime);
            builder.HasIndex(x => x.OrganizationId);
            builder.Metadata.FindNavigation(nameof(Identity.Organization)).SetPropertyAccessMode(PropertyAccessMode.Field);
            var identities = new List<Identity>();
            var admin = new Identity("admin", "123456", "Admin", "bamboo@bamboo.com", "157", "bamboo", "admin");
            admin.InitializeId("admin");
            identities.Add(admin);
            builder.HasData(identities);
        }
    }
}

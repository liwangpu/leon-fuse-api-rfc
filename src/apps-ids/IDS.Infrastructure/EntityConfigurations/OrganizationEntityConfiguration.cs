using IDS.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace IDS.Infrastructure.EntityConfigurations
{
    public class OrganizationEntityConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Ignore(b => b.DomainEvents);
            builder.Property(x => x.Name);
            builder.Property(x => x.Description);
            builder.Property(x => x.Mail);
            builder.Property(x => x.Phone);
            builder.Property(x => x.Location);
            builder.Property(x => x.OwnerId);
            builder.Property(x => x.ExpireTime);
            builder.Property(x => x.ActivationTime);
            builder.Property(x => x.Type);
            var navigation = builder.Metadata.FindNavigation(nameof(Organization.OwnAccounts));
            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            var organizations = new List<Organization>();
            var softwareProvider = new Organization("竹烛", "", null, "bamboo@bamboo", "157", null, "admin", DateTime.Now.AddYears(100), DateTime.Now.AddYears(-1), "admin");
            softwareProvider.InitializeId("bamboo");
            organizations.Add(softwareProvider);
            builder.HasData(organizations);
        }
    }
}

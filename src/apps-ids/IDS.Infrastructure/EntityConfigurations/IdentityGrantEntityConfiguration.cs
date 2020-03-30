
using IDS.Domain.AggregateModels.IdentityServerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDS.Infrastructure.EntityConfigurations
{
    public class IdentityGrantEntityConfiguration : IEntityTypeConfiguration<IdentityGrant>
    {
        public void Configure(EntityTypeBuilder<IdentityGrant> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Type);
            builder.Property(x => x.SubjectId);
            builder.Property(x => x.ClientId);
            builder.Property(b => b.CreationTime);
            builder.Property(x => x.Expiration);
            builder.Property(x => x.Data);
        }
    }
}

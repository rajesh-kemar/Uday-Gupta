using Kemar.WHL.Repository.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kemar.WHL.Repository.Configurations
{
    internal abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T:BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(b => b.CreatedAt).IsRequired();
            builder.Property(b => b.IsDeleted).HasDefaultValue(false);
            builder.Property(b => b.CreatedBy).HasMaxLength(100);
            builder.Property(b => b.UpdatedBy).HasMaxLength(100);
        }
    }
}
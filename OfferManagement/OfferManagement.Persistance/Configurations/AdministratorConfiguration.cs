using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Domain.Entities;

namespace OfferManagement.Persistance.Configurations
{
    public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
    {
        public void Configure(EntityTypeBuilder<Administrator> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(a => a.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(u => u.PasswordSalt)
                .IsRequired()
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(a => a.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }

}

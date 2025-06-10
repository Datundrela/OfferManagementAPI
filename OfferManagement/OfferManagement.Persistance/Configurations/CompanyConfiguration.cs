using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Persistance.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(u => u.PasswordSalt)
                .IsRequired()
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(c => c.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.HasMany(c => c.Offers)
                .WithOne(o => o.Company)
                .HasForeignKey(o => o.CompanyId);

            builder.HasOne(o => o.Image)
                .WithOne()
                .HasForeignKey<Company>(o => o.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}

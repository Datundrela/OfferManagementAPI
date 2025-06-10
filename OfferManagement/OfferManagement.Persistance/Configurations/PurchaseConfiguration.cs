using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Persistance.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OfferManagement.Domain.Entities;

    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            // Table Name
            builder.ToTable("Purchases");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Quantity - Required
            builder.Property(p => p.Quantity)
                .IsRequired();

            // TotalPrice - Decimal Precision (18,4)
            builder.Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            // PurchaseDate - Default to current timestamp
            builder.Property(p => p.PurchaseDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            // IsRefunded - Default false
            builder.Property(p => p.IsRefunded)
                .HasDefaultValue(false)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Offer)
                .WithMany()
                .HasForeignKey(p => p.OfferId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

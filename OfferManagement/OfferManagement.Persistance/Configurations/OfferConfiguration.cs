using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Domain.Entities;

namespace OfferManagement.Persistance.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.Description)
                .HasMaxLength(500);

            builder.Property(o => o.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Quantity)
                .IsRequired();

            builder.Property(o => o.ExpiryDate)
                .IsRequired();

            builder.Property(o => o.UploadDate)
                .IsRequired();

            builder.Property(o => o.OfferStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(o => o.Company)
                .WithMany(c => c.Offers)
                .HasForeignKey(o => o.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Category)
                .WithMany(c => c.Offers)
                .HasForeignKey(o => o.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Image)
                .WithOne()
                .HasForeignKey<Offer>(o => o.ImageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

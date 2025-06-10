using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfferManagement.Domain.Entities;

namespace OfferManagement.Persistance.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.UploadedAt)
                .IsRequired();
        }
    }
}

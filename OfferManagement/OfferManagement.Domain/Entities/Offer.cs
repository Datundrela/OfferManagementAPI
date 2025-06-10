namespace OfferManagement.Domain.Entities
{
    public class Offer
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public OfferStatus OfferStatus { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int? ImageId { get; set; } = null;

        public Image? Image { get; set; } = null;
    }
}

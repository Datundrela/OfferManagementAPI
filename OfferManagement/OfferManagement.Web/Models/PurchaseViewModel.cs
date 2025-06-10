namespace OfferManagement.Web.Models
{
    public class PurchaseViewModel
    {
        public int OfferId { get; set; }
        public string OfferTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}

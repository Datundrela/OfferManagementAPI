namespace OfferManagement.Web.Models
{
    public class PurchaseMVCDTO
    {
        public int Id { get; set; }
        public string OfferTitle { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsRefunded { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}

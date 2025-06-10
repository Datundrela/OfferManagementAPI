using OfferManagement.Application.Purchases.Models;

namespace OfferManagement.Web.Models
{
    public class PurchaseListViewModel
    {
        public IEnumerable<PurchaseMVCDTO> Purchases { get; set; }
    }

}

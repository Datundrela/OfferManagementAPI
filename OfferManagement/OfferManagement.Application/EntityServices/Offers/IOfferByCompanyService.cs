using OfferManagement.Application.EntityServices.Offers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Offers
{
    public interface IOfferByCompanyService
    {
        Task<OfferResponseModel> AddAsync(AddOfferRequestModel model, int companhyId, CancellationToken cancellationToken);
        Task<List<OfferDTO>> GetCompanyAllOffersAsync(int companyId, CancellationToken cancellationToken);
        Task<OfferResponseModel> CancelOfferAsync(int offerId, CancellationToken cancellationToken);
    }
}

using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Infrastructure.Images
{
    public class ImageRepository : BaseRepository<Image>, IImageRepository
    {
        public ImageRepository(OfferManagementContext context) : base(context) { }
    }

}

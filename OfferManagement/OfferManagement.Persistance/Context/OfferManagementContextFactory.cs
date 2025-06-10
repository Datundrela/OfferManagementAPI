/*using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace OfferManagement.Persistance.Context
{
    public class OfferManagementContextFactory : IDesignTimeDbContextFactory<OfferManagementContext>
    {
        public OfferManagementContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OfferManagementContext>();
            
            optionsBuilder.UseSqlServer("Server=localhost;Database=OffersDB;Integrated Security=True;Encrypt=False;");

            return new OfferManagementContext(optionsBuilder.Options);
        }
    }
}*/

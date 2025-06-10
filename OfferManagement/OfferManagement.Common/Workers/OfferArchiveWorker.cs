using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfferManagement.Application.EntityServices.Offers;
using Serilog;

namespace OfferManagement.Common.Workers
{
    public class OfferArchiveWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OfferArchiveWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.Information("OfferArchiveWorker started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

                        await offerService.ArchiveExpiredOffersAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in OfferArchiveWorker", ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }
    }
}

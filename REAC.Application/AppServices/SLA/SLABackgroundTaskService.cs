using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.SLA;

namespace REAC.Application.AppServices.SLA
{
    public class SLABackgroundTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly object _lock = new();
        private Timer? _timer = null;
        bool canExecute = true;

        public SLABackgroundTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(async t =>
            {
                await DoWork(stoppingToken);
            }
           , null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            if (canExecute)
            {
                canExecute = false;
                try
                {
                    using (var scope1 = _scopeFactory.CreateScope())
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var slaReviewService = scope.ServiceProvider.GetRequiredService<IReviewSlaService>();
                            if (slaReviewService is not null)
                                await slaReviewService.Execute();

                            var slaReconciliationService = scope.ServiceProvider.GetRequiredService<IReconciliationSlaService>();
                            if (slaReconciliationService is not null)
                                await slaReconciliationService.Execute();

                            var slaArbitrationService = scope.ServiceProvider.GetRequiredService<IArbitrationSlaService>();
                            if (slaArbitrationService is not null)
                                await slaArbitrationService.Execute();

                        }
                    }
                }
                catch (Exception e)
                {

                }
                finally
                {
                    canExecute = true;

                }
            }
        }

    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REAC.Application.Contracts.IAppServices.Integration;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class NafathBackgroundTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly object _lock = new();
        private Timer? _timer = null;
        bool canExecute = true;

        public NafathBackgroundTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(async t =>
            {
                await DoWork(stoppingToken);
            }
           , null, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));

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
                            //var nafathService = scope.ServiceProvider.GetRequiredService<INafathService>();
                            //if (nafathService is not null)
                            //    await nafathService.CheckNafathStatus();
                            
                            var nafathAgreementRequestService = scope.ServiceProvider.GetRequiredService<INafathService>();
                            if (nafathAgreementRequestService is not null)
                                await nafathAgreementRequestService.CheckNafathAgreementRequestStatus();
                            
                            var nafathReconciliationReportService = scope.ServiceProvider.GetRequiredService<INafathService>();
                            if (nafathReconciliationReportService is not null)
                                await nafathReconciliationReportService.CheckNafathReconciliationReportStatus();

                            canExecute = true;
                        }
                    }
                }
                catch (Exception)
                {
                    canExecute = true;
                }
            }
        }

    }
}
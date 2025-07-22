using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REAC.Application.Contracts.IAppServices.SLA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.SLA
{
    public class SLANotificationBackgroundTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly object _lock = new();
        private Timer? _timer = null;
        bool canExecute = true;
        public SLANotificationBackgroundTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async t =>
            {
                await DoWork(stoppingToken); 
            }
           , null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1800));//to run every 30 min
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
                            var slaSendEmailService = scope.ServiceProvider.GetRequiredService<ISendEmailSLAService>();
                            var slaSendSMSService = scope.ServiceProvider.GetRequiredService<ISendSMSSLAService>();

                            //if (SendEmailSLAService.emailSetting is null)
                            //    await slaSendEmailService.GetEmailSettings();

                            if (slaSendEmailService is not null)
                                await slaSendEmailService.SendMail();
                            
                            if (slaSendSMSService is not null)
                                await slaSendSMSService.SendSMS();

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

using Core.Log.EntityFrameworkCore;
using Core.Security;
using Core.Security.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using REAC.EntityFrameworkCore.Entities.System;
using Ticketing.EntityFrameworkCore.SeedData;

namespace REAC.EntityFrameworkCore.Seed
{
    public static class SeedExtension
    {
        /// <summary>
        /// Seed initial data , apply migrations
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task Seed(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<string>>();
            var context = scope.ServiceProvider.GetRequiredService<REACDBContext>();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            var logDbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            try
            {
                logger.LogInformation("Seed Data Starting");

                context.Database.Migrate();
                identityContext.Database.Migrate();
                logDbContext.Database.Migrate();

                await DefaultRolesCreator.CreateAsync(roleManager);
                await DefaultUsersCreator.CreateAsync(userManager, roleManager, identityContext);
                userManager.Dispose();
                roleManager.Dispose();
                await BankSeedDataCreator.SeedAsync(context);
                await LitigationRequestAmountSeedDataCreator.SeedAsync(context);
                await ExemptionReductionCategorySeedDataCreator.SeedAsync(context);
                await CitizenRequestStatusSeedDataCreator.SeedAsync(context);
                await FileSeedDataCreator.SeedAsync(context);
                await DelegationRelationshipTypeSeedDataCreator.SeedAsync(context);
                await GenderSeedDataCreator.SeedAsync(context);
                await IdentityTypeSeedDataCreator.SeedAsync(context);
                await LitigationRequestCategorySeedDataCreator.SeedAsync(context);
                await LitigationRequestContractTypeSeedDataCreator.SeedAsync(context);
                await LitigationRequestCorrespondenceDestinationSeedDataCreator.SeedAsync(context);
                await LitigationRequestExtendPeriodRequestStatusSeedDataCreator.SeedAsync(context);
                await LitigationRequestOwnerTypeSeedDataCreator.SeedAsync(context);
                await LitigationRequestStatusSeedDataCreator.SeedAsync(context);
                await AreasSeedDataCreator.SeedAsync(context);
                await CitiesSeedDataCreator.SeedAsync(context);
                await ReformerStatusSeedDataCreator.SeedAsync(context);
                await ArbitratorAssigningRequestSeedDataCreator.SeedAsync(context);

                await MeetingLocationSeedDataCreator.SeedAsync(context);
                await ArbitratorNominationListCountSeedDataCreator.SeedAsync(context);
                await ArbitratorCountSeedDataCreator.SeedAsync(context);
                await CorrespondenceStatusSeedDataCreator.SeedAsync(context);


                await NationalitySeedDataCreator.SeedAsync(context);
                await ArbitratorClassessSeedDataCreator.SeedAsync(context);
                await CitizenRequestTypeSeedDataCreator.SeedAsync(context);
                await RequestTypeSeedDataCreator.SeedAsync(context);
                await NotificationActorTypeSeedDataCreator.SeedAsync(context);
                await ArbitratorStatusSeedDataCreator.SeedAsync(context);


                await SettingSeedDataCreator.SeedAsync(context);
                await EmployeeSeedDataCreator.SeedAsync(context);
                await CitizenSeedDataCreator.SeedAsync(context);
                await FacilitySeedDataCreator.SeedAsync(context);
                await DelegatorSeedDataCreator.SeedAsync(context);

                await OtherCategoriesSeedData.SeedAsync(context);

                await DefaultWorkFlowDataCreator.CreateAsync(context);

                context.SaveChanges();

                logger.LogInformation("***********************************************");
                logger.LogInformation("***********************************************");
                logger.LogInformation("***********************************************");
                logger.LogInformation("Seed Data End");
                logger.LogInformation("***********************************************");
                logger.LogInformation("***********************************************");
                logger.LogInformation("***********************************************");
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "Error occured during migrations");
            }
        }
    }
}
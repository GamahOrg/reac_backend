using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    static class DelegatorSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            try
            {
                if (await context.Delegators.AnyAsync()) return;
                await context.Delegators.AddRangeAsync(
                    new List<Delegator>
                    {
                        new Delegator
                        {
                            Id = Guid.Parse("057acd1e-87d5-49b7-966b-034dc397801c"),
                            CitizenId =  Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                            DelegationRelationshipTypeId = DelegationRelationshipType.Agent,
                            DelegatorCitizenId = Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                            IsActive = true,
                            DelegationFileId = context.SysFileManagers.ToList().FirstOrDefault().Id,
                        },
                        new Delegator
                        {
                            Id = Guid.Parse("68d1ff0b-43ae-41cd-9a47-a458750628a8"),
                            CitizenId =  Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                            DelegationRelationshipTypeId = DelegationRelationshipType.Lawyer,
                            DelegatorCitizenId = Guid.Parse("04540979-8ae2-489f-bf90-d59790d7204e"),
                            IsActive = true,
                            DelegationFileId = context.SysFileManagers.ToList().FirstOrDefault().Id,
                        },
                        new Delegator
                        {
                            Id = Guid.Parse("cf81b1e4-78a1-4ad0-a79f-ec83883a5f39"),
                            FacilityId =  Guid.Parse("25d5b134-3f63-4daf-a92d-0ce224ee024c"),
                            DelegationRelationshipTypeId = DelegationRelationshipType.Lawyer,
                            DelegatorCitizenId = Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                            IsActive = true,
                            DelegationFileId = context.SysFileManagers.ToList().FirstOrDefault().Id,
                        },
                        new Delegator
                        {
                            Id = Guid.Parse("dedc1926-070c-422a-a5bd-181743fbbc61"),
                            FacilityId =  Guid.Parse("deef9a46-e906-4f89-950b-0d0e8cb7e5a1"),
                            DelegationRelationshipTypeId = DelegationRelationshipType.Agent,
                            DelegatorCitizenId = Guid.Parse("04540979-8ae2-489f-bf90-d59790d7204e"),
                            IsActive = true,
                            DelegationFileId = context.SysFileManagers.ToList().FirstOrDefault().Id,
                        }

                    });
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

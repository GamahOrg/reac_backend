using Core.Security;
using Core.Security.Authorization;
using Core.Security.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace REAC.EntityFrameworkCore.Seed
{
    public static class DefaultUsersCreator
    {
        /// <summary>
        /// Seed Default admin users 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManger"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task CreateAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            await SeedAdminUserAsync(userManager, roleManger, context);

            await SeedAdmin1UserAsync(userManager, roleManger, context);
            await SeedSubAdmin1UserAsync(userManager, roleManger, context);

            await SeedAdmin2UserAsync(userManager, roleManger, context);
            await SeedSubAdmin2UserAsync(userManager, roleManger, context);

            await SeedIntegratonUserAsync(userManager, roleManger, context);

            await SeedCitizin1UserAsync(userManager, roleManger, context);
            await SeedCitizin2UserAsync(userManager, roleManger, context);
            await SeedCitizin3UserAsync(userManager, roleManger, context);
            await SeedCitizin4UserAsync(userManager, roleManger, context);
            await SeedCitizin5UserAsync(userManager, roleManger, context);
            await SeedCitizin6UserAsync(userManager, roleManger, context);
            await SeedCitizin7UserAsync(userManager, roleManger, context);
            await SeedCitizin8UserAsync(userManager, roleManger, context);
            await SeedCitizin9UserAsync(userManager, roleManger, context);
            await SeedCitizin10UserAsync(userManager, roleManger, context);

            await SeedEmployee1UserAsync(userManager, roleManger, context);
            await SeedEmployee2UserAsync(userManager, roleManger, context);
            //await SeedEmployee3UserAsync(userManager, roleManger, context);
            await SeedEmployee4UserAsync(userManager, roleManger, context);
            await SeedEmployee5UserAsync(userManager, roleManger, context);
            //await SeedEmployee6UserAsync(userManager, roleManger, context);
            await SeedEmployee7UserAsync(userManager, roleManger, context);
            await SeedEmployee8UserAsync(userManager, roleManger, context);
            //await SeedEmployee9UserAsync(userManager, roleManger, context);



        }

        public static async Task SeedIntegratonUserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("6b4dfc44-508e-45e1-9f12-ab96b5aec1c4"),
                UserName = "IntegrationUser",
                Email = "integrationuser@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.OnlyView.ToString() });
                await context.SaveChangesAsync();

            }

            //await roleManger.AddPermissionClaimsForUser(Roles.OnlyView.ToString(), context);

        }

        public static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("c30016d7-556d-4a55-a4f0-ef9e5055a113"),
                UserName = "admin",
                Email = "superadmin@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }

        public static async Task SeedAdmin1UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("5b08a219-9ae2-4814-9f79-e070aaeee6c5"),
                UserName = "admin1",
                Email = "admin1@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }


        public static async Task SeedCitizin1UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("04540979-8ae2-489f-bf90-d59790d7204e"),
                ReferenceId = Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                UserName = "1234567890",
                Email = "Citizin1@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin2UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("fdcb7911-c808-4225-aae9-4900915df16f"),
                UserName = "1120528482",
                ReferenceId = Guid.Parse("04540979-8ae2-489f-bf90-d59790d7204e"),
                Email = "Citizin2@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin3UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("625aee71-3f9d-40d8-9ce2-a3e6f961cb3a"),
                UserName = "3456789012",
                ReferenceId = Guid.Parse("89daceef-7cda-47a0-836c-eccb57cfe933"),
                Email = "Citizin3@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin4UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("f1efed0e-f79e-4d9c-9b75-d2d5b986c230"),
                UserName = "4567890123",
                ReferenceId = Guid.Parse("57008f08-3002-487d-bc6b-83ca78b56f4b"),
                Email = "Citizin4@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin5UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("92a45117-e62a-481b-8977-f0f7657ea55a"),
                UserName = "5678901234",
                Email = "Citizin5@domain.com",
                ReferenceId = Guid.Parse("37fe42f5-e8c2-4c6d-a560-141637abddee"),
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin6UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("ff46d1af-41dd-409a-ba37-2be84f2c0f19"),
                UserName = "6789012345",
                ReferenceId = Guid.Parse("38c675cb-ae4f-42a4-af22-ca6fdd0b3cee"),
                Email = "Citizin6@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin7UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("62ce258b-bd3b-4e81-bf73-0b873ea5ca7b"),
                UserName = "7890123456",
                ReferenceId = Guid.Parse("0518066f-8555-4036-bb01-66398ab56a12"),
                Email = "Citizin7@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin8UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("b1e0393c-03cc-418b-97bc-268edf7879b0"),
                UserName = "8901234567",
                ReferenceId = Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                Email = "Citizin8@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin9UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("df24ed45-0534-4185-a3d3-bc865cec8372"),
                UserName = "9012345678",
                ReferenceId = Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                Email = "Citizin9@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedCitizin10UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("5f992e36-ab66-468e-8041-ba54cd0ab179"),
                UserName = "0123456789",
                ReferenceId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                Email = "Citizin10@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }


        public static async Task SeedEmployee1UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("a0c57bde-d7bf-49ca-9662-9665ef3fc835"),
                UserName = "coadmin",
                Email = "Employee1@domain.com",
                ReferenceId = Guid.Parse("d4430015-c883-413d-b57a-29b5fc6c07c8"),
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedEmployee2UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("b0735375-8b63-4709-93a8-037eead509cf"),
                ReferenceId = Guid.Parse("7f14c6d6-8b17-4749-9c60-a07e191bc4f1"),
                UserName = "coemp1",
                Email = "Employee2@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        //public static async Task SeedEmployee3UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        //{
        //    var defaultUser = new User
        //    {
        //        Id = Guid.Parse("1fda6687-66c7-48ae-9d2e-21b8465596eb"),
        //        ReferenceId = Guid.Parse("9d77c383-c2dd-40f5-b8b8-ff6aeacdd80b"),
        //        UserName = "coemp2",
        //        Email = "Employee3@domain.com",
        //        EmailConfirmed = true,
        //    };

        //    var user = await userManager.FindByEmailAsync(defaultUser.Email);

        //    if (user == null)
        //    {
        //        await userManager.CreateAsync(defaultUser, "123456");
        //        await context.SaveChangesAsync();

        //        await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
        //        await context.SaveChangesAsync();

        //    }

        // await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        // }
        public static async Task SeedEmployee4UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("998bb126-eb41-4a6a-a039-e85795c5dcbf"),
                ReferenceId = Guid.Parse("301511f5-9025-4235-a724-94e8be2e07a6"),
                UserName = "aradmin",
                Email = "Employee4@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedEmployee5UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("4bf8345e-16d7-43ef-b263-935dee667d96"),
                ReferenceId = Guid.Parse("8ccace67-f2aa-447d-9cae-054eda040ca4"),
                UserName = "aremp1",
                Email = "Employee5@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        //public static async Task SeedEmployee6UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        //{
        //    var defaultUser = new User
        //    {
        //        Id = Guid.Parse("d6adf582-0498-4d52-951e-4d42be7c2502"),
        //        ReferenceId = Guid.Parse("c969e6af-dca4-4cba-8221-4c8f60c6c67b"),
        //        UserName = "aremp2",
        //        Email = "Employee6@domain.com",
        //        EmailConfirmed = true,
        //    };

        //    var user = await userManager.FindByEmailAsync(defaultUser.Email);

        //    if (user == null)
        //    {
        //        await userManager.CreateAsync(defaultUser, "123456");
        //        await context.SaveChangesAsync();

        //        await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
        //        await context.SaveChangesAsync();

        //    }

        //    await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        //}

        public static async Task SeedEmployee7UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("17840ab6-b134-4c5f-9273-5c97345010a3"),
                ReferenceId = Guid.Parse("17840ab6-b134-4c5f-9273-5c97345010a3"),
                UserName = "readmin",
                Email = "reEmployee1@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }
        public static async Task SeedEmployee8UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("e4c57d8c-86db-49e6-8f7c-a9948a6f860e"),
                ReferenceId = Guid.Parse("e4c57d8c-86db-49e6-8f7c-a9948a6f860e"),
                UserName = "reemp1",
                Email = "reEmployee2@domain.com",
                EmailConfirmed = true,
            };
            
            //var defaultUser2 = new User
            //{
            //    Id = Guid.Parse("f83c927d-a5ee-45ca-b95b-be75d09c1e51"),
            //    ReferenceId = Guid.Parse("ddc90d42-62b3-4a07-b20a-68b0365aec45"),
            //    UserName = "reemp2",
            //    Email = "reEmployee3@domain.com",
            //    EmailConfirmed = true,
            //};

            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            //var user2 = await userManager.FindByEmailAsync(defaultUser2.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.Employee.ToString() });
                await context.SaveChangesAsync();

            }
            
            //if (user2 == null)
            //{
            //    await userManager.CreateAsync(defaultUser2, "123456");
            //    await context.SaveChangesAsync();

            //    await userManager.AddToRolesAsync(defaultUser2, new List<string> { Roles.Employee.ToString() });
            //    await context.SaveChangesAsync();

            //}

            await roleManger.AddPermissionClaimsForUser(Roles.Employee.ToString(), context);

        }
        //public static async Task SeedEmployee9UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        //{
        //    var defaultUser = new User
        //    {
        //        Id = Guid.Parse("1be718d3-aa47-429d-a5fe-c8a064462ee0"),
        //        ReferenceId = Guid.Parse("66855f0e-ffb7-49ae-a831-516d93822e11"),

        //        UserName = "reemp2",
        //        Email = "reEmployee3@domain.com",
        //        EmailConfirmed = true,
        //    };

        //    var user = await userManager.FindByEmailAsync(defaultUser.Email);

        //    if (user == null)
        //    {
        //        await userManager.CreateAsync(defaultUser, "123456");
        //        await context.SaveChangesAsync();

        //        await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
        //        await context.SaveChangesAsync();

        //    }

        // await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        // }




        public static async Task SeedSubAdmin1UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("c1c3cfb1-c22d-4a2b-b036-6056443a61e8"),
                UserName = "subadmin1",
                Email = "subadmin1@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }

        public static async Task SeedAdmin2UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("afc6cdd8-1ec2-4abb-a34d-828de40bc308"),
                UserName = "admin2",
                Email = "admin2@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }

        public static async Task SeedSubAdmin2UserAsync(UserManager<User> userManager, RoleManager<Role> roleManger, IdentityContext context)
        {
            var defaultUser = new User
            {
                Id = Guid.Parse("66e5c034-eca3-448e-96d9-c4a2a0b946fe"),
                UserName = "subadmin2",
                Email = "subadmin2@domain.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await context.SaveChangesAsync();

                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.SuperAdmin.ToString() });
                await context.SaveChangesAsync();

            }

            await roleManger.AddPermissionClaimsForUser(Roles.SuperAdmin.ToString(), context);

        }

        #region Permission Seed

        public static async Task AddPermissionClaimsForUser(this RoleManager<Role> roleManager, string roleName, IdentityContext context)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GenerateAllPermissions();
            if (roleName == Roles.OnlyView.ToString())
                allPermissions = allPermissions.Where(x => x.Contains(Policy.View)).ToList();

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == Permissions.PolicyType && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim(Permissions.PolicyType, permission));
            }
            await context.SaveChangesAsync();

        }
        #endregion
    }
}
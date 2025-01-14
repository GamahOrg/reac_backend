using Core.Security.Entities;
using Microsoft.AspNetCore.Identity;

namespace REAC.EntityFrameworkCore.Seed
{
    public enum Roles
    {
        Employee,
        SuperAdmin,
        OnlyView,

        // New Roles

        ReEmployee,
        ReAdmin,
        CoAdmin,
        Reformer,
        ArAdmin,
        Arbitrator,
        Citizen
    }
    public static class DefaultRolesCreator
    {
        /// <summary>
        /// Seed default admin roles
        /// </summary>
        /// <param name="roleManger"></param>
        /// <returns></returns>
        public static async Task CreateAsync(RoleManager<Role> roleManger)
        {
            if (!roleManger.Roles.Any(x => x.Name == Roles.SuperAdmin.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.SuperAdmin.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.OnlyView.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.OnlyView.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            

            // New Roles
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
            if (!roleManger.Roles.Any(x => x.Name == Roles.Employee.ToString()))
            {
                await roleManger.CreateAsync(new Role { Name = Roles.Employee.ToString() });
            }
        }
    }
}
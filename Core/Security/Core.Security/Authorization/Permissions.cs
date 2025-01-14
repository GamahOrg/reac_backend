using Core.Shared.Extentions;

namespace Core.Security.Authorization
{
    public static class Permissions
    {
        //public const string PolicyPrefix = "Permissions";
        public const string PolicyType = "Permission";
        public const string RoleType = "Role";
        /// <summary>
        /// Get all permissions for one module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static List<string> GenerateModulePermissions(string module)
        {
            List<string> permissionsList = new List<string>();
            
            foreach (var policy in typeof(Policy).GetAllFieldsValues())
                permissionsList.Add($"{module}.{policy.Value}");

            return permissionsList;
        }

        /// <summary>
        /// Get all permissions for all modules
        /// </summary>
        /// <returns></returns>
        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();

            var modules = typeof(Modules).GetAllFieldsValues();

            foreach (var module in modules)
                allPermissions.AddRange(GenerateModulePermissions(module.Value));

            return allPermissions;
        }

        /// <summary>
        /// Get all policies only
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPoliciesList()
        {
            List<string> policiesList = new List<string>();

            foreach (var policy in typeof(Policy).GetAllFieldsValues())
                policiesList.Add(policy.Value);

            return policiesList;
        }
        
    }
}
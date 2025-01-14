namespace Core.Security.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class PermissionRequiredAttribute : Attribute
    {
        public PermissionRequiredAttribute(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }
    }

}

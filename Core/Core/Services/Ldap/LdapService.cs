using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Core.Services.Ldap
{
    public class LdapService : ILdapService
    {
        private string LdapPath { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }

        public LdapService(string ldapPath, string userName, string password)
        {
            LdapPath = ldapPath;
            UserName = userName;
            Password = password;
        }

        public bool UserExist(string username)
        {
            using var domain = new PrincipalContext(ContextType.Domain, LdapPath, UserName, Password);
            var foundUser =
                UserPrincipal.FindByIdentity(domain, IdentityType.UserPrincipalName, username);
            return foundUser is { Enabled: true };
        }

        public bool Login(string username, string password)
        {
            return Validate(username, password);
        }

        private bool Validate(string username, string password)
        {
            try
            {
                using PrincipalContext pc = new PrincipalContext(ContextType.Domain, LdapPath, UserName, Password);
                return pc.ValidateCredentials(username, password);
            }
            catch (DirectoryServicesCOMException ex)
            {
                return false;
            }
        }
    }
}
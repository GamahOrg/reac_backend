namespace Core.Services.Ldap
{
    public interface ILdapService
    {
        bool Login(string username, string password);
        bool UserExist(string username);
    }
}

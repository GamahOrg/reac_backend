namespace Core.Log.Domain.Shared.Enums
{
    [Flags]
    public enum LoggerType
    {
        Console = 1,
        File = 2,
        Database = 3,
        Email = 4,
        SMS = 5,
        Default = Console | File | Database,
        All = Console | File | Database | Email | SMS
    }
}

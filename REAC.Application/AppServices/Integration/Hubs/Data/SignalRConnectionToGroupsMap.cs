using System.Collections.Concurrent;

namespace REAC.Application.AppServices.Integration.Hubs.Data

{
    public static class SignalRConnectionToGroupsMap
    {
        private static readonly ConcurrentDictionary<string, string> Groups = new ConcurrentDictionary<string, string>();

        public static void AddGroup(string groupName, string connectionId)
        {
            Groups.TryAdd(groupName, connectionId);
        }

        public static void RemoveGroup(string groupName)
        {
            Groups.TryRemove(groupName, out string connectionId);
        }
        public static string? GetGroupConnection(string groupName)
        {
            string? connectionId = null;

            Groups.TryGetValue(groupName, out connectionId);

            return connectionId;
        }

        public static bool IsGroupExist(string groupName)
        {
            return Groups.ContainsKey(groupName);
        }
    }
}

using System.Collections.Generic;

namespace TgVozderzhansBot.Core.TgBot
{
    public static class UserDataContainerManager
    {
        private static Dictionary<long, UserDataContainer> _containers = new Dictionary<long, UserDataContainer>();
        
        public static UserDataContainer GetOrCreate(long userId)
        {
            if (!_containers.ContainsKey(userId))
            {
                _containers[userId] = new UserDataContainer(userId);
            }

            return _containers[userId];
        }
    }
}
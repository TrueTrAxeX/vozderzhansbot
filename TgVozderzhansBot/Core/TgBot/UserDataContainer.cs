using System.Collections.Generic;

namespace TgVozderzhansBot.Core.TgBot
{
    public class UserDataContainer
    {
        private long _userId;
        
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();
        
        public UserDataContainer(long userId)
        {
            _userId = userId;
        }

        public T Get<T>(string key) where T : class
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key] as T;
            }
            else
            {
                return null;
            }
        }

        public T GetStruct<T>(string key) where T : struct
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key] is T ? (T) _dict[key] : default;
            }
            else
            {
                return default(T);
            }
        }

        public void SetStruct<T>(string key, T value) where T : struct
        {
            _dict[key] = value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            _dict[key] = value as T;
        }
    }
}
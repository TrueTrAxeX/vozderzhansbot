using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    public enum Type
    {
        Message, Callback
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class HandleOnlyFromGroup : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandType : Attribute
    {
        public Type Type { get; set; }
        
        public CommandType(Type type)
        {
            Type = type;
        }
    }
    
    public abstract class CommandHandler
    {
        private Type? _supportedType;
        protected TelegramBotClient Client;
        
        public CommandHandler(TelegramBotClient client)
        {
            Client = client;
            
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(this.GetType());  // Reflection.  

            _supportedType = attrs.Select(x => (x as CommandType)?.Type).FirstOrDefault();
        }

        public bool AllowNext(Update update)
        {
            ChatType? chatType = update?.CallbackQuery?.Message?.Chat?.Type ?? update?.Message?.Chat?.Type;

            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(this.GetType());  // Reflection.  

            var count = attrs.Where(x => x is HandleOnlyFromGroup).Count();

            if (count > 0)
            {
                if (chatType != ChatType.Supergroup) return false;
                else return true;
            }

            if (chatType == ChatType.Supergroup)
            {
                return false;
            }
            
            if (update.Type == UpdateType.Message && _supportedType == Type.Message)
            {
                return true;
            }

            if (update.Type == UpdateType.CallbackQuery && _supportedType == Type.Callback)
            {
                return true;
            }

            if (_supportedType == null)
            {
                return true;
            }

            return false;
        }
         
        public abstract bool CanHandle(Update update);
        
        public abstract Task Handle(Update update);
    }
}
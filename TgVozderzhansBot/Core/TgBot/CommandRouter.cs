using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.TgBot.CommandHandlers;

namespace TgVozderzhansBot.Core.TgBot
{
    public class CommandRouter
    {
        private List<CommandHandler> _handlers;
        private TelegramBotClient _client;
        
        public CommandRouter(ref List<CommandHandler> handlers, TelegramBotClient client)
        {
            _client = client;
            _handlers = handlers;
            
            _client.OnUpdate += (sender, args) =>
            {
                try
                {
                    Update update = args.Update;

                    if (update.Type == UpdateType.Message && args.Update.Message != null)
                    {
                        UserRepository userRepository = new UserRepository();

                        var from = args.Update.Message?.From;
                        string username = from?.Username ?? "user_"+from?.Id.ToString();
                        
                        userRepository.UpdateNicknameIfDifferent(args.Update.Message.Chat.Id, username.Replace("@", ""));
                    }
                   
                    Handle(update).Wait();
                }
                catch
                {
                    Console.WriteLine("Не удалось обработать команду");
                }
            };
        }

        public async Task Handle(Update update)
        {
            foreach (CommandHandler handler in _handlers)
            {
                if (handler.AllowNext(update))
                {
                    if (handler.CanHandle(update))
                    {
                        try
                        {
                            await handler.Handle(update);
                            return;
                        }
                        catch
                        {
                            Console.WriteLine("Не удалось обработать команду");
                        }
                    }
                }
            }
        }
    }
}
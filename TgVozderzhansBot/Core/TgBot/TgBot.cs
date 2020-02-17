using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using TgVozderzhansBot.Core.TgBot.CommandHandlers;
using File = System.IO.File;
using Type = System.Type;

namespace TgVozderzhansBot.Core.TgBot
{
    public class TgBot
    {
        private readonly TelegramBotClient _client;

        public TelegramBotClient TelegramBotClient => _client;
        
        private readonly List<CommandHandler> _handlers = new List<CommandHandler>();

        private readonly CommandRouter _router;

        private string ReadBotApiToken()
        {
            try
            {
                return File.ReadAllText("./Data/BotApiToken.txt").Trim();
            }
            catch(IOException e)
            {
                Console.WriteLine("Не удалось прочитать API токен бота");

                return null;
            }
        }
        
        public TgBot()
        {
            _client = new TelegramBotClient(ReadBotApiToken());

            _router = new CommandRouter(ref _handlers, _client);
        }

        public void AddCommandHandlers(params Type[] handlers)
        {
            foreach (Type t in handlers)
            {
                if (t.IsSubclassOf(typeof(CommandHandler)))
                {
                    _handlers.Add(Activator.CreateInstance(t, _client) as CommandHandler);
                }
            }
        }

        public void Start()
        {
            _client.StartReceiving();
        }
    }
}
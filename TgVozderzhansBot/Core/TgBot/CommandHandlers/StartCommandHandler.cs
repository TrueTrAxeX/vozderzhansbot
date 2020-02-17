using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Message)]
    public class StartCommandHandler : CommandHandler
    {
        public override bool CanHandle(Update update)
        {
            if (update.Message.Text == "/start")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            if (update.Message.From.Username == null || update.Message.From.Username.Length < 2)
            {
                await Client.SendTextMessageAsync(update.Message.Chat.Id,
                    "Чтобы пользовать ботом, сначала сделайте себе логин, который начинается с @. Это можно сделать в настройках Telegram");
                return;
            }
            
            UserRepository userRepository = new UserRepository();
            
            userRepository.CreateUserIfNOtExists(new Models.User
            {
                Username = update.Message.From.Username,
                ChatId = update.Message.Chat.Id,
                CreatedAt = DateTime.Now
            });
            
            await Client.SendTextMessageAsync(update.Message.Chat.Id, "Приветствуем Вас! Этот бот поможет вам вести свой дневник воздержания", replyMarkup: new ReplyKeyboardMarkup()
            {
                Keyboard = new []{ new List<KeyboardButton>()
                {
                    new KeyboardButton("💪 Мое воздержание"),
                    new KeyboardButton("〽️TOP пользователей")
                },
                    new List<KeyboardButton>()
                    {
                        new KeyboardButton("⚙️Настройки")
                    }
                },
                ResizeKeyboard = true
            });
        }

        public StartCommandHandler(TelegramBotClient client) : base(client)
        {
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    public class FriendsMenuCommandHandler : CommandHandler
    {
        public FriendsMenuCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Type == UpdateType.Message && update.Message.Text.EndsWith("Мои друзья"))
            {
                return true;
            }
            
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data == "back_to_friend_menu")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            
            buttons.Add(new InlineKeyboardButton
            {
                Text = "Добавить друга",
                CallbackData = "add_new_friend"
            });

            buttons.Add(new InlineKeyboardButton
            {
                Text = "Список друзей",
                CallbackData = "get_friend_list"
            });

            if (update.CallbackQuery?.Data == "back_to_friend_menu")
            {
                await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Выберите пункт меню:", ParseMode.Html, replyMarkup: 
                    new InlineKeyboardMarkup(buttons.Select(x => new InlineKeyboardButton[] {x}).ToArray()));
                return;
            }
            
            await Client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите пункт меню:", ParseMode.Html, replyMarkup: 
                new InlineKeyboardMarkup(buttons.Select(x => new InlineKeyboardButton[] {x}).ToArray()));
        }
    }
}
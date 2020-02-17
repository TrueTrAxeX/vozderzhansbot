using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Message)]
    public class SettingsCommandHandler : CommandHandler
    {
        public SettingsCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Message.Text.EndsWith("Настройки"))
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
                Text = "👤 Статус аккаунта",
                CallbackData = "select_status_menu"
            });

            await Client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите подходящую настройку:", replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }
}
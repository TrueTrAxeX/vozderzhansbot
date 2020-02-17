using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Message)]
    public class MyAbsistenceCommandHandler : CommandHandler
    {
        public MyAbsistenceCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Message.Text.EndsWith("Мое воздержание"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.Message.Chat.Id);

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            
            if (absItemRepository.HasAbs(userId))
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "〽️Текущий срок",
                    CallbackData = "current_term"
                });
                
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "❌ Прервать воздержание",
                    CallbackData = "stop_abs"
                });
            }
            else
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Начать воздерживаться",
                    CallbackData = "start_abs"
                });
            }

            bool hasHistory = absItemRepository.HasHistory(userId);

            if (hasHistory)
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "История срывов",
                    CallbackData = "get_history"
                });
            }
            
            InlineKeyboardButton[][] btnMatrix = buttons.Select(x => new InlineKeyboardButton[] {x}).ToArray();

            await Client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите пункт меню:", 
                replyMarkup: new InlineKeyboardMarkup(btnMatrix));
        }
    }
}
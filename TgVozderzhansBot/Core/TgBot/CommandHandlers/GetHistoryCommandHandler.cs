using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class GetHistoryCommandHandler : CommandHandler
    {
        public GetHistoryCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "get_history")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();

            var userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            IEnumerable<AbsItem> histories = absItemRepository.GetHistory(userId, 10);

            string txt = "<b>Лог воздержания:</b>\n\n";

            int pos = 1;
            
            foreach (AbsItem interval in histories)
            {
                TimeSpan span = interval.FinishedAt.Value - interval.DateFrom;
                
                txt += pos++ + ". " + interval.DateFrom.ToString("yyyy-MM-dd") + " - " + interval.FinishedAt?.ToString("yyyy-MM-dd") + $" (<b>{(int)span.TotalDays} дней</b>)\n";
            }

            await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, txt, ParseMode.Html);

            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers.FromGroups
{
    [CommandType(Type.Message)]
    [HandleOnlyFromGroup]
    public class GetTopUsersCommandHandler : CommandHandler
    {
        public GetTopUsersCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Message.Text == "/top")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();
            
            var topUsers = absItemRepository.GetTopUsers(0, 10).ToList();

            string txt = "TOP воздержанцев чата:\n\n";

            int pos = 1;
            
            foreach (dynamic user in topUsers)
            {
                DateTime dateFrom = user.DateFrom;
                TimeSpan span = DateTime.Now - dateFrom;
                
                txt += pos++ + $". <b>@{user.Username}</b> - <b>{(int)span.TotalDays}</b> дней\n";
            }

            if (topUsers.Count == 0)
            {
                txt += "<i>Не найдено ни одного пользователя</i>";
            }

            await Client.SendTextMessageAsync(update.Message.Chat.Id, txt, ParseMode.Html, replyToMessageId: update.Message.MessageId);
        }
    }
}
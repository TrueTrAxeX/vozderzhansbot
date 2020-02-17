using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers.FromGroups
{
    [CommandType(Type.Message)]
    [HandleOnlyFromGroup]
    public class GetMyTermInfoCommandHandler : CommandHandler
    {
        public GetMyTermInfoCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Message.Text == "/myterm")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();

            DateTime? dateFrom = absItemRepository.GetCurrentTermByNickname(update.Message.From.Username.Replace("@", ""));

            if (dateFrom == null)
            {
                await Client.SendTextMessageAsync(update.Message.Chat.Id, "Вы не воздерживаетесь в данный момент. Зайдите в бота, и начните воздержание",
                    replyToMessageId: update.Message.MessageId);
                return;
            }
            
            TimeSpan span = DateTime.Now - dateFrom.Value;
            
            var str = String.Format ("{0:00} дней, {1:00} часов, {2:00} минут, {3:00} секунд", span.Days, span.Hours, span.Minutes, span.Seconds); 

            await Client.SendTextMessageAsync(update.Message.Chat.Id, $"Вы воздерживаетесь <b>{str}</b>.", replyToMessageId: update.Message.MessageId, parseMode: ParseMode.Html);
        }
    }
}
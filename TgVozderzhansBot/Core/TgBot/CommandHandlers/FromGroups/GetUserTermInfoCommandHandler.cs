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
    public class GetUserTermInfoCommandHandler : CommandHandler
    {
        public GetUserTermInfoCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Message.Text.StartsWith("/term @"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            string nickname = update.Message.Text.Split("/term ")[1].Replace("@", "");
            
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();
            
            long? userId = userRepository.GetPublicUserIdByNickname(nickname);

            if (userId == null)
            {
                await Client.SendTextMessageAsync(update.Message.Chat.Id, "Пользователь имеет приватный статус аккаунта, доступ к его статистике невозможен.",
                    replyToMessageId: update.Message.MessageId);
                return;
            }

            DateTime dateFrom = absItemRepository.CurrentTerm(userId.Value).Value;
            
            TimeSpan span = DateTime.Now - dateFrom;
            
            var str = String.Format ("{0:00} дней, {1:00} часов, {2:00} минут, {3:00} секунд", span.Days, span.Hours, span.Minutes, span.Seconds); 

            await Client.SendTextMessageAsync(update.Message.Chat.Id, $"Пользователь @{nickname} воздерживается: <b>{str}</b>.", replyToMessageId: update.Message.MessageId, parseMode: ParseMode.Html);
        }
    }
}
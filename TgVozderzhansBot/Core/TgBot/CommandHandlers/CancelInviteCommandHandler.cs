using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class CancelInviteCommandHandler : CommandHandler
    {
        public CancelInviteCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("cancel_invite"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id,
                update.CallbackQuery.Message.MessageId);
        }
    }
}
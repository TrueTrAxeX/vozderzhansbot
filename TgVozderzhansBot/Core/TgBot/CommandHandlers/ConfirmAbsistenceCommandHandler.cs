using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class ConfirmAbsistenceCommandHandler : CommandHandler
    {
        public ConfirmAbsistenceCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("confirm_absistence"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            long id = long.Parse(update.CallbackQuery.Data.Split(":")[1]);

            if (WarningsRepository.SetConfirmed(id))
            {
                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Операция выполнена");

                await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
            }
        }
    }
}
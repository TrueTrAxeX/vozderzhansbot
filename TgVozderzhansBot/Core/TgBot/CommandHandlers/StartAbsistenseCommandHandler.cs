using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class StartAbsistenseCommandHandler : CommandHandler
    {
        public StartAbsistenseCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "start_abs")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();
            
            bool success = absItemRepository.StartAbsistence(update.CallbackQuery.Message.Chat.Id);

            if (success)
            {
                await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                
                await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Поздравляем, вы только что начали свое воздержание");
            }
            
            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
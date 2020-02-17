
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class AddDaysCommandHandlers : CommandHandler
    {
        public AddDaysCommandHandlers(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "cancel_add_days")
            {
                return true;
            }
            
            if (update.CallbackQuery.Data == "add_abs_days")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);
            
            var container = UserDataContainerManager.GetOrCreate(userId);

            if (update.CallbackQuery.Data == "cancel_add_days")
            {
                container.Set<string>("wait_input", null);

                await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
            }
            else
            {
                bool allow = userRepository.IsAllowToAddDays(userId);

                if (!allow)
                {
                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Операция недоступна", true);
                    return;
                }
                
                string txt = "Введите количество дней, которое вы хотите добавить к своему воздержанию. <b>Внимание!</b> Операцию можно проделать только один раз.";
 
                container.Set<string>("wait_input", "add_days");

                await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, txt, replyMarkup: new InlineKeyboardMarkup(
                    new InlineKeyboardButton
                    {
                        Text = "Отмена",
                        CallbackData = "cancel_add_days"
                    }), parseMode: ParseMode.Html);

                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }
    }
}
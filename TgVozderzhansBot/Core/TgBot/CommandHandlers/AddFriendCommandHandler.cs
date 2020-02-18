using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class AddFriendCommandHandler : CommandHandler
    {
        public AddFriendCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "add_new_friend")
            {
                return true;
            }

            if (update.CallbackQuery.Data == "add_friend_cancel")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        { 
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            var cont = UserDataContainerManager.GetOrCreate(userId);

            if (update.CallbackQuery.Data == "add_friend_cancel")
            {
                cont.Set<string>("wait_input", null);
                await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Отменено");
                return;
            }
           
            cont.Set("wait_input", "add_friend");
            
            await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                "Введите имя пользователя, которого собираетесь добавить в друзья. Имя должно начинаться с знака @ (собака). Например, @BatMoped",
                replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                {
                    Text = "Отмена",
                    CallbackData = "add_friend_cancel"
                }));

            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Workers;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class StopAbsistenceCommandHandler : CommandHandler
    {
        public StopAbsistenceCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("stop_abs"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();
            
            if (update.CallbackQuery.Data == "stop_abs")
            {
                await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                    "Вы точно хотите сорваться? Если так, то подтвердите свой срыв", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                    {
                        Text = "Подтверждаю свой срыв",
                        CallbackData = "stop_abs_confirm"
                    }));
            }

            if (update.CallbackQuery.Data == "stop_abs_confirm")
            {
                await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                UserRepository userRepository = new UserRepository();
                
                long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

                var user = userRepository.GetUserById(userId);
                
                absItemRepository.StopAbsistence(userId);

                var allFriends = FriendsRepository.GetFriends(userId);

                foreach (var f in allFriends)
                {
                    MassMessagePoster.Add(f.ChatId, $"Оповещаем вас, что ваш друг с ником @{user.Username} сорвался.");
                }
                
                await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                    "Ваш срыв учтен, вы можете начать воздерживаться заново");
            }

            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
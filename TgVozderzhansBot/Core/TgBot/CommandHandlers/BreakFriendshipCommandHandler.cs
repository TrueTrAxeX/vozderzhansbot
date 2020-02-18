using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Workers;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class BreakFriendshipCommandHandler : CommandHandler
    {
        public BreakFriendshipCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("break_friendship"))
            {
                return true;
            }

            if (update.CallbackQuery.Data.StartsWith("break_friendship_confirm"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            long friendId = long.Parse(update.CallbackQuery.Data.Split(":")[1]);
            
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            var user = userRepository.GetUserById(userId);
            var friend = userRepository.GetUserById(friendId);

            if (update.CallbackQuery.Data.StartsWith("break_friendship_confirm"))
            {
                bool success = FriendsRepository.RemoveFriend(userId, friendId);

                if (success)
                {
                    MassMessagePoster.Add(friend.ChatId, $"Пользователь @{user.Username} завершил дружбу с вами! Не обижайтесь на него, он не ведает что творит.");
                    
                    await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId);

                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Вы успешно оборвали дружбу", true);
                }
                else
                {
                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Произошла ошибка! Попробуйте еще раз", true);
                }
                
                return;
            }
            
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            
            buttons.Add(new InlineKeyboardButton
            {
                Text = "Подтверждаю",
                CallbackData = $"break_friendship_confirm:{friend.Id}"
            });
            
            buttons.Add( new InlineKeyboardButton
            {
                CallbackData = $"e_select_friend:{friend.Id}",
                Text = "← Назад"
            });
            
            await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                update.CallbackQuery.Message.MessageId, $"Вы точно хотите разорвать дружбу с @{friend.Username}?", replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }
}
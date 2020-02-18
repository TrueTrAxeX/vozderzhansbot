using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class FriendListCommandHandler : CommandHandler
    {
        public FriendListCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "get_friend_list")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {  
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();

            var userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            List<Models.User> users = FriendsRepository.GetFriends(userId);

            foreach (Models.User user in users)
            {
                buttons.Add(new InlineKeyboardButton
                {
                    Text = $"{user.Username}",
                    CallbackData = $"select_friend:{user.Id}"
                });
            }

            buttons.Add(new InlineKeyboardButton
            {
                Text = "← Вернуться назад",
                CallbackData = "back_to_friend_menu"
            });

            string txt = "Список друзей:";

            if (users.Count == 0)
            {
                txt += "\n\n<i>У вас пока еще нет друзей</i>";
            }
            
            await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, txt, 
                replyMarkup: new InlineKeyboardMarkup(buttons.Select(x => new [] {x}).ToArray()), parseMode: ParseMode.Html);

            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Message)]
    public class AddFriendInputCommandHandler : CommandHandler
    {
        public AddFriendInputCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.Message.Chat.Id);
            
            var container = UserDataContainerManager.GetOrCreate(userId);

            string waitInput = container.Get<string>("wait_input");

            if (waitInput != null && waitInput == "add_friend")
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();

            long userId = userRepository.GetUserIdByChatId(update.Message.Chat.Id);
            
            var container = UserDataContainerManager.GetOrCreate(userId);

            string waitInput = container.Get<string>("wait_input");

            if (waitInput == "add_friend")
            {
                string txt = update.Message.Text;

                if (txt.StartsWith("@"))
                {
                    string friendNickname = txt.Substring(1);

                    long? friendId = userRepository.GetUserIdByNickname(friendNickname);

                    if (friendId == null)
                    {
                        await Client.SendTextMessageAsync(update.Message.Chat.Id, "Пользователь не найден, попробуйте ввести другой ник");
                        return;
                    }

                    if (FriendsRepository.HasFriend(userId, friendId.Value))
                    {
                        await Client.SendTextMessageAsync(update.Message.Chat.Id,
                            "Этот пользователь уже является вашим другом");
                        
                        container.Set<string>("wait_input", null);
                        
                        return;
                    }

                    await this.SendInviteToFriend(friendId.Value, userId);

                    await Client.SendTextMessageAsync(update.Message.Chat.Id,
                        "Приглашение в друзья отослано. Вам придет сообщение, когда пользователь подтвердит дружбу");
                    
                    container.Set<string>("wait_input", null);
                }
            }
        }

        private async Task SendInviteToFriend(long friendId, long currentUserId)
        {
            UserRepository userRepository = new UserRepository();

            Models.User friend = userRepository.GetUserById(friendId);
            Models.User user = userRepository.GetUserById(currentUserId);

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            
            buttons.Add(new InlineKeyboardButton
            {
                Text = "Подтвердить",
                CallbackData = $"accept_invite:{currentUserId}"
            });
            
            buttons.Add(new InlineKeyboardButton
            {
                Text = "Отменить",
                CallbackData = $"cancel_invite:{currentUserId}"
            });
            
            await Client.SendTextMessageAsync(friend.ChatId, 
                $"Пользователь @{user.Username} хочет добавить вас в друзья", replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }
}
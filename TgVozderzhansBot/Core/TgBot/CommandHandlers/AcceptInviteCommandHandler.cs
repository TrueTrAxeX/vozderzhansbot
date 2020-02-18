using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class AcceptInviteCommandHandler : CommandHandler
    {
        public AcceptInviteCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("accept_invite"))
            {
                return true;
            }

            if (update.CallbackQuery.Data.StartsWith("cancel_invite"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();
            
            if (update.CallbackQuery.Data.StartsWith("accept_invite"))
            {
                long friendId = long.Parse(update.CallbackQuery.Data.Split(":")[1]);
                long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

                Models.User friend = userRepository.GetUserById(friendId);
                Models.User user = userRepository.GetUserById(userId);

                if (!FriendsRepository.HasFriend(userId, friendId))
                {
                    Friend friendShip1 = new Friend
                    {
                        CreatedAt = DateTime.Now,
                        FriendId = friendId,
                        UserId = userId
                    };
                
                    Friend friendShip2 = new Friend
                    {
                        CreatedAt = DateTime.Now,
                        FriendId = userId,
                        UserId = friendId
                    };

                    FriendsRepository.Insert(friendShip1);
                    FriendsRepository.Insert(friendShip2);

                    await Client.SendTextMessageAsync(friend.ChatId, $"Пользователь @{user.Username} принял вашу дружбу");
                    await Client.SendTextMessageAsync(user.ChatId, $"Вы подружились с @{friend.Username}");

                    await Client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId);
                }

                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }
    }
}
using System;
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
    public class SelectFriendCommandHandler : CommandHandler
    {
        public SelectFriendCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("select_friend"))
            {
                return true;
            }

            if (update.CallbackQuery.Data.StartsWith("e_select_friend"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepostory = new AbsItemRepository();

            long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);
            long friendId = long.Parse(update.CallbackQuery.Data.Split(":")[1]);

            if (FriendsRepository.HasFriend(userId, friendId))
            {
                Models.User friend = userRepository.GetUserById(friendId);

                DateTime? dateFrom = absItemRepostory.CurrentTerm(friendId);
                
                string txt = null;
                
                if (dateFrom != null)
                {
                    TimeSpan span = DateTime.Now - dateFrom.Value;
            
                    var str = String.Format ("{0:00} дней, {1:00} часов, {2:00} минут, {3:00} секунд", span.Days, span.Hours, span.Minutes, span.Seconds); 

                    string rating = "Рейтинг: ";

                    int stars = AbsItemRepository.GetUserStarsCount(friend.Id);
                
                    if (stars == 0) stars = 1;
                
                    for (int i = 0; i < stars; i++)
                    {
                        rating += "⭐";
                    }

                    txt = $"Ваш друг <b>@{friend.Username}</b> воздерживается: <b>{str}</b>\n\n";
                    txt += rating;
                }
                else
                {
                    txt = $"Ваш друг @{friend.Username} в текущее время не воздерживается";
                }
                
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                buttons.Add(new InlineKeyboardButton
                {
                    Text = "👀 История срывов",
                    CallbackData = $"f_breakdown_history:{friend.Id}"
                });
                
                buttons.Add(new InlineKeyboardButton
                {
                    Text = "❌ Разорвать дружбу",
                    CallbackData = $"break_friendship:{friend.Id}"
                });

                if (update.CallbackQuery.Data.StartsWith("e_select_friend"))
                {
                    await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, txt, ParseMode.Html, replyMarkup: 
                        new InlineKeyboardMarkup(buttons.Select(x => new [] {x}).ToArray()));
                }
                else
                {
                    await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, txt, ParseMode.Html, replyMarkup: 
                        new InlineKeyboardMarkup(buttons.Select(x => new [] {x}).ToArray()));
                }
                
                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
            else
            {
                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Пользователь не найден в друзьях");
            }
        }
    }
}
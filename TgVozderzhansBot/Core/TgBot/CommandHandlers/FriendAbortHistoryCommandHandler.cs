using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class FriendAbortHistoryCommandHandler : CommandHandler
    {
        public FriendAbortHistoryCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("f_breakdown_history"))
            {
                return true;
            }

            return false;
        }

        public override async Task Handle(Update update)
        {
            long friendId = long.Parse(update.CallbackQuery.Data.Split(":")[1]);
            
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();

            var userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            if (FriendsRepository.HasFriend(userId, friendId))
            {
                Models.User friend = userRepository.GetUserById(friendId);
                
                IEnumerable<AbsItem> histories = absItemRepository.GetHistory(friendId, 10);

                string txt = $"<b>Лог воздержания пользователя @{friend.Username}:</b>\n\n";

                int pos = 1;
            
                foreach (AbsItem interval in histories)
                {
                    TimeSpan span = interval.FinishedAt.Value - interval.DateFrom;
                
                    txt += pos++ + ". " + interval.DateFrom.ToString("yyyy-MM-dd") + " - " + interval.FinishedAt?.ToString("yyyy-MM-dd") + $" (<b>{(int)span.TotalDays} дней</b>)\n";
                }

                await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, txt, ParseMode.Html, 
                    replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                    {
                        Text = "← Вернуться назад",
                        CallbackData = $"e_select_friend:{friend.Id}"
                    }));

                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }
    }
}
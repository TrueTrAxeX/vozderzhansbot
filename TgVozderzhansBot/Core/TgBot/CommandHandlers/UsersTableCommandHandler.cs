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
    public class UsersTableCommandHandler : CommandHandler
    {
        public UsersTableCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Text.EndsWith("Топ 10 пользователей") || update.Message.Text.EndsWith("TOP пользователей"))
                {
                    return true;
                }

                return false;
            } 
            
            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.StartsWith("users_table_nav"))
                {
                    return true;
                }
            }

            return false;
        }

        private const int ItemsOnPage = 10;
        
        public override async Task Handle(Update update)
        {
            AbsItemRepository absItemRepository = new AbsItemRepository();

            int offset = 0;
            
            long totalUsersCount = absItemRepository.GetTotalUsersCount();

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains("nav"))
                {
                    offset = int.Parse(update.CallbackQuery.Data.Split(":").Last());
                    
                    if (offset < 0) offset = 0;
                }
            }
            
            var topUsers = absItemRepository.GetTopUsers(offset, ItemsOnPage).ToList();

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            if (offset + 1 >= ItemsOnPage)
            {
                int newOffset = offset - ItemsOnPage;
                
                if (newOffset < 0) newOffset = 0;

                buttons.Add(new InlineKeyboardButton
                {
                    Text = "<<",
                    CallbackData = $"users_table_nav:{newOffset}"
                });
            }
            
            if (ItemsOnPage+offset < totalUsersCount)
            {
                buttons.Add(new InlineKeyboardButton
                {
                    Text = ">>",
                    CallbackData = $"users_table_nav:{(offset+ItemsOnPage)}"
                });
            }
            
            string txt = "Список лучших пользователей:\n\n";

            int pos = offset+1;
            
            foreach (dynamic user in topUsers)
            {
                DateTime dateFrom = user.DateFrom;
                TimeSpan span = DateTime.Now - dateFrom;
                
                txt += pos++ + $". <b>@{user.Username}</b> - <b>{(int)span.TotalDays}</b> дней\n";
            }

            if (topUsers.Count == 0)
            {
                txt += "<i>Не найдено ни одного пользователя</i>";
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                try
                {
                    await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        txt, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                }
                catch
                {

                }
                finally
                {
                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                }
                
            }
            else if (update.Type == UpdateType.Message)
            {
                await Client.SendTextMessageAsync(update.Message.Chat.Id, txt, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
            }
        }
    }
}
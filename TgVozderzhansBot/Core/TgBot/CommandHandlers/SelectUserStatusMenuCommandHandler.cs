using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class SelectUserStatusMenuCommandHandler : CommandHandler
    {
        public SelectUserStatusMenuCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data.StartsWith("select_status_menu"))
            {
                return true;
            }

            if (update.CallbackQuery.Data.StartsWith("set_status_public"))
            {
                return true;
            }

            if (update.CallbackQuery.Data.StartsWith("set_status_private"))
            {
                return true;
            }

            return false;
        }

        private string GetUserStatusStr(UserRepository.UserStatus userStatus)
        {
            switch (userStatus)
            {
                case UserRepository.UserStatus.Public:
                    return "Публичный";
                case UserRepository.UserStatus.Private:
                    return "Приватный";
                
                default:
                    return null;
            }
        }

        public override async Task Handle(Update update)
        {
            UserRepository userRepository = new UserRepository();
            long userId = userRepository.GetUserIdByChatId(update.CallbackQuery.Message.Chat.Id);

            if (update.CallbackQuery.Data.StartsWith("set_status_public"))
            {
                if (userRepository.SetUserStatus(userId, UserRepository.UserStatus.Public))
                {
                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Статус установлен как \"Публичный\"");

                    update.CallbackQuery.Data = "select_status_menu_up";
                    await Handle(update);
                }
            }
            else if(update.CallbackQuery.Data.StartsWith("set_status_private"))
            {
                if (userRepository.SetUserStatus(userId, UserRepository.UserStatus.Private))
                {
                    await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id,
                        "Статус установлен как \"Приватный\"");
                    
                    update.CallbackQuery.Data = "select_status_menu_up";
                    await Handle(update);
                }
            }
            else
            {
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            
                buttons.Add(new InlineKeyboardButton
                {
                    Text = "Публичный",
                    CallbackData = "set_status_public"
                });
            
                buttons.Add(new InlineKeyboardButton
                {
                    Text = "Приватный",
                    CallbackData = "set_status_private"
                });
                
                UserRepository.UserStatus status = userRepository.GetUserStatus(userId);

                string statusStr = GetUserStatusStr(status);

                string msgStr = $"Установите свой статус видимости в ТОПе. \n\nТекущий статус: <b>{statusStr}</b>";
                
                if (update.CallbackQuery.Data == "select_status_menu_up")
                {
                    try
                    {
                        await Client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId,
                            msgStr, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                    }
                    catch(MessageIsNotModifiedException e)
                    {
                        
                    }
                    
                }
                else
                {
                    await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                        msgStr, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                }
                
                await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }
    }
}
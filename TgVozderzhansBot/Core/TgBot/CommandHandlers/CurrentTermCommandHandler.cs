using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Callback)]
    public class CurrentTermCommandHandler : CommandHandler
    {
        public CurrentTermCommandHandler(TelegramBotClient client) : base(client)
        {
        }

        public override bool CanHandle(Update update)
        {
            if (update.CallbackQuery.Data == "current_term")
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

            TimeSpan span = DateTime.Now - absItemRepository.CurrentTerm(userId);
            
            var str = String.Format ("{0:00} дней, {1:00} часов, {2:00} минут, {3:00} секунд", span.Days, span.Hours, span.Minutes, span.Seconds);

            string rating = "Ваш рейтинг: ";

            int stars = AbsItemRepository.GetUserStarsCount(userId);

            if (stars == 0) stars = 1;
            
            for (int i = 0; i < stars; i++)
            {
                rating += "⭐";
            }
            
            string txt = $"{rating}\n\nВы воздерживаетесь <b>{str}</b>";
            
            InlineKeyboardButton button = new InlineKeyboardButton()
            {
                Text = "Добавить дни",
                CallbackData = "add_abs_days"
            };

            IReplyMarkup? markup = new InlineKeyboardMarkup(button);

            if (!userRepository.IsAllowToAddDays(userId))
            {
                markup = null;
            }
            
            await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, txt, ParseMode.Html, 
                replyMarkup: markup);

            await Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
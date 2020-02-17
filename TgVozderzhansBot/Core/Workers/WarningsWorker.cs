using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.Workers
{
    public class WarningsWorker
    {
        private TelegramBotClient _tgBotClient;
        
        public WarningsWorker(TelegramBotClient tgBotClient)
        {
            _tgBotClient = tgBotClient;
        }

        public void Start()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        WarningsRepository.CreateWarningsIfNotExists();
                        WarningsRepository.ClearOldWarnings();
                        WarningsRepository.MassChangeAccountStatuses();

                        foreach (WarningsRepository.UnnotifiedWarning uw in WarningsRepository.GetUnnotifiedWarnings())
                        {
                            try
                            {
                                var task = _tgBotClient.SendTextMessageAsync(uw.TgChatId,
                                    "<b>Внимание!</b> Проверка активности аккаунта. Подтвердите что вы воздерживаетесь, " +
                                    "иначе через несколько дней вы будете скрыты в рейтинге пользователей.", replyMarkup: new InlineKeyboardMarkup(
                                        new InlineKeyboardButton
                                        {
                                            Text = "Подтверждаю",
                                            CallbackData = $"confirm_absistence:{uw.WarnId}"
                                        }), parseMode: ParseMode.Html);

                                task.Wait();
                            
                                Thread.Sleep(500);
                            
                                if (task.Result.MessageId > 0)
                                {
                                    WarningsRepository.SetNotified(uw.WarnId);
                                }
                            }
                            catch
                            {
                                WarningsRepository.SetNotified(uw.WarnId);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Возникло исключение в воркере WarningsWorker");
                    }
                    
                    Thread.Sleep(60000);
                }    
            }).Start();
        }
    }
}
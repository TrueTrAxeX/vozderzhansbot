using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TgVozderzhansBot.Core.Workers
{
    public static class MassMessagePoster
    {
        private static readonly Stack<(long, string)> Messages = new Stack<(long, string)>();

        private static object Locker = new object();
        
        public static void Add(long chatId, string message)
        {
            Messages.Push((chatId, message));
        }

        public static void Start(TelegramBotClient client)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        while (Messages.Count > 0)
                        {
                            try
                            {
                                var data = Messages.Pop();
                                client.SendTextMessageAsync(data.Item1, data.Item2, ParseMode.Html).Wait();
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Ошибка постинга сообщений: " + e.Message);   
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка постинга сообщений");   
                    }
                
                    Thread.Sleep(3000);
                }
            }).Start();
        }
    }
}
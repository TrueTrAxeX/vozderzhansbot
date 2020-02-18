
using System;
using System.Data;
using System.IO;
using Dapper;
using Telegram.Bot;
using TgVozderzhansBot.Core;
using TgVozderzhansBot.Core.DbRepositories;
using TgVozderzhansBot.Core.Models;
using TgVozderzhansBot.Core.TgBot;
using TgVozderzhansBot.Core.TgBot.CommandHandlers;
using TgVozderzhansBot.Core.TgBot.CommandHandlers.FromGroups;
using TgVozderzhansBot.Core.Workers;

namespace TgVozderzhansBot
{
    class Program
    {
        static void Main(string[] args)
        {
            UserRepository userRepository = new UserRepository();
            AbsItemRepository absItemRepository = new AbsItemRepository();

            WarningsRepository.CreateTable();
            FriendsRepository.CreateTable();
            WarningsRepository.CreateWarningsIfNotExists();
            WarningsRepository.GetUnnotifiedWarnings();
            
            userRepository.CreateTable();
            absItemRepository.CreateTable();
            
            TgBot tgBot = new TgBot();
            
            WarningsWorker worker = new WarningsWorker(tgBot.TelegramBotClient);
            worker.Start();

            MassMessagePoster.Start(tgBot.TelegramBotClient);
            
            tgBot.AddCommandHandlers(
                typeof(CancelInviteCommandHandler),
                typeof(BreakFriendshipCommandHandler),
                typeof(FriendAbortHistoryCommandHandler),
                typeof(SelectFriendCommandHandler),
                typeof(FriendListCommandHandler),
                typeof(AcceptInviteCommandHandler),
                typeof(AddFriendInputCommandHandler),
                typeof(AddFriendCommandHandler),
                typeof(FriendsMenuCommandHandler),
                typeof(GetTopUsersCommandHandler),
                typeof(ConfirmAbsistenceCommandHandler),
                typeof(GetUserTermInfoCommandHandler),
                typeof(GetMyTermInfoCommandHandler),
                typeof(SettingsCommandHandler),
                typeof(SelectUserStatusMenuCommandHandler),
                typeof(StartCommandHandler), 
                typeof(MyAbsistenceCommandHandler),
                typeof(StartAbsistenseCommandHandler),
                typeof(StopAbsistenceCommandHandler),
                typeof(CurrentTermCommandHandler),
                typeof(GetHistoryCommandHandler),
                typeof(UsersTableCommandHandler),
                typeof(AddDaysCommandHandlers),
                typeof(UserAddDaysInputCommandHandler));

            tgBot.Start();
            
            Console.ReadLine();
        }
    }
}
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgVozderzhansBot.Core.DbRepositories;

namespace TgVozderzhansBot.Core.TgBot.CommandHandlers
{
    [CommandType(Type.Message)]
    public class UserAddDaysInputCommandHandler : CommandHandler
    {
        public UserAddDaysInputCommandHandler(TelegramBotClient client) : base(client)
        {
            
        }

        public override bool CanHandle(Update update)
        {
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(update.Message.Chat.Id);
            
            var container = UserDataContainerManager.GetOrCreate(userId);

            string waitInput = container.Get<string>("wait_input");

            if (waitInput != null && waitInput == "add_days")
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

            if (waitInput == "add_days")
            {
                if (Regex.IsMatch(update.Message.Text, "^[0-9]{1,3}$"))
                {
                    short days = short.Parse(update.Message.Text);

                    if (days == 0)
                    {
                        await Client.SendTextMessageAsync(update.Message.Chat.Id, "Вы ввели дни в неправильном формате. Попробуйте еще раз");
                    }
                    else
                    {
                        absItemRepository.AddDaysToUser(userId, days);
                        
                        await Client.SendTextMessageAsync(update.Message.Chat.Id, "Дни к воздержанию успешно добавлены!");

                        // Запрещаем пользователю добавлять дни впредь
                        userRepository.SetDisallowAddDays(userId);
                        
                        container.Set<string>("wait_input", null);
                    }
                }
                else
                {
                    container.Set<string>("wait_input", null);
                }
            }
        }
    }
}
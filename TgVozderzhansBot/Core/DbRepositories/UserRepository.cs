using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.DbRepositories
{
    public class UserRepository
    {
        public UserRepository()
        {
            
        }

        public enum UserStatus
        {
            Public = 0,
            Private = 1
        }

        public bool UpdateNicknameIfDifferent(long chatId, string newNickname)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"UPDATE Users SET Username = @Username WHERE ChatId = @ChatId AND Username != @Username";

                int res = conn.Execute(sqlQuery, new { ChatID = chatId, Username = newNickname });

                return res > 0;
            }
        }

        public bool SetUserStatus(long userId, UserStatus status)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"UPDATE Users SET Status = @Status WHERE id = @UserId";

                int res = conn.Execute(sqlQuery, new { UserId = userId, Status = status });

                return res > 0;
            }
        }
        
        public UserStatus GetUserStatus(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"SELECT Status FROM Users WHERE id = @UserId";

                UserStatus userStatus = conn.QuerySingleOrDefault<UserStatus>(sqlQuery, new { UserId = userId });

                return userStatus;
            }
        }

        public void SetDisallowAddDays(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"UPDATE Users SET AllowAddDays = @AllowAddDays WHERE id = @UserId";

                conn.Execute(sqlQuery, new { UserId = userId, AllowAddDays = false });
            }
        }

        public bool IsAllowToAddDays(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"SELECT COUNT(id) FROM Users WHERE id = @UserId AND AllowAddDays = @AllowAddDays";

                long user = conn.QuerySingleOrDefault<long>(sqlQuery, new { UserId = userId, AllowAddDays = true });

                return user > 0;
            }
        }

        public long? GetPublicUserIdByNickname(string nickname)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"SELECT id FROM Users WHERE Username = @Nickname AND Status = @Status";

                long? user = conn.QuerySingleOrDefault<long?>(sqlQuery, new { Nickname = nickname, Status = UserStatus.Public });

                return user;
            }
        }
        
        public long GetUserIdByChatId(long chatId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"SELECT id FROM Users WHERE ChatId = @ChatId";

                long user = conn.QuerySingleOrDefault<long>(sqlQuery, new { ChatId = chatId });

                return user;
            }
        }
        
        public IEnumerable<User> GetAllUsers(int offset = 0, int limit = 10)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"SELECT * FROM Users LIMIT {offset},{limit}";

                IEnumerable<User> users = conn.Query<User>(sqlQuery, sqlQuery);

                return users;
            }
        }

        public void Insert(User user)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = "INSERT INTO Users (Username, CreatedAt, ChatId) VALUES (@Username, @CreatedAt, @ChatId)";

                IEnumerable<long> userId = conn.Query<long>(sqlQuery, user);
                user.Id = userId.FirstOrDefault();
            }
        }
        
        public void CreateUserIfNOtExists(User user)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = "SELECT COUNT(id) FROM Users WHERE Username = @Username";

                long count = conn.QuerySingleOrDefault<long>(sqlQuery, new { Username = user.Username } );

                if (count == 0)
                {
                    sqlQuery = "INSERT INTO Users (Username, CreatedAt, ChatId) VALUES (@Username, @CreatedAt, @ChatId)";

                    IEnumerable<long> userId = conn.Query<long>(sqlQuery, user);
                    user.Id = userId.FirstOrDefault();
                }
            }
        }

        public void CreateTable()
        {
            string createUsersTableScript = File.ReadAllText("./Data/SqlScripts/users.sql");

            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                conn.Query(createUsersTableScript);
            }
        }
    }
}
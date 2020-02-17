using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.DbRepositories
{
    public class AbsItemRepository
    {
        public class AbsInterval
        {
            public DateTime DateFrom { get; set; }
            public DateTime FinishedAt { get; set; }
        }

        public void AddDaysToUser(long userId, short daysCount)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = $"UPDATE AbsItem SET DateFrom = date(DateFrom, '-{daysCount} day') WHERE UserId = @UserId AND FinishedAt IS NULL";

                conn.Execute(sqlQuery, new { UserId = userId });
            }
        }

        public IEnumerable<AbsItem> GetHistory(long userId, int limit)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                
                string sqlQuery = "SELECT * FROM AbsItem WHERE UserId = @UserId AND FinishedAt IS NOT NULL ORDER BY id DESC LIMIT " + limit;

                IEnumerable<AbsItem> date = conn.Query<AbsItem>(sqlQuery, new {UserId = userId});

                return date;
            }
        }

        public long GetTotalUsersCount()
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                
                string sqlQuery = $"SELECT COUNT(A.id) FROM AbsItem A LEFT JOIN Users U ON A.UserId = U.id WHERE FinishedAt IS NULL AND U.Username IS NOT NULL AND Status = @Status";

                long count = conn.QuerySingleOrDefault<long>(sqlQuery, new { Status = UserRepository.UserStatus.Public });

                return count;
            }
        }
        
        public IEnumerable<dynamic> GetTopUsers(int offset = 0, int limit = 10)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                
                string sqlQuery = $"SELECT A.*, U.Username FROM AbsItem A LEFT JOIN Users U ON A.UserId = U.id WHERE FinishedAt IS NULL AND U.Username IS NOT NULL AND Status = @Status ORDER BY A.DateFrom LIMIT {offset},{limit}";

                IEnumerable<dynamic> users = conn.Query<dynamic>(sqlQuery, new { Status = UserRepository.UserStatus.Public });

                return users;
            }
        }

        public DateTime? GetCurrentTermByNickname(string nickname)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                
                string sqlQuery = "SELECT DateFrom FROM AbsItem A LEFT JOIN Users U ON A.UserId = U.id WHERE U.Username = @Username AND A.FinishedAt IS NULL";

                DateTime? date = conn.QuerySingleOrDefault<DateTime?>(sqlQuery, new {Username = nickname});

                return date;
            }
        }
        
        public DateTime CurrentTerm(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                
                string sqlQuery = "SELECT DateFrom FROM AbsItem WHERE UserId = @UserId AND FinishedAt IS NULL";

                DateTime date = conn.QuerySingleOrDefault<DateTime>(sqlQuery, new {UserId = userId});

                return date;
            }
        }
        
        public bool HasHistory(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                string sqlQuery = "SELECT COUNT(id) FROM AbsItem WHERE UserId = @UserId AND FinishedAt IS NOT NULL";

                long count = conn.QuerySingleOrDefault<long>(sqlQuery, new {UserId = userId});

                return count > 0;
            }
        }
        
        public bool HasAbs(long userId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();
                string sqlQuery = "SELECT COUNT(id) FROM AbsItem WHERE UserId = @UserId AND FinishedAt IS NULL";

                long count = conn.QuerySingleOrDefault<long>(sqlQuery, new {UserId = userId});

                return count > 0;
            }
        }

        public bool StopAbsistence(long chatId)
        {
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(chatId);
            
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = "UPDATE AbsItem SET FinishedAt = @FinishedAt WHERE UserId = @UserId AND FinishedAt IS NULL";

                conn.Execute(sqlQuery, new { UserId = userId, FinishedAt = DateTime.Now });

                return true;
            }

            return false;
        }
        
        public bool StartAbsistence(long chatId)
        {
            UserRepository userRepository = new UserRepository();

            long userId = userRepository.GetUserIdByChatId(chatId);
            
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlQuery = "SELECT COUNT(id) FROM AbsItem WHERE UserId = @UserId AND FinishedAt IS NULL";

                long count = conn.QuerySingleOrDefault<long>(sqlQuery, new { UserId = userId } );

                if (count == 0)
                {
                    sqlQuery = "INSERT INTO AbsItem (UserId, DateFrom) VALUES (@UserId, @DateFrom)";

                    conn.Execute(sqlQuery, new { UserId = userId, DateFrom = DateTime.Now, LastConfirm = DateTime.Now });

                    return true;
                }
            }

            return false;
        }
        
        public void CreateTable()
        {
            string createTableScript = File.ReadAllText("./Data/SqlScripts/abs_item.sql");

            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                conn.Query(createTableScript);
            }
        }
    }
}
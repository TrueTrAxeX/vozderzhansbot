using System;
using System.Collections.Generic;
using System.IO;
using Dapper;

namespace TgVozderzhansBot.Core.DbRepositories
{
    public class WarningsRepository
    {
        public static void CreateTable()
        {
            string createTableScript = File.ReadAllText("./Data/SqlScripts/warnings.sql");

            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                conn.Query(createTableScript);
            }
        }

        public class UnnotifiedWarning
        {
            public long WarnId { get; set; }
            
            public long UserId { get; set; }
            
            public long AbsId { get; set; }
            
            public long TgChatId { get; set; }
        }

        /**
         * Если юзер не подтвердил что он воздерживается в течении трех дней, метод меняет его аккаунт на "Приватный"
         */
        public static void MassChangeAccountStatuses()
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"SELECT U.id as UserId, W.id as WarnId FROM Users U  
                            LEFT JOIN AbsItem A ON U.id = A.UserId
                            LEFT JOIN Warnings W ON W.AbsItemId = A.id
                            WHERE W.CreatedAt < @Date AND W.IsConfirmed = 0";
                
                var data = conn.Query<dynamic>(sqlScript, new {Date = DateTime.Now.AddDays(-3)});

                foreach (dynamic item in data)
                {
                    Console.WriteLine($"Статус юзера ID:{item.UserId} поменялся на Private");
                    conn.Execute("UPDATE Users SET Status = 1 WHERE id = @UserId", new { UserId = item.UserId });
                    conn.Execute("DELETE FROM Warnings WHERE id = @WarnId", new { WarnId = item.WarnId });
                }
            }
        }

        public static bool ClearOldWarnings()
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"DELETE FROM Warnings WHERE ActiveUntil < @CurrentDate";
                
                return conn.Execute(sqlScript, new {CurrentDate = DateTime.Now}) > 0;
            }
        }
        
        public static bool SetConfirmed(long warningId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"UPDATE Warnings SET IsConfirmed = 1 WHERE id = @WarningId";
                
                return conn.Execute(sqlScript, new {WarningId = warningId}) > 0;
            }
        }

        public static bool SetNotified(long warningId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"UPDATE Warnings SET IsNotified = 1 WHERE id = @WarningId";
                
                return conn.Execute(sqlScript, new {WarningId = warningId}) > 0;
            }
        }
        
        public static IEnumerable<UnnotifiedWarning> GetUnnotifiedWarnings()
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"SELECT W.id as WarnId, A.id as AbsId, U.id as UserId, U.ChatId as TgChatId
                    FROM Warnings W LEFT JOIN AbsItem A ON W.AbsItemId = A.id
                    LEFT JOIN Users U ON U.id = A.UserId
                    WHERE W.IsNotified = 0 AND U.Status = @Status";
                
                var items = conn.Query<UnnotifiedWarning>(sqlScript, new {Status = UserRepository.UserStatus.Public});

                return items;
            }
        }

        public static void CreateWarningsIfNotExists()
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string sqlScript = 
                    @"SELECT W.id as WarnId, A.id as AbsId, A.DateFrom as DateFrom, U.id as UserId
                    FROM AbsItem A LEFT JOIN Warnings W ON W.AbsItemId = A.id
                    LEFT JOIN Users U ON U.id = A.UserId
                    WHERE A.FinishedAt IS NULL AND W.id IS NULL AND U.Status = @Status";
                
                var items = conn.Query<dynamic>(sqlScript, new {Status = UserRepository.UserStatus.Public});

                foreach (dynamic item in items)
                {
                    var dateFrom = item.DateFrom as DateTime?;

                    if (dateFrom != null)
                    {
                        TimeSpan span = DateTime.Now - dateFrom.Value;

                        // Если воздержание больше трех дней
                        if (span.TotalDays > 1)
                        {
                            sqlScript = "INSERT INTO Warnings (AbsItemId, ActiveUntil, CreatedAt) VALUES (@AbsItemId, @ActiveUntil, @CreatedAt)";
                            
                            bool success = conn.Execute(sqlScript, 
                                new { 
                                    AbsItemId = item.AbsId, 
                                    ActiveUntil = DateTime.Now.AddDays(30), 
                                    CreatedAt = DateTime.Now }) > 0;

                            if (success)
                            {
                                Console.WriteLine($"Предупреждение Warning создано для пользователя ID:{item.UserId}");
                            }
                        }
                    }
                }
            }
        }
    }
}
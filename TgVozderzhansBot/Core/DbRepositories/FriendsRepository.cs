using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using TgVozderzhansBot.Core.Models;

namespace TgVozderzhansBot.Core.DbRepositories
{
    public class FriendsRepository
    {
        public static bool RemoveFriend(long userId, long friendId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                int count = conn.Execute(
                    "DELETE FROM Friends WHERE (UserId = @UserId AND FriendId = @FriendId) OR (UserId = @FriendId AND FriendId = @UserId)",
                    new { UserId = userId, FriendId = friendId });

                if (count == 2) return true;
                else return false;
            }
        }
        
        public static List<User> GetFriends(long userId, int offest = 0, int limit = 10)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                IEnumerable<User> users = conn.Query<User>(
                    "SELECT U.* FROM Friends F LEFT JOIN Users U ON F.FriendId = U.id WHERE F.UserId = @UserId",
                    new { UserId = userId });

                return users.ToList();
            }
        }
        
        public static bool HasFriend(long userId, long friendId)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                long count = conn.QueryFirstOrDefault<long>("SELECT COUNT(id) FROM Friends WHERE UserId = @UserId AND FriendId = @FriendId",
                    new { UserId = userId, FriendId = friendId});

                return count > 0;
            }
        }
        
        public static void CreateTable()
        {
            string createUsersTableScript = File.ReadAllText("./Data/SqlScripts/friends.sql");

            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                conn.Query(createUsersTableScript);
            }
        }

        public static bool Insert(Friend friend)
        {
            using (var conn = SqliteBaseRepository.SimpleDbConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Friends (UserId, FriendId, CreatedAt) VALUES (@UserId, @FriendId, @CreatedAt)";
                
                return conn.Execute(query, friend) > 0;
            }
        }
    }
}
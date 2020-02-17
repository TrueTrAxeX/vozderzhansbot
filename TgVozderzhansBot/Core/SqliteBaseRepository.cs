using System;
using System.Data.SQLite;

namespace TgVozderzhansBot.Core
{
    public class SqliteBaseRepository
    {
        public static string DbFile
        {
            get { return "./data.db"; }
        }

        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }
    }
}
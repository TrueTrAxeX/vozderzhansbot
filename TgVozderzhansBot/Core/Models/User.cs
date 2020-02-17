using System;

namespace TgVozderzhansBot.Core.Models
{
    public class User
    {
        public long Id { get; set; }
        
        public long ChatId { get; set; }
        
        public string Username { get; set; }

        public bool AllowAddDays { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
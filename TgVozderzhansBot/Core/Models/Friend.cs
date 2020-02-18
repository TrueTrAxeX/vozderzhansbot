using System;

namespace TgVozderzhansBot.Core.Models
{
    public class Friend
    {
        public long Id { get; set; }
        
        public long UserId { get; set; }
        
        public long FriendId { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
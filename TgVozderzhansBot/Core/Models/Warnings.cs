using System;

namespace TgVozderzhansBot.Core.Models
{
    public class Warnings
    {
        public long Id { get; set; }
        
        public long AbsItemId { get; set; }
        
        public DateTime ActiveUntil { get; set; }
        
        public bool IsNotified { get; set; }
    }
}
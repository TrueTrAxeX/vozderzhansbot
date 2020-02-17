using System;

namespace TgVozderzhansBot.Core.Models
{
    public class AbsItem
    {
        public long Id { get; set; }
        
        public User User { get; set; }
        
        public DateTime DateFrom { get; set; }
        
        public DateTime? FinishedAt { get; set; }
        
        public DateTime? LastConfirm { get; set; }
    }
}
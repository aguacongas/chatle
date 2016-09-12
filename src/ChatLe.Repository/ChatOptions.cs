using System;

namespace ChatLe.Models
{
    public class ChatOptions
    {
        public int UserPerPage { get; set; }= 50;

        public bool ContextAutoDetectChanges { get; set; } = true;

        public bool ContextEnableTracking { get; set; }
    }
}
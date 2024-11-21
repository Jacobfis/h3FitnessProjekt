﻿namespace API.Models
{
    public class ActivityLog
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public string Type { get; set; }
    }
    public class ActivityLogCreateDTO
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public string Type { get; set; }
    }
}

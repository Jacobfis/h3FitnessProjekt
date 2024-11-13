namespace API.Models
{
    public class ActivityLog
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; } // in kilometers
        public TimeSpan Duration { get; set; } // duration of the activity
        public string Type { get; set; }
    }
    public class ActivityLogCreateDTO
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; } // in kilometers
        public TimeSpan Duration { get; set; } // duration of the activity
        public string Type { get; set; }
    }
}

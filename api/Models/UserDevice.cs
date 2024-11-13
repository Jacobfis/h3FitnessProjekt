namespace API.Models
{
    public class UserDevice
    {
        public class User
        {
    public ICollection<ActivityLog> ActivityLogs { get; set; }
        }

        public class ActivityLog
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public int Steps { get; set; }
            public double Distance { get; set; } // in kilometers
            public TimeSpan Duration { get; set; } // duration of the activity
            public string Type { get; set; } // e.g., "Running", "Walking"
            public string UserId { get; set; }
            public User User { get; set; }
        }
    }
}

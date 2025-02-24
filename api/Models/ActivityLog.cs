    namespace API.Models
    {
        public class ActivityLog
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public int Steps { get; set; }
            public double Distance { get; set; }
            public double Calories { get; set; } 
            public TimeSpan Duration { get; set; }
            public string Type { get; set; }
            public string userId { get; set; }
            public User user { get; set; }
        }

        public class ActivityLogCreateDTO
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public int Steps { get; set; }
            public double Distance { get; set; }
            public double Calories { get; set; }
            public DurationDto Duration { get; set; } // Indlejret Duration-objekt
            public string Type { get; set; }
        }
    
        public class DurationDto
        {
            public long Ticks { get; set; }
        }

    }

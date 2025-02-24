namespace API.Models
{
    public class Quiz
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Text { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class Answer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}

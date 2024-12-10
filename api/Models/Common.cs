using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Common
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserDevice
    {
        [Key]
        public String id { get; set; }
        public String userId { get; set; }
        public User? user { get; set; }
        public String? deviceId { get; set; }
        public Device? device { get; set; }
    }

    public class CreateUserDeviceDTO
    {
        public String userId { get; set; }
        public String? deviceId { get; set; }
    }
}

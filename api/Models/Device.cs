namespace API.Models
{
    public class Device : Common
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public Device Devices { get; set; }
    }
    public class DeviceDataCreateDTO
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
    public class DeviceDataReadDTO
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}

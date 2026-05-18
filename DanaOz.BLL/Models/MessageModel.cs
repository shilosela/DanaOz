namespace DanaOz.BLL.Models
{
    public class MessageModel
    {
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
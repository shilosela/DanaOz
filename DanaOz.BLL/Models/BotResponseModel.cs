namespace DanaOz.BLL.Models
{
    public class BotResponseModel
    {
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
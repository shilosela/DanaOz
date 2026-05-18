using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserReminder
    {
        [Key]
        public int ReminderId { get; set; }

        [ForeignKey("UserReminderSettings")]
        public int ReminderSettingsId { get; set; }

        public DateTime? DatetimeSent { get; set; }
        public DateTime? DatetimeRead { get; set; }

        [MaxLength(100)]
        public string? UserReaction { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public UserReminderSettings UserReminderSettings { get; set; }
    }
}
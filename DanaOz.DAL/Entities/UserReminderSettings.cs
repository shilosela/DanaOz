using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserReminderSettings
    {
        [Key]
        public int ReminderSettingsId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public int? ClassId { get; set; }
        public int? TimetableId { get; set; }
        public bool IsRecurring { get; set; } = false;

        [MaxLength(200)]
        public string? RecurringInfo { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        [MaxLength(50)]
        public string? SubType { get; set; }

        public bool SyncToCalendar { get; set; } = false;

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public ICollection<UserReminder> Reminders { get; set; }
    }
}
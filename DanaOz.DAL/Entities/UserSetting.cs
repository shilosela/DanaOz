using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserSetting
    {
        [Key]
        public int SettingsId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string SettingsKey { get; set; }

        [MaxLength(200)]
        public string? SettingsValue { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
}
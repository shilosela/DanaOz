using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DanaOz.DAL.Entities;

namespace DanaOz.DAL.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; } = "female";

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(200)]
        public string? SchoolName { get; set; }

        public int? BirthYear { get; set; }
        public bool IsSubjectCoord { get; set; } = false;
        public int DanaPoints { get; set; } = 50;
        public int OnboardingStep { get; set; } = 0;
        public bool IsOnboarded { get; set; } = false;

        public DateTime? FirstUse_DateTime { get; set; }
        public DateTime? LastLogin_DateTime { get; set; }
        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<UserSetting> Settings { get; set; }
        public ICollection<UserClass> Classes { get; set; }
        public ICollection<ChatLog> ChatLogs { get; set; }
        public ICollection<UserPoints> Points { get; set; }
        public ICollection<UserReminderSettings> ReminderSettings { get; set; }
        public ICollection<UserMaterial> Materials { get; set; }
    }
}
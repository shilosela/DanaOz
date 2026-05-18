using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserCommercialHistory
    {
        [Key]
        public int CommercialHistoryId { get; set; }

        [ForeignKey("Commercial")]
        public int CommercialId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime? Launch_DateTime { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(100)]
        public string? TriggerForCommercial { get; set; }

        public int? WatchTimeSeconds { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public Commercial Commercial { get; set; }
        public User User { get; set; }
    }
}
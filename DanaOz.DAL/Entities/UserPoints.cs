using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserPoints
    {
        [Key]
        public int PointId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public int? EventTypeId { get; set; }
        public int PointsGiven { get; set; }
        public int? OriginalChatLogId { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
}
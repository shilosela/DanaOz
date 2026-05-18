using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserMaterial
    {
        [Key]
        public int MaterialId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public int? ClassId { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        public string? FileUrl { get; set; }
        public bool IsShared { get; set; } = false;
        public int PointsEarned { get; set; } = 0;

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
}
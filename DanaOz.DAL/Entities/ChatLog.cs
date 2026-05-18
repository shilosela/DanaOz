using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class ChatLog
    {
        [Key]
        public int ChatLogId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [MaxLength(20)]
        public string Direction { get; set; } // Inbound, Outbound, Push

        public string? MessageText { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserSchool
    {
        [Key]
        public int UserSchoolId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string SchoolName { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public int SortOrder { get; set; } = 1;

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
    }
}

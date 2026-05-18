using System.ComponentModel.DataAnnotations;

namespace DanaOz.DAL.Entities
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;
    }
}
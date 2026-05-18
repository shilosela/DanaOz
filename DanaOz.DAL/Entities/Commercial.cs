using System.ComponentModel.DataAnnotations;

namespace DanaOz.DAL.Entities
{
    public class Commercial
    {
        [Key]
        public int CommercialId { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;
    }
}
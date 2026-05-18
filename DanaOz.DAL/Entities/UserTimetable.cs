using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserTimetable
    {
        [Key]
        public int TimetableId { get; set; }

        [ForeignKey("UserClass")]
        public int ClassId { get; set; }

        public int DayOfWeek { get; set; } // 0=Sunday, 6=Saturday
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? Period { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public UserClass UserClass { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanaOz.DAL.Entities
{
    public class UserClass
    {
        [Key]
        public int ClassId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public int? SubjectId { get; set; }
        public int? SchoolId { get; set; }

        [MaxLength(100)]
        public string? ClassName { get; set; }

        [MaxLength(20)]
        public string? Grade { get; set; }

        public int? StudyUnits { get; set; }
        public int? StudentsCount { get; set; }

        public DateTime Creation_DateTime { get; set; } = DateTime.UtcNow;
        public DateTime Last_Update_DateTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public Subject? Subject { get; set; }
        public School? School { get; set; }
        public ICollection<UserTimetable> Timetables { get; set; }
    }
}
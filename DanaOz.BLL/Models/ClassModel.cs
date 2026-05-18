namespace DanaOz.BLL.Models
{
    public class ClassModel
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string? SubjectName { get; set; }
        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }
        public string? Grade { get; set; }
        public int? StudyUnits { get; set; }
        public int? StudentsCount { get; set; }
    }
}
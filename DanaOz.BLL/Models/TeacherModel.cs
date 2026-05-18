namespace DanaOz.BLL.Models
{
    public class TeacherModel
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? SchoolName { get; set; }
        public int DanaPoints { get; set; }
        public int OnboardingStep { get; set; }
        public bool IsOnboarded { get; set; }
        public List<ClassModel> Classes { get; set; } = new();
    }
}
namespace FcmsPortalUI.DTOs
{
    public class ClassScheduleListItem
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Venue { get; set; } = string.Empty;
        public int? ClassSessionId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int SessionNumber { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string? TeacherName { get; set; }
    }
}

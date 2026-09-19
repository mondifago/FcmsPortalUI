namespace FcmsPortalUI.DTOs
{
    public class ClassSessionListItem
    {
        public int Id { get; set; }
        public int SessionNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string? TeacherName { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public bool IsClosed => ClosedAt.HasValue;
    }
}

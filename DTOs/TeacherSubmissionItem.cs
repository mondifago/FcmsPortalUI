using FcmsPortal.Enums;

namespace FcmsPortalUI.DTOs
{
    public class TeacherSubmissionItem
    {
        public string StudentName { get; set; } = string.Empty;
        public ClassLevel? ClassLevel { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}

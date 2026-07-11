using FcmsPortal.Enums;

namespace FcmsPortalUI.DTOs
{
    public class GradesReport
    {
        public int LearningPathId { get; set; }
        public string LearningPathName { get; set; } = string.Empty;
        public DateTime DateSubmitted { get; set; } = DateTime.Now;
        public string SubmittedBy { get; set; } = string.Empty;
        public PrincipalApprovalStatus Status { get; set; } = PrincipalApprovalStatus.Pending;
    }
}

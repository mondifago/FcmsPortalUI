using FcmsPortal.Enums;

namespace FcmsPortalUI.DTOs
{
    public class LearningPathListItem
    {
        public int Id { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public Semester Semester { get; set; }
        public DateTime AcademicYearStart { get; set; }
        public int AcademicPeriodId { get; set; }
        public PrincipalApprovalStatus ApprovalStatus { get; set; }

        public string AcademicYear
        {
            get
            {
                int startYear = AcademicYearStart.Year;
                int endYear = startYear + 1;
                return $"{startYear}-{endYear}";
            }
        }
    }
}

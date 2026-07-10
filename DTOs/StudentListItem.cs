using FcmsPortal.Enums;

namespace FcmsPortalUI.DTOs
{
    public class StudentListItem
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public bool IsActive { get; set; }
        public int? LearningPathId { get; set; }

        public string FullName => string.Join(" ",
            new[] { FirstName, MiddleName, LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));

        public string Initials => $"{(FirstName.Length > 0 ? FirstName[0].ToString() : "")}{(LastName.Length > 0 ? LastName[0].ToString() : "")}".ToUpper();

        public bool IsEnrolled => LearningPathId > 0;

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
    }
}

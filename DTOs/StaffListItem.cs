using FcmsPortal.Enums;

namespace FcmsPortalUI.DTOs
{
    public class StaffListItem
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }

        public string FullName => string.Join(" ",
            new[] { FirstName, MiddleName, LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));

        public string Initials => $"{(FirstName.Length > 0 ? FirstName[0].ToString() : "")}{(LastName.Length > 0 ? LastName[0].ToString() : "")}".ToUpper();
    }
}

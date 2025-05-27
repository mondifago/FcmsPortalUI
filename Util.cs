using FcmsPortal.Constants;
using FcmsPortal.Enums;
using FcmsPortal.Models;
using System.Text.RegularExpressions;

namespace FcmsPortalUI
{
    public static class Util
    {
        public static string GetFullName(Person person)
        {
            if (person == null) return "-";

            string middleName = string.IsNullOrEmpty(person.MiddleName) ? "" : $" {person.MiddleName}";
            return $"{person.FirstName}{middleName} {person.LastName}";
        }

        public static string GetLearningPathName(LearningPath learningPath)
        {
            if (learningPath == null) return "-";
            return $"{learningPath.EducationLevel} - {learningPath.ClassLevel} ({learningPath.AcademicYear} {learningPath.Semester})";
        }

        public static string GetApprovalStatusBadgeClass(PrincipalApprovalStatus status)
        {
            return status switch
            {
                PrincipalApprovalStatus.Approved => "badge bg-success",
                PrincipalApprovalStatus.Review => "badge bg-info",
                PrincipalApprovalStatus.Pending => "badge bg-warning",
                _ => "badge bg-secondary"
            };
        }

        public static string GetInitials(Person person)
        {
            string initials = "";

            if (!string.IsNullOrEmpty(person.FirstName) && person.FirstName.Length > 0)
                initials += person.FirstName[0];

            if (!string.IsNullOrEmpty(person.LastName) && person.LastName.Length > 0)
                initials += person.LastName[0];

            return initials;
        }

        public static void ToggleItemSelection<T>(int itemId, object isChecked, HashSet<int> selectedIds)
        {
            if (isChecked is bool checkedValue)
            {
                if (checkedValue && !selectedIds.Contains(itemId))
                {
                    selectedIds.Add(itemId);
                }
                else if (!checkedValue && selectedIds.Contains(itemId))
                {
                    selectedIds.Remove(itemId);
                }
            }
        }

        public static string FormatTextWithLinks(string content)
        {
            if (string.IsNullOrEmpty(content))
                return "";

            string formatted = content.Replace(Environment.NewLine, "<br>").Replace("\n", "<br>");

            string urlPattern = @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})";
            formatted = Regex.Replace(
                formatted,
                urlPattern,
                match =>
                {
                    string url = match.Value;
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        url = "https://" + url;
                    return $"<a href=\"{url}\" target=\"_blank\">{match.Value}</a>";
                }
            );

            return formatted;
        }

        public static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int i = 0;
            double size = bytes;

            while (size >= FcmsConstants.BYTES_IN_KILOBYTE && i < suffixes.Length - 1)
            {
                size /= FcmsConstants.BYTES_IN_KILOBYTE;
                i++;
            }
            return $"{size:F2} {suffixes[i]}";
        }

        public static string GetFileIcon(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "fa fa-file";

            string ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                ".pdf" => "fa fa-file-pdf",
                ".doc" or ".docx" => "fa fa-file-word",
                ".xls" or ".xlsx" => "fa fa-file-excel",
                ".ppt" or ".pptx" => "fa fa-file-powerpoint",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "fa fa-file-image",
                ".zip" or ".rar" => "fa fa-file-archive",
                ".txt" => "fa fa-file-alt",
                _ => "fa fa-file"
            };
        }

        public static string GetScheduleTitle(ScheduleEntry schedule)
        {
            if (!string.IsNullOrEmpty(schedule.Title))
                return schedule.Title;
            if (schedule.ClassSession != null)
                return $"{schedule.ClassSession.Course} - {schedule.ClassSession.Topic}";
            if (!string.IsNullOrEmpty(schedule.Meeting))
                return schedule.Meeting;
            if (!string.IsNullOrEmpty(schedule.Event))
                return schedule.Event;
            return "Untitled Schedule";
        }

        public static string GetScheduleTypeColor(ScheduleEntry schedule)
        {
            if (schedule.ClassSession != null)
                return "bg-info";
            if (!string.IsNullOrEmpty(schedule.Meeting))
                return "bg-warning";
            if (!string.IsNullOrEmpty(schedule.Event))
                return "bg-success";
            return "bg-primary";
        }

        public static string GetScheduleTypeIcon(ScheduleEntry schedule)
        {
            if (schedule.ClassSession != null)
                return "chalkboard-teacher";
            if (!string.IsNullOrEmpty(schedule.Meeting))
                return "users";
            if (!string.IsNullOrEmpty(schedule.Event))
                return "calendar-day";
            return "calendar";
        }

        public static string GetScheduleTypeDisplayName(ScheduleEntry schedule)
        {
            if (schedule.ClassSession != null)
                return "Class";
            if (!string.IsNullOrEmpty(schedule.Meeting))
                return "Meeting";
            if (!string.IsNullOrEmpty(schedule.Event))
                return "Event";
            return string.Empty;
        }

        public static (string Text, string CssClass) GetPaymentStatus(Student student)
        {
            var schoolFees = student?.Person?.SchoolFees;
            double totalPaid = schoolFees?.Payments.Sum(p => p.Amount) ?? 0;
            double totalDue = schoolFees?.TotalAmount ?? 0;
            double threshold = totalDue * FcmsConstants.PAYMENT_THRESHOLD_FACTOR;

            if (totalPaid == 0)
            {
                return ("Unpaid", "bg-danger");
            }

            if (totalPaid < threshold)
            {
                return ("No Access", "bg-warning");
            }

            if (totalPaid < totalDue)
            {
                return ("Access", "bg-info");
            }

            return ("Paid", "bg-success");
        }
    }
}

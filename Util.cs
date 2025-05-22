using FcmsPortal.Constants;
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
    }
}

using FcmsPortal;
using FcmsPortal.Constants;
using FcmsPortal.Enums;
using FcmsPortal.Models;
using Microsoft.AspNetCore.Components.Forms;
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
            return $"{learningPath.EducationLevel.ToDisplayName()} - {learningPath.ClassLevel.ToDisplayName()} ({learningPath.AcademicYear} {learningPath.Semester.ToTermDisplay()})";
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
            return "Add Session";
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

        public static string GetPromotionStatusForArchive(LearningPath learningPath, bool isPromoted)
        {
            if (!isPromoted)
                return "NOT PROMOTED";

            if (LogicMethods.IsLastClassInEducationLevel(learningPath.EducationLevel, learningPath.ClassLevel))
                return "GRADUATED";

            var (nextEducation, nextClass) = LogicMethods.GetNextEducationLevelAndClass(
                learningPath.EducationLevel, learningPath.ClassLevel);

            if (nextEducation.HasValue && nextEducation != learningPath.EducationLevel)
            {
                return $"PROMOTED to {nextEducation.Value.ToDisplayName()} {nextClass.Value.ToDisplayName()}";
            }

            var nextClassLevel = LogicMethods.GetNextClassLevel(learningPath.EducationLevel, learningPath.ClassLevel);
            return $"PROMOTED to {learningPath.EducationLevel.ToDisplayName()} {nextClassLevel.Value.ToDisplayName()}";
        }

        public static string GetGradeCode(double totalGrade)
        {
            return totalGrade switch
            {
                >= FcmsConstants.A_GRADE_MIN => "A",
                >= FcmsConstants.B_GRADE_MIN => "B",
                >= FcmsConstants.C_GRADE_MIN => "C",
                >= FcmsConstants.D_GRADE_MIN => "D",
                >= FcmsConstants.E_GRADE_MIN => "E",
                _ => "F",
            };
        }

        public static string GetGradeRemark(double grade)
        {
            if (grade >= FcmsConstants.EXCELLENT_GRADE_MIN) return "Excellent";
            if (grade >= FcmsConstants.VERY_GOOD_GRADE_MIN) return "Very Good";
            if (grade >= FcmsConstants.GOOD_GRADE_MIN) return "Good";
            if (grade >= FcmsConstants.AVERAGE_GRADE_MIN) return "Average";
            if (grade >= FcmsConstants.FAIR_GRADE_MIN) return "Fair";
            return "Needs Improvement";
        }

        public static string GetGradeColorClass(double grade, bool useTextPrefix = false)
        {
            string prefix = useTextPrefix ? "text-" : "";

            if (grade >= FcmsConstants.A_GRADE_MIN) return $"{prefix}success";
            if (grade >= FcmsConstants.B_GRADE_MIN) return $"{prefix}warning";
            return $"{prefix}danger";
        }

        public static string GetOrdinalNumber(int number)
        {
            if (number <= 0) return number.ToString();

            if (number % 100 >= 11 && number % 100 <= 13)
                return $"{number}th";

            return (number % 10) switch
            {
                FcmsConstants.FIRST_PLACE => $"{number}st",
                FcmsConstants.SECOND_PLACE => $"{number}nd",
                FcmsConstants.THIRD_PLACE => $"{number}rd",
                _ => $"{number}th"
            };
        }

        public static string GetRankColorClass(int rank)
        {
            return rank switch
            {
                FcmsConstants.FIRST_PLACE => "text-warning",
                FcmsConstants.SECOND_PLACE => "text-secondary",
                FcmsConstants.THIRD_PLACE => "text-danger",
                _ => "text-muted"
            };
        }

        public static string GetGradeTypeName(GradeType gradeType)
        {
            return gradeType switch
            {
                GradeType.Homework => "HW",
                GradeType.Quiz => "Quiz",
                GradeType.FinalExam => "Exam",
                _ => gradeType.ToString()
            };
        }

        public static string GetPromotionButtonText(EducationLevel educationLevel, ClassLevel classLevel)
        {
            if (LogicMethods.IsLastClassInEducationLevel(educationLevel, classLevel))
            {
                return "Graduate";
            }
            return "Promote";
        }

        public static bool IsValidImageFile(IBrowserFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        public static string GetLogoDisplayUrl(string logoUrl)
        {
            if (string.IsNullOrEmpty(logoUrl))
                return "";

            if (logoUrl.StartsWith("data:image"))
                return logoUrl;

            return logoUrl;
        }

        public static async Task<IBrowserFile> ResizeImageForLogo(IBrowserFile originalFile)
        {
            return await originalFile.RequestImageFileAsync("image/jpeg", 400, 400);
        }

        public static bool IsStudentEnrolled(Student student)
        {
            return student?.LearningPathId > 0;
        }

        public static string ToTermDisplay(this Semester semester)
        {
            return semester switch
            {
                Semester.First => "First Term",
                Semester.Second => "Second Term",
                Semester.Third => "Third Term",
                _ => semester.ToString()
            };
        }

        public static string ToTermDisplay(this string semesterString)
        {
            if (string.IsNullOrEmpty(semesterString))
                return string.Empty;

            if (Enum.TryParse<Semester>(semesterString, out var semester))
            {
                return semester.ToTermDisplay();
            }
            return semesterString;
        }

        public static string ToDisplayName(this ClassLevel classLevel)
        {
            return classLevel switch
            {
                ClassLevel.KG_Daycare => "Day Care",
                ClassLevel.KG_PlayGroup => "Play Group",
                ClassLevel.KG_PreNursery => "Pre-Nursery",
                ClassLevel.KG_Nursery => "Nursery",
                ClassLevel.PRI_1 => "Primary 1",
                ClassLevel.PRI_2 => "Primary 2",
                ClassLevel.PRI_3 => "Primary 3",
                ClassLevel.PRI_4 => "Primary 4",
                ClassLevel.PRI_5 => "Primary 5",
                ClassLevel.PRI_6 => "Primary 6",
                ClassLevel.JC_1 => "JSS 1",
                ClassLevel.JC_2 => "JSS 2",
                ClassLevel.JC_3 => "JSS 3",
                ClassLevel.SC_1 => "SSS 1",
                ClassLevel.SC_2 => "SSS 2",
                ClassLevel.SC_3 => "SSS 3",
                ClassLevel.None => "",
                _ => classLevel.ToString()
            };
        }

        public static string ToDisplayName(this EducationLevel educationLevel)
        {
            return educationLevel switch
            {
                EducationLevel.Kindergarten => "Kindergarten",
                EducationLevel.Primary => "Primary School",
                EducationLevel.JuniorCollege => "Junior College",
                EducationLevel.SeniorCollege => "Senior College",
                EducationLevel.None => "",
                _ => educationLevel.ToString()
            };
        }
    }
}

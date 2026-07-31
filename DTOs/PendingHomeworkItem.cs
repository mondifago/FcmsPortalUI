namespace FcmsPortalUI.DTOs
{
    public class PendingHomeworkItem
    {
        public int HomeworkId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}

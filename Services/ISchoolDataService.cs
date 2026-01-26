using FcmsPortal.Enums;
using FcmsPortal.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FcmsPortalUI.Services
{
    public interface ISchoolDataService
    {
        #region School
        School AddSchool(School school);
        bool HasSchool();
        bool HasPrincipal();
        Task UpdateSchoolAsync(School updatedSchool);
        public School? GetSchoolLearningPathsForReports();
        School? GetSchoolBasicInfo();
        School? GetSchoolForSettings();
        #endregion

        #region Staff
        Staff AddStaff(Staff staff);
        IEnumerable<Staff> GetStaff();
        Staff? GetStaffById(int id);
        List<Staff> GetTeachersByEducationLevel(EducationLevel educationLevel);
        void UpdateStaff(Staff staff);
        bool DeleteStaff(int staffId);
        string? ValidateStaffDeletion(int staffId);
        #endregion

        #region Guardians
        Guardian AddGuardian(Guardian guardian);
        IEnumerable<Guardian> GetGuardians();
        Guardian? GetGuardianById(int id);
        Guardian? GetGuardianByStudentId(int studentId);
        void UpdateGuardian(Guardian guardian);
        bool DeleteGuardian(int guardianId);
        #endregion

        #region Students
        Student AddStudent(Student student);
        IEnumerable<Student> GetStudents();
        List<Guardian> GetAllGuardians();
        Student? GetStudentById(int id);
        void UpdateStudent(Student student);
        void RemoveStudentFromAllLearningPaths(Student student);
        Task<bool> DeleteStudentAsync(int studentId);
        string? ValidateGuardianDeletion(int guardianId);
        List<Student> GetStudentsByLevel(EducationLevel educationLevel, ClassLevel classLevel);
        #endregion

        #region Learning Paths
        LearningPath AddLearningPath(LearningPath learningPath);
        void SetStudentSchoolFees(Student student, double feeAmount);
        void RemoveStudentFromLearningPath(LearningPath learningPath, Student student);
        List<LearningPath> GetLearningPathsForPayments(int academicYearStartYear, Semester semester);
        IEnumerable<LearningPath> GetAllLearningPaths();
        LearningPath? GetLearningPathById(int id);
        LearningPath? GetLearningPathForAttendanceReport(int id);
        LearningPath? GetLearningPathForGradeManagement(int id);
        LearningPath? GetLearningPathForSchedules(int id);
        LearningPath? GetLearningPathByScheduleEntry(int scheduleEntryId);
        Dictionary<int, LearningPath?> GetLearningPathsByScheduleEntries(List<int> scheduleEntryIds);
        LearningPath? GetLearningPathByClassSessionId(int classSessionId);
        void UpdateLearningPath(LearningPath learningPath);
        bool DeleteLearningPath(int id);
        void AddMultipleStudentsToLearningPath(int learningPathId, List<Student> studentsToAdd);
        void AddStudentToLearningPath(int learningPathId, Student student);
        #endregion

        #region Learning Path Templates
        void CreateTemplateFromLearningPath(LearningPath learningPath);
        string GenerateTemplateKey(EducationLevel educationLevel, ClassLevel classLevel, Semester semester);
        LearningPath? GetTemplate(EducationLevel educationLevel, ClassLevel classLevel, Semester semester);
        bool HasTemplate(EducationLevel educationLevel, ClassLevel classLevel, Semester semester);
        LearningPath? ApplyTemplateToNewLearningPath(LearningPath template, DateTime newAcademicYearStart);
        #endregion

        #region Calendar & Scheduling
        ScheduleEntry? GetLatestEventOrMeetingForDate(DateTime date);
        ScheduleEntry? GetScheduleEntryByClassSessionId(int classSessionId);
        ScheduleEntry? AddScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry);
        IEnumerable<ScheduleEntry> GetAllSchoolCalendarSchedules();
        Task<List<ScheduleEntry>> GetAllSchoolCalendarSchedulesAsync();
        bool UpdateScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry);
        bool DeleteScheduleEntry(int learningPathId, int scheduleEntryId);
        ScheduleEntry? AddGeneralScheduleEntry(ScheduleEntry scheduleEntry);
        void UpdateScheduleInSchoolCalendar(ScheduleEntry scheduleEntry);
        void RemoveScheduleFromSchoolCalendar(ScheduleEntry scheduleEntry);
        bool UpdateGeneralCalendarScheduleEntry(ScheduleEntry scheduleEntry);
        bool DeleteGeneralCalendarScheduleEntry(int scheduleEntryId);
        List<ScheduleEntry> GetAllSchedules();
        List<ScheduleEntry> GetTodayClassSessionsForStudent(int studentId, int maxCount);
        #endregion

        #region Class Sessions
        bool UpdateClassSession(ClassSession classSession);
        ClassSession? GetClassSessionById(int classSessionId);
        #endregion

        #region Homework
        Homework? GetHomeworkById(int id);
        HomeworkSubmission? SubmitHomework(int homeworkId, Student student, string answer);
        void UpdateHomework(Homework homework);
        bool DeleteHomework(int id);
        HomeworkSubmission? GetHomeworkSubmissionById(int id);
        HomeworkSubmission? AddHomeworkSubmission(HomeworkSubmission submission);
        void UpdateHomeworkSubmission(HomeworkSubmission submission);
        List<Homework> GetPendingHomeworkForStudent(int studentId, int maxCount);
        #endregion

        #region Discussions
        Task<DiscussionThread> AddDiscussionThreadAsync(int classSessionId, FirstPost firstPost);
        Task<Reply> AddReplyAsync(int threadId, int authorId, string comment);
        Task<List<DiscussionThread>> GetThreadsForClassSessionAsync(int classSessionId);
        #endregion

        #region File Attachments
        Task<FileAttachment> UploadFileAsync(IBrowserFile file, string category);
        Task DeleteFileAsync(FileAttachment attachment);
        Task<List<FileAttachment>> GetAttachmentsAsync(string category, int referenceId);
        Task SaveAttachmentReferenceAsync(FileAttachment attachment, string category, int referenceId);
        #endregion

        #region Payments
        Payment AddPayment(Payment payment);
        void UpdatePayment(Payment payment);
        void DeletePayment(int id);
        Payment PrepareNewPayment(Student student);
        SchoolFees? GetSchoolFees(int id);
        Student? GetStudentBySchoolFeesId(int schoolFeesId);
        #endregion

        #region Grading
        void SaveCourseGradingConfiguration(CourseGradingConfiguration configuration);
        CourseGradingConfiguration? GetCourseGradingConfiguration(int learningPathId, string courseName);
        List<CourseGradingConfiguration> GetAllCourseGradingConfigurations(int learningPathId);
        List<string> GetCoursesWithoutGradingConfiguration(int learningPathId);
        List<GradesReport> GetGradesReports(string academicYear, string semester);
        void SaveTestGrade(TestGrade testGrade);
        void AddTestGrade(int studentId, string course, double score, GradeType gradeType,
                  int teacherId, string teacherRemark, int learningPathId);
        int GetGradeCountByType(int learningPathId, string course, GradeType gradeType);
        Task<TestGrade> AddHomeworkSubmissionGradeAsync(
                int studentId,
                string course,
                double score,
                int teacherId,
                string teacherRemark,
                int learningPathId);
        void SaveFinalizedGrades(LearningPath learningPath);
        public List<double> GetStudentAllSemesterGrades(int studentId, EducationLevel educationLevel, ClassLevel classLevel);
        List<(string Course, GradeType GradeType, double Score)> GetRecentGradesForStudent(int studentId, int maxCount);
        #endregion

        #region Curriculum
        List<Curriculum> GetFullCurriculum();
        List<Curriculum> FilterCurriculum(List<Curriculum> curriculum, EducationLevel educationLevel, ClassLevel classLevel, Semester? semester = null);
        #endregion

        #region Attendance
        DailyAttendanceLogEntry SaveAttendance(int learningPathId, List<int> presentStudentIds, int teacherId, DateTime? attendanceDate = null);
        public LearningPath? GetLearningPathForAttendance(int id);
        bool HasAttendanceBeenTaken(int learningPathId, DateTime date);
        DailyAttendanceLogEntry? GetAttendanceForDate(int learningPathId, DateTime date);
        int GetNumberOfAttendanceDaysRecorded(int learningPathId);
        double GetDailyAttendanceAverage(int learningPathId, DateTime date);
        #endregion

        #region Student Report Cards
        StudentReportCard? GetStudentReportCard(int studentId, int learningPathId);
        List<StudentReportCard> GetStudentReportCards(int studentId);
        List<StudentReportCard> GetStudentReportCardsForLearningPath(int learningPathId);
        StudentReportCard SaveStudentReportCard(StudentReportCard reportCard);
        void UpdateStudentReportCardRemarks(int studentId, int learningPathId, string? teacherRemarks = null, string? principalRemarks = null);
        #endregion

        #region Archives
        void ArchiveStudent(Student student);
        List<Student> GetArchivedStudents();
        List<string> GetArchivedPaymentsAcademicYears();
        void ArchiveStudentPayments(LearningPath learningPath);
        List<ArchivedStudentPayment> GetArchivedStudentPayments(
            string academicYear,
            EducationLevel educationLevel,
            ClassLevel classLevel,
            Semester semester);
        List<ArchivedPaymentDetail> GetArchivedPaymentDetails(int archivedStudentPaymentId);
        void ArchiveStudentAttendance(LearningPath learningPath);
        List<AttendanceArchive> GetArchivedStudentAttendance(
           string academicYear,
           EducationLevel educationLevel,
           ClassLevel classLevel,
           Semester semester);
        List<AttendanceArchive> GetArchivedStudentAttendanceDetails(int studentId, int learningPathId);
        List<string> GetArchivedAttendanceAcademicYears();
        List<DailyAttendanceLogEntry> GetDailyAttendanceForLearningPath(int learningPathId);
        List<string> GetArchivedGradesAcademicYears();
        LearningPath? GetLearningPathByFilter(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester);
        List<string> GetGradeArchiveAcademicYears();
        ArchivedLearningPathGrade? GetArchivedLearningPathGrade(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester);
        void ArchiveStudentReportCard(StudentReportCard reportCard);
        void ArchiveLearningPathGrades(LearningPath learningPath);
        #endregion

        #region Announcements
        List<Announcement> GetAllAnnouncements();
        List<Announcement> GetActiveAnnouncements();
        Announcement CreateAnnouncement(Announcement announcement);
        Announcement UpdateAnnouncement(Announcement announcement);
        void DeleteAnnouncement(int announcementId);
        #endregion

        #region Quotes
        List<Quote> GetAllQuotes();
        Quote CreateQuote(Quote quote);
        Quote UpdateQuote(Quote quote);
        void DeleteQuote(int quoteId);
        #endregion

        #region Academic Period Management
        AcademicPeriod? GetCurrentAcademicPeriod();
        void SetSchoolAcademicPeriod(AcademicPeriod academicPeriod);
        List<string> GetArchivedLearningPathAcademicYears();
        List<ArchivedLearningPathPayment> GetArchivedLearningPathPayments(string academicYear, Semester semester);
        ArchivedSchoolPaymentSummary? GetArchivedSchoolPaymentSummary(string academicYear, Semester semester);
        void ArchiveSchoolPayments();
        void ArchiveLearningPathPayments(LearningPath lp);

        #endregion

        #region Account Management
        void ActivatePerson(Person person);
        void DeactivatePerson(Person person);
        #endregion
    }
}
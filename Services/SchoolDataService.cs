using FcmsPortal;
using FcmsPortal.Constants;
using FcmsPortal.Enums;
using FcmsPortal.Models;
using FcmsPortalUI.Data;
using FcmsPortalUI.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;


namespace FcmsPortalUI.Services
{
    public class SchoolDataService : ISchoolDataService
    {
        private readonly FcmsPortalUIContext _context;
        private readonly IDbContextFactory<FcmsPortalUIContext> _contextFactory;
        private readonly IWebHostEnvironment _environment;
        private AcademicPeriod? _cachedCurrentAcademicPeriod;
        private static int _academicPeriodVersion;
        private int _cachedAcademicPeriodVersion = -1;
        private bool _academicPeriodLoaded;

        public SchoolDataService(
            FcmsPortalUIContext context,
            IDbContextFactory<FcmsPortalUIContext> contextFactory,
            IWebHostEnvironment environment)
        {
            _context = context;
            _contextFactory = contextFactory;
            _environment = environment;
        }

        #region School
        public School? GetSchoolBasicInfo()
        {
            var school = _context.School
                .AsNoTracking()
                .Select(s => new School
                {
                    Id = s.Id,
                    Name = s.Name,
                    LogoUrl = s.LogoUrl,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    WebsiteUrl = s.WebsiteUrl,
                    Address = s.Address
                })
                .FirstOrDefault();

            return school;
        }

        public School? GetSchoolForSettings()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.School
                .Include(s => s.Address)
                .FirstOrDefault();
        }

        public bool HasSchool()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.School.Any();
        }

        public School AddSchool(School school)
        {
            if (!school.SchoolCalendar.Any())
            {
                school.SchoolCalendar.Add(new CalendarModel
                {
                    Name = $"{DateTime.Now.Year} School Calendar",
                    ScheduleEntries = new List<ScheduleEntry>()
                });
            }

            _context.School.Add(school);
            _context.SaveChanges();
            return school;
        }

        public async Task UpdateSchoolAsync(School updatedSchool)
        {
            using var context = _contextFactory.CreateDbContext();

            var existingSchool = await context.School
                .Include(s => s.Address)
                .FirstOrDefaultAsync();

            if (existingSchool == null)
                return;

            existingSchool.Name = updatedSchool.Name;
            existingSchool.LogoUrl = updatedSchool.LogoUrl;
            existingSchool.Email = updatedSchool.Email;
            existingSchool.PhoneNumber = updatedSchool.PhoneNumber;
            existingSchool.WebsiteUrl = updatedSchool.WebsiteUrl;

            if (existingSchool.Address == null && updatedSchool.Address != null)
                existingSchool.Address = new Address();

            if (existingSchool.Address != null && updatedSchool.Address != null)
            {
                existingSchool.Address.Street = updatedSchool.Address.Street;
                existingSchool.Address.City = updatedSchool.Address.City;
                existingSchool.Address.State = updatedSchool.Address.State;
                existingSchool.Address.PostalCode = updatedSchool.Address.PostalCode;
                existingSchool.Address.Country = updatedSchool.Address.Country;
            }

            await context.SaveChangesAsync();
        }

        public bool HasPrincipal()
        {
            using var context = _contextFactory.CreateDbContext();

            int? principalRoleId = context.Roles
                        .Where(r => r.Name == "Principal")
                .Select(r => (int?)r.Id)
                .SingleOrDefault();

            if (principalRoleId == null)
                return false;

            return _context.UserRoles.Any(ur => ur.RoleId == principalRoleId.Value);
        }
        #endregion

        #region Staff
        public IEnumerable<Staff> GetStaff()
        {
            return _context.Staff
                .Include(st => st.Person)
                .ToList();
        }

        public Staff? GetStaffById(int id)
        {
            return _context.Staff
                .Include(st => st.Person)
                .FirstOrDefault(st => st.Id == id);
        }

        public Staff? GetStaffByPersonId(int personId)
        {
            return _context.Staff
                .AsNoTracking()
                .Include(st => st.Person)
                .FirstOrDefault(st => st.PersonId == personId);
        }

        public List<Staff> GetTeachersByEducationLevel(EducationLevel educationLevel)
        {
            return _context.Staff
                .AsNoTracking()
                .Include(st => st.Person)
                .Where(st => st.UserRole == UserRole.Teacher &&
                             st.Person.EducationLevel == educationLevel)
                .ToList();
        }

        public Staff AddStaff(Staff staff)
        {
            var school = _context.School.FirstOrDefault();
            if (school == null)
                throw new InvalidOperationException("No school found. Cannot add staff without a school.");

            staff.SchoolId = school.Id;
            staff.School = school;
            _context.Staff.Add(staff);
            _context.SaveChanges();
            return staff;
        }


        public void UpdateStaff(Staff staff)
        {
            var existingStaff = _context.Staff.Include(s => s.Person).FirstOrDefault(s => s.Id == staff.Id);
            if (existingStaff != null)
            {
                existingStaff.UserRole = staff.UserRole;
                existingStaff.DateOfEmployment = staff.DateOfEmployment;
                existingStaff.JobDescription = staff.JobDescription;
                existingStaff.WorkExperience = staff.WorkExperience;
                existingStaff.AreaOfSpecialization = staff.AreaOfSpecialization;
                existingStaff.Qualifications = staff.Qualifications;
                existingStaff.Person.ProfilePictureUrl = staff.Person.ProfilePictureUrl;
                existingStaff.Person.FirstName = staff.Person.FirstName;
                existingStaff.Person.MiddleName = staff.Person.MiddleName;
                existingStaff.Person.LastName = staff.Person.LastName;
                existingStaff.Person.Sex = staff.Person.Sex;
                existingStaff.Person.StateOfOrigin = staff.Person.StateOfOrigin;
                existingStaff.Person.LgaOfOrigin = staff.Person.LgaOfOrigin;
                existingStaff.Person.Email = staff.Person.Email;
                existingStaff.Person.PhoneNumber = staff.Person.PhoneNumber;
                existingStaff.Person.DateOfBirth = staff.Person.DateOfBirth;
                existingStaff.Person.EmergencyContact = staff.Person.EmergencyContact;
                existingStaff.Person.EducationLevel = staff.Person.EducationLevel;
                existingStaff.Person.IsActive = staff.Person.IsActive;
                existingStaff.Person.Address = staff.Person.Address;

                _context.SaveChanges();
            }
        }

        public bool DeleteStaff(int staffId)
        {
            var staff = _context.Staff
                .Include(s => s.Person)
                .FirstOrDefault(s => s.Id == staffId);

            if (staff == null)
            {
                return false;
            }

            var person = staff.Person;

            _context.Staff.Remove(staff);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            _context.SaveChanges();
            return true;
        }

        public string? ValidateStaffDeletion(int staffId)
        {
            var staff = _context.Staff
                .Include(s => s.Person)
                .FirstOrDefault(s => s.Id == staffId);

            if (staff == null)
            {
                return "Staff not found.";
            }

            if (staff.UserRole == UserRole.Principal)
            {
                var principalCount = _context.Staff
                    .Count(s => s.UserRole == UserRole.Principal &&
                                s.Person.Email != "developer@fcms.system" &&
                                s.Person.Email != "principal@fcms.system");

                if (principalCount <= 1)
                {
                    return $"Cannot delete {staff.Person.FirstName} {staff.Person.LastName}. This is the only Principal account. At least one Principal must exist in the system.";
                }
            }

            var currentPeriod = GetCurrentAcademicPeriod();
            if (currentPeriod == null)
            {
                return null;
            }

            var assignedClasses = _context.ClassSessions
                 .AsNoTracking()
                 .Where(cs => cs.TeacherId == staffId && cs.Semester == currentPeriod.Semester)
                 .Select(cs => cs.ClassLevel)
                 .ToList();

            if (assignedClasses.Any())
            {
                var staffName = $"{staff.Person.FirstName} {staff.Person.LastName}";
                var classNames = string.Join(", ", assignedClasses.Distinct().Select(classLevel => classLevel.ToDisplayName()));

                return $"Cannot delete {staffName}. This staff member is assigned to {assignedClasses.Count} class session(s) this term ({currentPeriod.Semester} {currentPeriod.AcademicYear}) for: {classNames}. Please remove the staff from all assigned class sessions before deleting.";
            }

            return null;
        }

        public List<Staff> GetTeachers()
        {
            return _context.Staff
                .AsNoTracking()
                .Include(st => st.Person)
                .Where(st => st.UserRole == UserRole.Teacher)
                .OrderBy(st => st.Person.FirstName)
                .ToList();
        }
        #endregion

        #region Guardians
        public IEnumerable<Guardian> GetGuardians()
        {
            return _context.Guardians
                .Include(g => g.Person)
                .Include(g => g.Wards)
                    .ThenInclude(w => w.Person)
                .ToList();
        }

        public List<Guardian> GetAllGuardians()
        {
            return _context.Guardians
                .Include(g => g.Person)
                .AsNoTracking()
                .ToList();
        }

        public Guardian? GetGuardianById(int id)
        {
            return _context.Guardians
                .Include(g => g.Person)
                .Include(g => g.Wards)
                    .ThenInclude(w => w.Person)
                .FirstOrDefault(g => g.Id == id);
        }

        public Guardian? GetGuardianByPersonId(int personId)
        {
            return _context.Guardians
                .Include(g => g.Person)
                .Include(g => g.Wards)
                    .ThenInclude(w => w.Person)
                .FirstOrDefault(g => g.PersonId == personId);
        }

        public Guardian? GetGuardianByStudentId(int studentId)
        {
            return _context.Guardians
                .Include(g => g.Person)
                .Include(g => g.Wards)
                .FirstOrDefault(g => g.Wards.Any(w => w.Id == studentId));
        }

        public void UpdateGuardian(Guardian guardian)
        {
            var existingGuardian = _context.Guardians
                .Include(g => g.Person)
                .FirstOrDefault(g => g.Id == guardian.Id);

            if (existingGuardian != null)
            {
                existingGuardian.Person.FirstName = guardian.Person.FirstName;
                existingGuardian.Person.MiddleName = guardian.Person.MiddleName;
                existingGuardian.Person.LastName = guardian.Person.LastName;
                existingGuardian.Person.Sex = guardian.Person.Sex;
                existingGuardian.Person.StateOfOrigin = guardian.Person.StateOfOrigin;
                existingGuardian.Person.LgaOfOrigin = guardian.Person.LgaOfOrigin;
                existingGuardian.Person.Email = guardian.Person.Email;
                existingGuardian.Person.PhoneNumber = guardian.Person.PhoneNumber;
                existingGuardian.Person.DateOfEnrollment = guardian.Person.DateOfEnrollment;
                existingGuardian.Person.DateOfBirth = guardian.Person.DateOfBirth;
                existingGuardian.Occupation = guardian.Occupation;
                existingGuardian.RelationshipToStudent = guardian.RelationshipToStudent;
                existingGuardian.Person.ProfilePictureUrl = guardian.Person.ProfilePictureUrl;
                existingGuardian.Person.IsActive = guardian.Person.IsActive;
                existingGuardian.Person.Address = guardian.Person.Address;

                _context.SaveChanges();
            }
        }

        public Guardian AddGuardian(Guardian guardian)
        {
            var school = _context.School.FirstOrDefault();
            if (school == null)
                throw new InvalidOperationException("No school found. Cannot add guardian without a school.");

            guardian.SchoolId = school.Id;
            guardian.School = school;
            _context.Guardians.Add(guardian);
            _context.SaveChanges();
            return guardian;
        }

        public bool DeleteGuardian(int guardianId)
        {
            var guardian = _context.Guardians
                .Include(g => g.Person)
                .FirstOrDefault(g => g.Id == guardianId);

            if (guardian == null)
            {
                return false;
            }

            var person = guardian.Person;

            _context.Guardians.Remove(guardian);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            _context.SaveChanges();
            return true;
        }

        public string? ValidateGuardianDeletion(int guardianId)
        {
            var guardian = _context.Guardians
                .Include(g => g.Person)
                .Include(g => g.Wards)
                .FirstOrDefault(g => g.Id == guardianId);

            if (guardian == null)
            {
                return "Guardian not found.";
            }

            if (guardian.Wards != null && guardian.Wards.Any())
            {
                var wardNames = string.Join(", ", guardian.Wards.Select(w => $"{w.Person.FirstName} {w.Person.LastName}"));
                return $"Cannot delete guardian {guardian.Person.FirstName} {guardian.Person.LastName}. They still have the following wards: {wardNames}. Please reassign or remove these wards first.";
            }

            return null;
        }
        #endregion

        #region Students
        public Student? GetStudentById(int id)
        {
            return _context.Students
                .Include(s => s.Person)
                .Include(s => s.SchoolFees)
                    .ThenInclude(sf => sf.Payments)
                .Include(s => s.CourseGrades)
                .Include(s => s.LearningPath)
                .Include(s => s.Guardian)
                    .ThenInclude(g => g.Person)
                    .AsSplitQuery()
                .FirstOrDefault(s => s.Id == id);
        }

        public Student? GetStudentByPersonId(int personId)
        {
            return _context.Students
                .Include(s => s.Person)
                .FirstOrDefault(s => s.PersonId == personId);
        }

        public void UpdateStudent(Student student)
        {
            var existingStudent = _context.Students
                .Include(s => s.Person)


                .FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent != null)
            {
                existingStudent.Person.FirstName = student.Person.FirstName;
                existingStudent.Person.MiddleName = student.Person.MiddleName;
                existingStudent.Person.LastName = student.Person.LastName;
                existingStudent.Person.ProfilePictureUrl = student.Person.ProfilePictureUrl;
                existingStudent.Person.DateOfBirth = student.Person.DateOfBirth;
                existingStudent.Person.EducationLevel = student.Person.EducationLevel;
                existingStudent.Person.ClassLevel = student.Person.ClassLevel;
                existingStudent.Person.Sex = student.Person.Sex;

                existingStudent.Person.StateOfOrigin = student.Person.StateOfOrigin;
                existingStudent.Person.LgaOfOrigin = student.Person.LgaOfOrigin;
                existingStudent.Person.DateOfEnrollment = student.Person.DateOfEnrollment;
                existingStudent.Person.EmergencyContact = student.Person.EmergencyContact;
                existingStudent.Person.Email = student.Person.Email;
                existingStudent.Person.PhoneNumber = student.Person.PhoneNumber;
                existingStudent.Person.IsActive = student.Person.IsActive;
                existingStudent.Person.Address = student.Person.Address;
                existingStudent.PositionAmongSiblings = student.PositionAmongSiblings;
                existingStudent.LastSchoolAttended = student.LastSchoolAttended;
                existingStudent.GuardianId = student.GuardianId;

                _context.SaveChanges();
            }
        }

        public Student AddStudent(Student student)
        {
            if (student.SchoolId == null)
            {
                var school = _context.School.FirstOrDefault();
                if (school != null)
                {
                    student.SchoolId = school.Id;
                }
            }

            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }

        public void RemoveStudentFromAllLearningPaths(Student student)
        {
            var existingStudent = _context.Students.FirstOrDefault(s => s.Id == student.Id);
            if (existingStudent == null)
                return;

            existingStudent.LearningPathId = null;
            _context.SaveChanges();
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = _context.Students
                .Include(s => s.Person)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                return false;
            }

            // Block deletion if student is enrolled in any active learning path
            var activeLearningPath = _context.LearningPaths
                .AsNoTracking()
                .Where(lp => lp.ApprovalStatus != PrincipalApprovalStatus.Approved)
                .FirstOrDefault(lp => lp.Students.Any(s => s.Id == studentId));

            if (activeLearningPath != null)
            {
                throw new BusinessRuleException(
                    $"Cannot delete a student enrolled in an active learning path " +
                    $"({activeLearningPath.ClassLevel} - {activeLearningPath.Semester}). " +
                    "Please wait until the semester is finalized and approved.");
            }

            // Delete profile picture if exists
            if (!string.IsNullOrEmpty(student.Person?.ProfilePictureUrl) &&
                !student.Person.ProfilePictureUrl.StartsWith("data:"))
            {
                var attachment = new FileAttachment { FilePath = student.Person.ProfilePictureUrl };
                await DeleteFileAsync(attachment);
            }

            var person = student.Person;
            _context.Students.Remove(student);

            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public List<Student> GetStudentsByLevel(EducationLevel educationLevel, ClassLevel classLevel)
        {
            return _context.Students
                .Include(s => s.Person)
                .Where(s => s.Person.EducationLevel == educationLevel &&
                            s.Person.ClassLevel == classLevel)
                .ToList();
        }
        #endregion

        #region Learning Paths
        public LearningPath AddLearningPath(LearningPath learningPath)
        {
            var school = _context.School.FirstOrDefault();
            if (school == null)
                throw new InvalidOperationException("No school found. Cannot add learning path without a school.");

            learningPath.SchoolId = school.Id;
            learningPath.School = school;
            _context.LearningPaths.Add(learningPath);
            _context.SaveChanges();
            return learningPath;
        }

        public int? GetCurrentLearningPathId(ClassLevel classLevel, Semester semester)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(lp => lp.ClassLevel == classLevel && lp.Semester == semester)
                .OrderByDescending(lp => lp.AcademicYearStart)
                .Select(lp => (int?)lp.Id)
                .FirstOrDefault();
        }

        public void RemoveStudentFromLearningPath(LearningPath learningPath, Student student)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            var enrolledStudent = existingLearningPath.Students
                .FirstOrDefault(candidate => candidate.Id == student.Id);

            if (enrolledStudent == null)
                return;

            bool hasGrades = _context.CourseGrades
                .Any(courseGrade => courseGrade.StudentId == student.Id
                                 && courseGrade.LearningPathId == existingLearningPath.Id
                                 && courseGrade.TestGrades.Any());

            if (hasGrades)
            {
                throw new BusinessRuleException(
                    $"{Util.GetFullName(student.Person)} has grades recorded for " +
                    $"{Util.GetLearningPathName(existingLearningPath)} and cannot be removed from it. ");
            }

            bool hasAttendance = _context.DailyAttendanceLogEntries
                .Any(log => log.LearningPathId == existingLearningPath.Id
                         && (log.PresentStudents.Any(candidate => candidate.Id == student.Id)
                          || log.AbsentStudents.Any(candidate => candidate.Id == student.Id)));

            if (hasAttendance)
            {
                throw new BusinessRuleException(
                    $"{Util.GetFullName(student.Person)} has attendance recorded for " +
                    $"{Util.GetLearningPathName(existingLearningPath)} and cannot be removed from it.");
            }

            existingLearningPath.Students.Remove(enrolledStudent);

            _context.SaveChanges();
        }

        public List<LearningPath> GetLearningPathsForPeriod(int academicPeriodId)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(learningPath => learningPath.AcademicPeriodId == academicPeriodId)
                .ToList();
        }

        public List<SchoolFees> GetSchoolFeesForPeriodStudents(int academicPeriodId)
        {
            var learningPathIds = _context.LearningPaths
                .AsNoTracking()
                .Where(learningPath => learningPath.AcademicPeriodId == academicPeriodId)
                .Select(learningPath => learningPath.Id)
                .ToList();

            var studentIds = _context.SchoolFees
                .AsNoTracking()
                .Where(fees => learningPathIds.Contains(fees.LearningPathId))
                .Select(fees => fees.StudentId)
                .Distinct()
                .ToList();

            return _context.SchoolFees
                .AsNoTracking()
                .Where(fees => studentIds.Contains(fees.StudentId))
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .ToList();
        }

        public List<SchoolFees> GetSchoolFeesForLearningPath(int learningPathId)
        {
            return _context.SchoolFees
                .AsNoTracking()
                .Where(fees => fees.LearningPathId == learningPathId)
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .ToList();
        }

        public List<SchoolFees> GetSchoolFeesForStudent(int studentId)
        {
            return _context.SchoolFees
                .AsNoTracking()
                .Where(fees => fees.StudentId == studentId)
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .ToList();
        }

        public LearningPath? GetLearningPathForDetails(int id)
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                        
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.TestGrades)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.GradingConfiguration)
                .AsSplitQuery()
                .FirstOrDefault(lp => lp.Id == id);
        }

        public LearningPath? GetLearningPathBasicInfo(int id)
        {
            return _context.LearningPaths.FirstOrDefault(lp => lp.Id == id);
        }

        public LearningPath? GetLearningPathForAttendanceReport(int id)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Include(lp => lp.AttendanceLog)
                    .ThenInclude(al => al.PresentStudents)
                .Include(lp => lp.AttendanceLog)
                    .ThenInclude(al => al.AbsentStudents)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .FirstOrDefault(lp => lp.Id == id);
        }

        public LearningPath? GetLearningPathForGradeManagement(int id)
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.TestGrades)
                            .ThenInclude(tg => tg.Teacher)
                                .ThenInclude(t => t.Person)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.GradingConfiguration)
                .Include(lp => lp.AttendanceLog)
                    .ThenInclude(al => al.PresentStudents)
                .AsSplitQuery()
                .FirstOrDefault(lp => lp.Id == id);
        }

        public bool LearningPathCombinationExists(int excludeId, EducationLevel educationLevel, ClassLevel classLevel, int academicPeriodId)
        {
            return _context.LearningPaths
                .Any(lp => lp.Id != excludeId &&
                           lp.EducationLevel == educationLevel &&
                           lp.ClassLevel == classLevel &&
                           lp.AcademicPeriodId == academicPeriodId);
        }
        public LearningPath? GetLearningPathWithAttendanceByClassSessionId(int classSessionId, DateTime sessionDate)
        {
            var learningPathId = GetLearningPathIdByClassSessionId(classSessionId);

            if (learningPathId == null)
                return null;

            var targetDate = sessionDate.Date;

            return _context.LearningPaths
                .AsNoTracking()
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.AttendanceLog.Where(al => al.TimeStamp.Date == targetDate))
                    .ThenInclude(al => al.PresentStudents)
                .Include(lp => lp.AttendanceLog.Where(al => al.TimeStamp.Date == targetDate))
                    .ThenInclude(al => al.AbsentStudents)
                .AsSplitQuery()
                .FirstOrDefault(lp => lp.Id == learningPathId);
        }

        public int? GetLearningPathIdByClassSessionId(int classSessionId)
        {
            var classSession = _context.ClassSessions
                .AsNoTracking()
                .Select(cs => new { cs.Id, cs.ClassLevel, cs.Semester })
                .FirstOrDefault(cs => cs.Id == classSessionId);

            if (classSession == null)
                return null;

            return GetCurrentLearningPathId(classSession.ClassLevel, classSession.Semester);
        }

        public async Task<bool> DeleteLearningPathAsync(int id)
        {
            var learningPath = await _context.LearningPaths
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (learningPath == null)
            {
                return false;
            }

            var blocker = GetLearningPathDeletionBlocker(learningPath);
            if (blocker != null)
            {
                throw new BusinessRuleException(
                    $"{Util.GetLearningPathName(learningPath)} cannot be deleted because {blocker}.");
            }

            _context.LearningPaths.Remove(learningPath);
            await _context.SaveChangesAsync();

            return true;
        }

        private string? GetLearningPathDeletionBlocker(LearningPath learningPath)
        {
            var statusBlocker = learningPath.ApprovalStatus switch
            {
                PrincipalApprovalStatus.Review => "it has been submitted for approval",
                PrincipalApprovalStatus.Approved => "it has already been approved",
                _ => null
            };

            if (statusBlocker != null)
                return statusBlocker;

            if (_context.Students.Any(s => s.LearningPathId == learningPath.Id))
                return "students are enrolled in it";

            if (_context.CourseGrades.Any(cg => cg.LearningPathId == learningPath.Id && cg.TestGrades.Any()))
                return "grades have been recorded for it";

            if (_context.DailyAttendanceLogEntries.Any(a => a.LearningPathId == learningPath.Id))
                return "attendance has been taken for it";

            if (_context.Payments.Any(p => p.LearningPathId == learningPath.Id))
                return "school fee payments have been made for it";

            if (_context.StudentReportCards.Any(rc => rc.LearningPathId == learningPath.Id))
                return "report cards have been generated for it";

            return null;
        }

        public void UpdateLearningPath(LearningPath learningPath)
        {
            var existingLearningPath = _context.LearningPaths
                   .Include(lp => lp.Students)
                   .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath != null)
            {
                if (LogicMethods.IsLearningPathReadOnly(existingLearningPath.ApprovalStatus))
                {
                    throw new BusinessRuleException(
                        $"{Util.GetLearningPathName(existingLearningPath)} has been approved and can no longer be edited.");
                }

                existingLearningPath.SemesterStartDate = learningPath.SemesterStartDate;
                existingLearningPath.SemesterEndDate = learningPath.SemesterEndDate;
                existingLearningPath.ExamsStartDate = learningPath.ExamsStartDate;
                existingLearningPath.FeePerSemester = learningPath.FeePerSemester;
                existingLearningPath.ApprovalStatus = learningPath.ApprovalStatus;
                existingLearningPath.EducationLevel = learningPath.EducationLevel;
                existingLearningPath.ClassLevel = learningPath.ClassLevel;
                existingLearningPath.Semester = learningPath.Semester;
                existingLearningPath.AcademicYearStart = learningPath.AcademicYearStart;
                existingLearningPath.SubmittedById = learningPath.SubmittedById;
                existingLearningPath.SubmittedByName = learningPath.SubmittedByName;
                existingLearningPath.DateSubmitted = learningPath.DateSubmitted;

                _context.SaveChanges();
            }
        }

        public double AddStudentToLearningPath(int learningPathId, Student student)
        {
            if (learningPathId == 0)
                throw new ArgumentNullException(nameof(learningPathId));
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existingLearningPath = _context.LearningPaths
             .Include(lp => lp.Students)
             .FirstOrDefault(lp => lp.Id == learningPathId);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (LogicMethods.IsLearningPathReadOnly(existingLearningPath.ApprovalStatus))
            {
                throw new BusinessRuleException(
                    $"{Util.GetLearningPathName(existingLearningPath)} has been approved. Students can no longer be added to it.");
            }

            if (existingLearningPath.Students == null)
                existingLearningPath.Students = new List<Student>();

            double amountTransferred = 0;

            if (!existingLearningPath.Students.Any(s => s.Id == student.Id))
            {
                existingLearningPath.Students.Add(student);
                amountTransferred = EnsureSchoolFeesForEnrolment(student.Id, existingLearningPath.Id);

                _context.SaveChanges();
            }

            return amountTransferred;
        }

        public double AddMultipleStudentsToLearningPath(int learningPathId, List<Student> studentsToAdd)
        {
            if (learningPathId == 0)
                throw new ArgumentNullException(nameof(learningPathId));
            if (studentsToAdd == null || !studentsToAdd.Any())
                throw new ArgumentException("Students list cannot be null or empty.", nameof(studentsToAdd));

            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                .FirstOrDefault(lp => lp.Id == learningPathId);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (LogicMethods.IsLearningPathReadOnly(existingLearningPath.ApprovalStatus))
            {
                throw new BusinessRuleException(
                    $"{Util.GetLearningPathName(existingLearningPath)} has been approved. Students can no longer be added to it.");
            }

            if (existingLearningPath.Students == null)
                existingLearningPath.Students = new List<Student>();

            bool hasChanges = false;
            double amountTransferred = 0;

            foreach (var student in studentsToAdd)
            {
                if (student == null)
                    continue;

                if (!existingLearningPath.Students.Any(s => s.Id == student.Id))
                {
                    existingLearningPath.Students.Add(student);
                    amountTransferred += EnsureSchoolFeesForEnrolment(student.Id, existingLearningPath.Id);

                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                _context.SaveChanges();
            }

            return amountTransferred;
        }

        public void ApproveLearningPath(int learningPathId)
        {
            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)

                .FirstOrDefault(lp => lp.Id == learningPathId);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (LogicMethods.IsLearningPathReadOnly(existingLearningPath.ApprovalStatus))
            {
                throw new BusinessRuleException(
                    $"{Util.GetLearningPathName(existingLearningPath)} has already been approved.");
            }

            if (!AreLearningPathGradesFinalized(learningPathId))
            {
                throw new BusinessRuleException(
                    $"{Util.GetLearningPathName(existingLearningPath)} cannot be approved because its grades have not been finalized. Open 'Finalize Grade' and click 'Finalize All Grades'");
            }

            foreach (var student in existingLearningPath.Students.ToList())
            {
                existingLearningPath.Students.Remove(student);
            }

            existingLearningPath.ApprovalStatus = PrincipalApprovalStatus.Approved;

            _context.SaveChanges();
        }
        #endregion

        #region Calendar & Scheduling
        public IEnumerable<ScheduleEntry> GetAllSchoolCalendarSchedules()
        {
            return _context.ScheduleEntries
                .AsNoTracking()
                .ToList();
        }

        public async Task<List<ScheduleEntry>> GetAllSchoolCalendarSchedulesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.ScheduleEntries
                .AsNoTracking()
                .OrderBy(se => se.DateTime)
                .ToListAsync();
        }

        public ScheduleEntry? AddGeneralScheduleEntry(ScheduleEntry scheduleEntry)
        {
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            if (school == null)
                return null;

            var mainCalendar = school.SchoolCalendar.FirstOrDefault();
            if (mainCalendar == null)
            {
                mainCalendar = new CalendarModel
                {
                    Name = "Main School Calendar",
                    ScheduleEntries = new List<ScheduleEntry>()
                };
                school.SchoolCalendar.Add(mainCalendar);
            }

            if (mainCalendar.ScheduleEntries == null)
            {
                mainCalendar.ScheduleEntries = new List<ScheduleEntry>();
            }

            if (scheduleEntry.Id > 0)
            {
                var existingSchedule = mainCalendar.ScheduleEntries.FirstOrDefault(s => s.Id == scheduleEntry.Id);
                if (existingSchedule == null)
                {
                    mainCalendar.ScheduleEntries.Add(scheduleEntry);
                }
                _context.SaveChanges();
                return scheduleEntry;
            }

            if (scheduleEntry.IsRecurring)
            {
                var recurringEntries = LogicMethods.GenerateRecurringSchedules(scheduleEntry);
                foreach (var entry in recurringEntries)
                {
                    mainCalendar.ScheduleEntries.Add(entry);
                }
                _context.SaveChanges();
                return recurringEntries.FirstOrDefault();
            }
            else
            {
                mainCalendar.ScheduleEntries.Add(scheduleEntry);
                _context.SaveChanges();
                return scheduleEntry;
            }
        }

        public void UpdateScheduleInSchoolCalendar(ScheduleEntry scheduleEntry)
        {
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            if (school?.SchoolCalendar == null)
                return;

            foreach (var calendar in school.SchoolCalendar)
            {
                if (calendar.ScheduleEntries != null)
                {
                    var existingIndex = calendar.ScheduleEntries.FindIndex(s => s.Id == scheduleEntry.Id);
                    if (existingIndex >= 0)
                    {
                        calendar.ScheduleEntries[existingIndex] = scheduleEntry;
                        _context.SaveChanges();
                        break;
                    }
                }
            }
        }


        public void RemoveScheduleFromSchoolCalendar(ScheduleEntry scheduleEntry)
        {
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            if (school?.SchoolCalendar == null)
                return;

            foreach (var calendar in school.SchoolCalendar)
            {
                if (calendar.ScheduleEntries != null)
                {
                    var scheduleToRemove = calendar.ScheduleEntries.FirstOrDefault(s => s.Id == scheduleEntry.Id);
                    if (scheduleToRemove != null)
                    {
                        calendar.ScheduleEntries.Remove(scheduleToRemove);
                        _context.SaveChanges();
                        break;
                    }
                }
            }
        }

        public bool UpdateGeneralCalendarScheduleEntry(ScheduleEntry scheduleEntry)
        {
            using var context = _contextFactory.CreateDbContext();

            var existing = context.ScheduleEntries
                .FirstOrDefault(se => se.Id == scheduleEntry.Id);

            if (existing == null)
                return false;

            context.Entry(existing).CurrentValues.SetValues(scheduleEntry);

            context.SaveChanges();
            return true;
        }

        public bool DeleteGeneralCalendarScheduleEntry(int scheduleEntryId)
        {
            var scheduleEntry = _context.ScheduleEntries
                .FirstOrDefault(s => s.Id == scheduleEntryId);

            if (scheduleEntry == null)
                return false;

            _context.ScheduleEntries.Remove(scheduleEntry);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Class Schedules
        public List<ClassSchedule> GetClassSchedules(ClassLevel classLevel, Semester semester)
        {
            return _context.ClassSchedules
                .AsNoTracking()
                .Where(cs => cs.ClassLevel == classLevel && cs.Semester == semester)
                .Include(cs => cs.ClassSession)
                .OrderBy(cs => cs.DateTime)
                .ToList();
        }

        public ClassSchedule? GetClassScheduleByClassSessionId(int classSessionId)
        {
            return _context.ClassSchedules
                .AsNoTracking()
                .FirstOrDefault(cs => cs.ClassSessionId == classSessionId);
        }

        public ClassSchedule CreateClassSchedule(ClassSchedule classSchedule)
        {
            _context.ClassSchedules.Add(classSchedule);
            _context.SaveChanges();
            return classSchedule;
        }

        public List<ClassSchedule> CreateClassSchedules(ClassSchedule template, List<DateTime> occurrences)
        {
            var created = occurrences
                .Select(occurrence => new ClassSchedule
                {
                    ClassLevel = template.ClassLevel,
                    Semester = template.Semester,
                    DateTime = occurrence,
                    Duration = template.Duration,
                    Venue = template.Venue
                })
                .ToList();

            _context.ClassSchedules.AddRange(created);
            _context.SaveChanges();
            return created;
        }

        public bool UpdateClassSchedule(ClassSchedule classSchedule)
        {
            var existing = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == classSchedule.Id);

            if (existing == null)
                return false;

            existing.DateTime = classSchedule.DateTime;
            existing.Duration = classSchedule.Duration;
            existing.Venue = classSchedule.Venue;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteClassSchedule(int classScheduleId)
        {
            var classSchedule = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == classScheduleId);

            if (classSchedule == null)
                return false;

            if (classSchedule.ClassSessionId.HasValue)
                throw new BusinessRuleException("This schedule holds a class session. Remove the session from it first.");

            _context.ClassSchedules.Remove(classSchedule);
            _context.SaveChanges();
            return true;
        }

        public bool PlaceSession(int classScheduleId, int classSessionId)
        {
            var classSchedule = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == classScheduleId);

            if (classSchedule == null)
                return false;

            var classSession = _context.ClassSessions
                .AsNoTracking()
                .Select(cs => new { cs.Id, cs.ClassLevel, cs.Semester })
                .FirstOrDefault(cs => cs.Id == classSessionId);

            if (classSession == null)
                return false;

            if (classSchedule.ClassSessionId.HasValue)
                throw new BusinessRuleException("This schedule already holds a class session.");

            if (classSession.ClassLevel != classSchedule.ClassLevel || classSession.Semester != classSchedule.Semester)
                throw new BusinessRuleException("A session can only be placed in a schedule of its own class and term.");

            if (_context.ClassSchedules.Any(cs => cs.ClassSessionId == classSessionId))
                throw new BusinessRuleException("This session is already on the timetable.");

            classSchedule.ClassSessionId = classSessionId;
            _context.SaveChanges();
            return true;
        }

        public bool UnplaceSession(int classScheduleId)
        {
            var classSchedule = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == classScheduleId);

            if (classSchedule == null)
                return false;

            classSchedule.ClassSessionId = null;
            _context.SaveChanges();
            return true;
        }

        public bool MoveSession(int fromClassScheduleId, int toClassScheduleId)
        {
            var from = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == fromClassScheduleId);
            var to = _context.ClassSchedules.FirstOrDefault(cs => cs.Id == toClassScheduleId);

            if (from == null || to == null || !from.ClassSessionId.HasValue)
                return false;

            if (to.ClassSessionId.HasValue)
                throw new BusinessRuleException("The target schedule already holds a class session.");

            if (to.ClassLevel != from.ClassLevel || to.Semester != from.Semester)
                throw new BusinessRuleException("A session can only be moved within its own class and term.");

            var classSessionId = from.ClassSessionId.Value;
            from.ClassSessionId = null;
            _context.SaveChanges();

            to.ClassSessionId = classSessionId;
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Class Sessions
        public ClassSession? GetClassSessionById(int classSessionId)
        {
            return _context.ClassSessions
                .Include(cs => cs.Teacher)
                    .ThenInclude(t => t.Person)
                .Include(cs => cs.StudyMaterials)
                .Include(cs => cs.HomeworkDetails)
                    .ThenInclude(h => h.Submissions)
                        .ThenInclude(s => s.Student)
                            .ThenInclude(st => st.Person)
                .Include(cs => cs.HomeworkDetails)
                    .ThenInclude(h => h.Submissions)
                        .ThenInclude(s => s.HomeworkGrade)
                .Include(cs => cs.DiscussionThreads)
                    .ThenInclude(dt => dt.FirstPost)
                        .ThenInclude(fp => fp.Author)
                .Include(cs => cs.DiscussionThreads)
                    .ThenInclude(dt => dt.Replies)
                        .ThenInclude(r => r.Author)
                .AsSplitQuery()
                .FirstOrDefault(cs => cs.Id == classSessionId);
        }

        public bool UpdateClassSession(ClassSession updated)
        {
            var existing = _context.ClassSessions.Local.FirstOrDefault(c => c.Id == updated.Id)
                           ?? _context.ClassSessions.FirstOrDefault(c => c.Id == updated.Id);

            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(updated);

            _context.SaveChanges();

            return true;
        }

        public List<ClassSessionListItem> GetClassSessionList(ClassLevel classLevel, Semester semester)
        {
            return _context.ClassSessions
                .AsNoTracking()
                .Where(cs => cs.ClassLevel == classLevel && cs.Semester == semester)
                .OrderBy(cs => cs.Course)
                .ThenBy(cs => cs.SessionNumber)
                .Select(cs => new ClassSessionListItem
                {
                    Id = cs.Id,
                    SessionNumber = cs.SessionNumber,
                    Course = cs.Course,
                    Topic = cs.Topic,
                    TeacherName = cs.Teacher == null
                        ? null
                        : cs.Teacher.Person.FirstName + " " + cs.Teacher.Person.LastName,
                    ScheduledAt = _context.ClassSchedules
                        .Where(sched => sched.ClassSessionId == cs.Id)
                        .Select(sched => (DateTime?)sched.DateTime)
                        .FirstOrDefault(),
                    ScheduledEnd = _context.ClassSchedules
                        .Where(sched => sched.ClassSessionId == cs.Id)
                        .Select(sched => (DateTime?)sched.DateTime.Add(sched.Duration))
                        .FirstOrDefault(),
                    ClosedAt = cs.ClosedAt
                })
                .ToList();
        }

        public ClassSession? GetClassSessionForEdit(int classSessionId)
        {
            return _context.ClassSessions
                .AsNoTracking()
                .FirstOrDefault(cs => cs.Id == classSessionId);
        }

        public int GetNextSessionNumber(ClassLevel classLevel, Semester semester, string course)
        {
            var highestSessionNumber = _context.ClassSessions
                .AsNoTracking()
                .Where(cs => cs.ClassLevel == classLevel && cs.Semester == semester && cs.Course == course)
                .Max(cs => (int?)cs.SessionNumber);

            return LogicMethods.GetNextSessionNumber(highestSessionNumber);
        }

        public ClassSession AddClassSession(ClassSession classSession)
        {
            classSession.SessionNumber = GetNextSessionNumber(classSession.ClassLevel, classSession.Semester, classSession.Course);
            _context.ClassSessions.Add(classSession);
            _context.SaveChanges();
            return classSession;
        }

        public void UpdateClassSessionDetails(ClassSession edited)
        {
            var existing = _context.ClassSessions.FirstOrDefault(cs => cs.Id == edited.Id);

            if (existing == null)
                return;

            var courseSessions = LoadCourseSessions(existing.ClassLevel, existing.Semester, existing.Course);
            var orderedIds = courseSessions.Select(cs => cs.Id).ToList();

            if (existing.Course == edited.Course)
            {
                ApplySessionNumbers(courseSessions, LogicMethods.MoveInSequence(orderedIds, existing.Id, edited.SessionNumber));
            }
            else
            {
                ApplySessionNumbers(courseSessions, LogicMethods.RemoveFromSequence(orderedIds, existing.Id));

                var newCourseSessions = LoadCourseSessions(existing.ClassLevel, existing.Semester, edited.Course);
                newCourseSessions.Add(existing);
                var newOrderedIds = newCourseSessions.Select(cs => cs.Id).ToList();
                ApplySessionNumbers(newCourseSessions, LogicMethods.MoveInSequence(newOrderedIds, existing.Id, newOrderedIds.Count));

                existing.Course = edited.Course;
            }

            existing.Topic = edited.Topic;
            existing.Description = edited.Description;
            existing.TeacherId = edited.TeacherId;

            _context.SaveChanges();
        }

        public async Task DeleteClassSessionsAsync(List<int> classSessionIds)
        {
            var placed = await _context.ClassSchedules
                .Where(sched => sched.ClassSessionId.HasValue && classSessionIds.Contains(sched.ClassSessionId.Value))
                .Select(sched => sched.ClassSession!.Course + " " + sched.ClassSession.SessionNumber)
                .ToListAsync();

            if (placed.Any())
                throw new BusinessRuleException($"These sessions are on the timetable: {string.Join(", ", placed)}. Remove each from its schedule first.");

            var studyMaterials = await _context.ClassSessions
                .Where(cs => classSessionIds.Contains(cs.Id))
                .SelectMany(cs => cs.StudyMaterials)
                .ToListAsync();

            foreach (var material in studyMaterials)
            {
                await DeleteFileAsync(material);
            }

            var sessions = await _context.ClassSessions
                .Where(cs => classSessionIds.Contains(cs.Id))
                .ToListAsync();

            _context.ClassSessions.RemoveRange(sessions);
            await _context.SaveChangesAsync();

            var affectedCourses = sessions
                .Select(cs => (cs.ClassLevel, cs.Semester, cs.Course))
                .Distinct()
                .ToList();

            foreach (var (classLevel, semester, course) in affectedCourses)
            {
                var remainingSessions = LoadCourseSessions(classLevel, semester, course);
                ApplySessionNumbers(remainingSessions, remainingSessions.Select(cs => cs.Id).ToList());
            }

            await _context.SaveChangesAsync();
        }

        private List<ClassSession> LoadCourseSessions(ClassLevel classLevel, Semester semester, string course)
        {
            return _context.ClassSessions
                .Where(cs => cs.ClassLevel == classLevel && cs.Semester == semester && cs.Course == course)
                .OrderBy(cs => cs.SessionNumber)
                .ThenBy(cs => cs.Id)
                .ToList();
        }

        private static void ApplySessionNumbers(List<ClassSession> sessions, List<int> orderedIds)
        {
            var sessionNumbers = LogicMethods.AssignSequenceNumbers(orderedIds);

            foreach (var session in sessions)
            {
                if (sessionNumbers.TryGetValue(session.Id, out var sessionNumber))
                    session.SessionNumber = sessionNumber;
            }
        }

        public List<ClassSessionReport> GetClassSessionReportsForDate(DateTime sessionDate)
        {
            var targetDate = sessionDate.Date;

            return _context.ClassSchedules
                .AsNoTracking()
                .Include(sched => sched.ClassSession)
                    .ThenInclude(cs => cs!.Teacher)
                        .ThenInclude(t => t!.Person)
                .Where(sched => sched.DateTime.Date == targetDate &&
                                sched.ClassSession != null &&
                                sched.ClassSession.TeacherRemarks != "")
                .OrderByDescending(sched => sched.ClassSession!.RemarksSubmittedAt)
                .ToList()
                .Select(sched => new ClassSessionReport
                {
                    ClassSessionId = sched.ClassSession!.Id,
                    LearningPathName = sched.ClassLevel.ToDisplayName(),
                    Course = sched.ClassSession.Course,
                    Topic = sched.ClassSession.Topic,
                    SubmittedBy = !string.IsNullOrEmpty(sched.ClassSession.RemarksSubmittedByName)
                        ? sched.ClassSession.RemarksSubmittedByName
                        : sched.ClassSession.Teacher?.Person?.LastName ?? "Unknown",
                    TimeSubmitted = sched.ClassSession.RemarksSubmittedAt ?? sched.DateTime
                })
                .ToList();
        }
        #endregion

        #region Homework
        public Homework? GetHomeworkById(int id)
        {
            return _context.Homework
                .Include(h => h.Submissions)
                    .ThenInclude(sub => sub.Student)
                        .ThenInclude(st => st.Person)
                .FirstOrDefault(h => h.Id == id);
        }

        public HomeworkSubmission? SubmitHomework(int homeworkId, Student student, string answer)
        {
            var homework = GetHomeworkById(homeworkId);
            if (homework == null)
                throw new ArgumentException($"Homework with ID {homeworkId} not found.");

            if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");

            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("Answer cannot be null or empty.", nameof(answer));

            var submission = new HomeworkSubmission
            {
                Student = student,
                Answer = answer,
                IsGraded = false,
                Homework = homework
            };

            return AddHomeworkSubmission(submission);
        }

        public bool DeleteHomework(int id)
        {
            var homework = _context.Set<Homework>()
                .Include(h => h.ClassSession)
                .FirstOrDefault(h => h.Id == id);

            if (homework == null)
                return false;

            if (homework.ClassSession != null)
            {
                homework.ClassSession.HomeworkDetails = null;
            }

            _context.Set<Homework>().Remove(homework);
            _context.SaveChanges();
            return true;
        }

        public HomeworkSubmission? GetHomeworkSubmissionById(int id)
        {
            return _context.HomeworkSubmissions
                .Include(sub => sub.Student)
                    .ThenInclude(st => st.Person)
                .FirstOrDefault(sub => sub.Id == id);
        }

        public HomeworkSubmission? AddHomeworkSubmission(HomeworkSubmission submission)
        {
            if (submission == null)
                return null;

            var homework = GetHomeworkById(submission.Homework?.Id ?? 0);
            if (homework == null)
                return null;

            if (homework.Submissions == null)
                homework.Submissions = new List<HomeworkSubmission>();

            submission.Homework = homework;
            submission.SubmissionDate = DateTime.Now;
            homework.Submissions.Add(submission);

            _context.SaveChanges();
            return submission;
        }

        public void UpdateHomeworkSubmission(HomeworkSubmission submission)
        {
            if (submission == null)
                return;

            var existingSubmission = GetHomeworkSubmissionById(submission.Id);
            if (existingSubmission != null)
            {
                existingSubmission.Answer = submission.Answer;
                existingSubmission.IsGraded = submission.IsGraded;
                existingSubmission.FeedbackComment = submission.FeedbackComment;
                existingSubmission.HomeworkGrade = submission.HomeworkGrade;

                _context.SaveChanges();
            }
        }
        #endregion

        #region Discussions
        public async Task<DiscussionThread> AddDiscussionThreadAsync(int classSessionId, FirstPost firstPost)
        {
            if (firstPost == null)
                throw new ArgumentNullException(nameof(firstPost));

            var thread = new DiscussionThread
            {
                ClassSessionId = classSessionId,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            firstPost.DiscussionThread = thread;
            firstPost.CreatedAt = DateTime.Now;

            thread.FirstPost = firstPost;

            _context.DiscussionThreads.Add(thread);
            await _context.SaveChangesAsync();

            return thread;
        }

        public async Task<Reply> AddReplyAsync(int threadId, int authorId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty.", nameof(comment));

            var thread = await _context.DiscussionThreads
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == threadId)
                ?? throw new InvalidOperationException("Thread not found.");

            var author = await _context.Persons.FindAsync(authorId)
                ?? throw new InvalidOperationException("Author not found.");

            var reply = new Reply
            {
                DiscussionThreadId = threadId,
                DiscussionThread = thread,
                PersonId = authorId,
                Author = author,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            thread.Replies.Add(reply);
            thread.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return reply;
        }

        public async Task<List<DiscussionThread>> GetThreadsForClassSessionAsync(int classSessionId)
        {
            return await _context.DiscussionThreads
                .Where(t => t.ClassSessionId == classSessionId)
                .Include(t => t.FirstPost)
                    .ThenInclude(fp => fp.Author)
                .Include(t => t.Replies)
                    .ThenInclude(r => r.Author)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        #endregion

        #region File Attachments
        public async Task<FileAttachment> UploadFileAsync(IBrowserFile file, string category)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be null or empty.", nameof(category));
            if (file.Size > FcmsConstants.MAX_FILE_SIZE)
                throw new InvalidOperationException($"File size exceeds the {FcmsConstants.MAX_FILE_SIZE_MB}MB limit. File size: {file.Size / FcmsConstants.BYTES_IN_MEGABYTE:F2}MB");

            var folderName = Path.GetInvalidFileNameChars()
                .Aggregate(category, (current, c) => current.Replace(c, '_'));

            var targetFolder = UploadPathHelper.GetCategoryPath(_environment, folderName);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var extension = Path.GetExtension(file.Name);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.OpenReadStream(FcmsConstants.MAX_FILE_SIZE).CopyToAsync(stream);
            }

            var publicUrl = UploadPathHelper.GetPublicUrl(folderName, uniqueFileName);

            var attachment = new FileAttachment
            {
                FileName = file.Name,
                FilePath = publicUrl,
                FileSize = file.Size,
                UploadDate = DateTime.Now
            };

            using var context = _contextFactory.CreateDbContext();
            context.FileAttachments.Add(attachment);
            await context.SaveChangesAsync();

            return attachment;
        }

        public async Task DeleteFileAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            var filePath = UploadPathHelper.GetPhysicalPath(_environment, attachment.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var existingAttachment = await _context.FileAttachments.FindAsync(attachment.Id);
            if (existingAttachment != null)
            {
                _context.FileAttachments.Remove(existingAttachment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FileAttachment>> GetAttachmentsAsync(string category, int referenceId)
        {
            if (category == "StudyMaterials")
            {
                var classSession = await _context.ClassSessions
                    .Include(cs => cs.StudyMaterials)
                    .FirstOrDefaultAsync(cs => cs.Id == referenceId);

                return classSession?.StudyMaterials ?? new List<FileAttachment>();
            }
            return new List<FileAttachment>();
        }

        public async Task SaveAttachmentReferenceAsync(FileAttachment attachment, string category, int referenceId)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));


            if (category == "StudyMaterials")
            {
                var classSession = await _context.ClassSessions
                    .Include(cs => cs.StudyMaterials)
                    .FirstOrDefaultAsync(cs => cs.Id == referenceId);

                if (classSession != null)
                {
                    if (classSession.StudyMaterials == null)
                        classSession.StudyMaterials = new List<FileAttachment>();

                    if (!classSession.StudyMaterials.Any(sm => sm.Id == attachment.Id))
                    {
                        classSession.StudyMaterials.Add(attachment);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        #endregion

        #region Payments
        public Payment AddPayment(Payment payment)
        {
            var schoolFees = _context.SchoolFees
                .Include(fees => fees.Payments)
                .Include(fees => fees.Adjustments)
                .AsSplitQuery()
                .FirstOrDefault(fees => fees.Id == payment.SchoolFeesId);

            if (schoolFees == null)
                throw new ArgumentException("School fees record not found.");

            if (!LogicMethods.IsPaymentPeriodEditable(schoolFees.LearningPath, GetCurrentAcademicPeriod()))
                throw new BusinessRuleException(
                    "Payments cannot be recorded against a closed academic period.");

            var studentFees = _context.SchoolFees
                .Include(fees => fees.Payments)
                .Include(fees => fees.Adjustments)
                .AsSplitQuery()
                .Where(fees => fees.StudentId == schoolFees.StudentId)
                .ToList();

            payment.Reference = string.IsNullOrWhiteSpace(payment.Reference)
                ? null
                : payment.Reference.Trim();

            if (!LogicMethods.IsPaymentReferenceRequired(payment.PaymentMethod))
                payment.Reference = null;

            if (LogicMethods.IsPaymentReferenceRequired(payment.PaymentMethod) && payment.Reference == null)
                throw new BusinessRuleException(
                    "A bank transfer requires the teller or transfer reference.");

            if (payment.Reference != null &&
                _context.Payments.Any(existing => existing.Reference == payment.Reference))
            {
                throw new BusinessRuleException(
                    $"Payment reference '{payment.Reference}' has already been recorded.");
            }

            if (!LogicMethods.IsPaymentWithinBalance(studentFees, schoolFees, payment.Amount))
                throw new BusinessRuleException(
                    $"Payment of {payment.Amount:N2} exceeds the total outstanding of {LogicMethods.GetCarriedForward(studentFees, schoolFees):N2}.");

            payment.LearningPathId = schoolFees.LearningPathId;
            schoolFees.Payments.Add(payment);
            _context.SaveChanges();

            return payment;
        }


        public void UpdatePayment(Payment payment)
        {
            var existingPayment = _context.Payments
                .FirstOrDefault(candidate => candidate.Id == payment.Id);

            if (existingPayment == null)
                throw new ArgumentException("Payment not found.");

            var schoolFees = _context.SchoolFees
                .Include(fees => fees.Payments)
                .Include(fees => fees.Adjustments)
                .AsSplitQuery()
                .FirstOrDefault(fees => fees.Id == existingPayment.SchoolFeesId);

            if (schoolFees == null)
                throw new ArgumentException("School fees record not found.");

            if (!LogicMethods.IsPaymentPeriodEditable(schoolFees.LearningPath, GetCurrentAcademicPeriod()))
                throw new BusinessRuleException(
                    "Payments from a closed academic period cannot be modified.");

            var studentFees = _context.SchoolFees
                .Include(fees => fees.Payments)
                .Include(fees => fees.Adjustments)
                .AsSplitQuery()
                .Where(fees => fees.StudentId == schoolFees.StudentId)
                .ToList();

            payment.Reference = string.IsNullOrWhiteSpace(payment.Reference)
                ? null
                : payment.Reference.Trim();

            if (!LogicMethods.IsPaymentReferenceRequired(payment.PaymentMethod))
                payment.Reference = null;

            if (LogicMethods.IsPaymentReferenceRequired(payment.PaymentMethod) && payment.Reference == null)
                throw new BusinessRuleException(
                    "A bank transfer requires the teller or transfer reference.");

            if (payment.Reference != null &&
                _context.Payments.Any(candidate => candidate.Reference == payment.Reference
                                                && candidate.Id != payment.Id))
            {
                throw new BusinessRuleException(
                    $"Payment reference '{payment.Reference}' has already been recorded.");
            }

            if (!LogicMethods.IsPaymentWithinBalance(studentFees, schoolFees, payment.Amount, payment.Id))
                throw new BusinessRuleException(
                    $"Payment of {payment.Amount:N2} exceeds the total outstanding for this student.");

            existingPayment.Amount = payment.Amount;
            existingPayment.Date = payment.Date;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.Reference = payment.Reference;

            _context.SaveChanges();
        }

        public void DeletePayment(int paymentId)
        {
            var payment = _context.Payments
                .Include(candidate => candidate.SchoolFees)
                .FirstOrDefault(candidate => candidate.Id == paymentId);

            if (payment == null)
                return;

            if (!LogicMethods.IsPaymentPeriodEditable(payment.SchoolFees?.LearningPath, GetCurrentAcademicPeriod()))
                throw new BusinessRuleException(
                    "Payments from a closed academic period cannot be deleted.");

            _context.Payments.Remove(payment);
            _context.SaveChanges();
        }

        public Payment PrepareNewPayment(SchoolFees schoolFees)
        {
            return new Payment
            {
                Date = DateTime.Today,
                SchoolFeesId = schoolFees.Id,
                LearningPathId = schoolFees.LearningPathId
            };
        }

        public SchoolFees? GetSchoolFees(int id)
        {
            return _context.SchoolFees
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .FirstOrDefault(fees => fees.Id == id);
        }

        public Student? GetStudentBySchoolFeesId(int schoolFeesId)
        {
            var studentId = _context.SchoolFees
                .AsNoTracking()
                .Where(fees => fees.Id == schoolFeesId)
                .Select(fees => fees.StudentId)
                .FirstOrDefault();

            if (studentId == 0)
                return null;

            return _context.Students
                .Include(student => student.Person)
                .Include(student => student.LearningPath)
                .Include(student => student.SchoolFees)
                .ThenInclude(fees => fees.Payments)
                .AsSplitQuery()
                .FirstOrDefault(student => student.Id == studentId);
        }

        private double EnsureSchoolFeesForEnrolment(int studentId, int learningPathId)
        {
            bool alreadyExists = _context.SchoolFees
                .Any(fees => fees.StudentId == studentId && fees.LearningPathId == learningPathId);

            if (alreadyExists)
                return 0;

            int academicPeriodId = _context.LearningPaths
                .Where(learningPath => learningPath.Id == learningPathId)
                .Select(learningPath => learningPath.AcademicPeriodId)
                .FirstOrDefault();

            var feesInSamePeriod = _context.SchoolFees
                .Include(fees => fees.Payments)
                .FirstOrDefault(fees => fees.StudentId == studentId
                                     && fees.LearningPath!.AcademicPeriodId == academicPeriodId);

            if (feesInSamePeriod == null)
            {
                _context.SchoolFees.Add(new SchoolFees
                {
                    StudentId = studentId,
                    LearningPathId = learningPathId
                });

                return 0;
            }

            double amountMoved = feesInSamePeriod.Payments.Sum(payment => payment.Amount);

            feesInSamePeriod.LearningPathId = learningPathId;

            foreach (var payment in feesInSamePeriod.Payments)
            {
                payment.LearningPathId = learningPathId;
            }

            return amountMoved;
        }

        public Dictionary<int, double> GetTotalPaidByStudentForLearningPath(int learningPathId)
        {
            return _context.SchoolFees
                .AsNoTracking()
                .Where(fees => fees.LearningPathId == learningPathId)
                .Select(fees => new
                {
                    fees.StudentId,
                    TotalPaid = fees.Payments.Sum(payment => payment.Amount)
                })
                .ToDictionary(row => row.StudentId, row => row.TotalPaid);
        }

        public FeeAdjustment AddFeeAdjustment(FeeAdjustment adjustment)
        {
            var schoolFees = _context.SchoolFees
                .Include(fees => fees.Adjustments)
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .FirstOrDefault(fees => fees.Id == adjustment.SchoolFeesId);

            if (schoolFees == null)
                throw new ArgumentException("School fees record not found.");

            if (!LogicMethods.IsPaymentPeriodEditable(schoolFees.LearningPath, GetCurrentAcademicPeriod()))
                throw new BusinessRuleException(
                    "Discounts cannot be applied to a closed academic period.");

            double termFee = schoolFees.LearningPath?.FeePerSemester ?? 0;

            double value = adjustment.Mode == FeeAdjustmentMode.Percentage
                ? termFee * adjustment.Value / 100
                : adjustment.Value;

            if (value <= 0)
                throw new BusinessRuleException("An adjustment must be greater than zero.");

            if (schoolFees.TotalAdjustments + value > termFee)
                throw new BusinessRuleException("Adjustments cannot exceed the term fee.");

            if (termFee - (schoolFees.TotalAdjustments + value) < schoolFees.TotalPaid)
                throw new BusinessRuleException(
                    $"This adjustment would put the fee below the {schoolFees.TotalPaid:N2} already paid.");

            adjustment.Date = DateTime.Now;
            schoolFees.Adjustments.Add(adjustment);
            _context.SaveChanges();

            return adjustment;
        }

        public void DeleteFeeAdjustment(int adjustmentId)
        {
            var adjustment = _context.FeeAdjustments
                .Include(candidate => candidate.SchoolFees)
                .FirstOrDefault(candidate => candidate.Id == adjustmentId);

            if (adjustment == null)
                return;

            if (!LogicMethods.IsPaymentPeriodEditable(adjustment.SchoolFees?.LearningPath, GetCurrentAcademicPeriod()))
                throw new BusinessRuleException(
                    "Discounts cannot be removed from a closed academic period.");

            _context.FeeAdjustments.Remove(adjustment);
            _context.SaveChanges();
        }

        public Dictionary<int, int> GetEnrolledStudentCountsForPeriod(int academicPeriodId)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(learningPath => learningPath.AcademicPeriodId == academicPeriodId)
                .Select(learningPath => new { learningPath.Id, Count = learningPath.Students.Count })
                .ToDictionary(row => row.Id, row => row.Count);
        }

        public int GetEnrolledStudentCount(int learningPathId)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(learningPath => learningPath.Id == learningPathId)
                .Select(learningPath => learningPath.Students.Count)
                .FirstOrDefault();
        }
        #endregion

        #region Grading
        public void SaveCourseGradingConfiguration(CourseGradingConfiguration configuration)
        {
            var learningPath = _context.LearningPaths
                .Include(lp => lp.CourseGradingConfigurations)
                .FirstOrDefault(lp => lp.Id == configuration.LearningPathId);

            if (learningPath == null) return;

            var existingConfig = learningPath.CourseGradingConfigurations
                .FirstOrDefault(c => c.Course == configuration.Course);

            if (existingConfig != null)
            {
                existingConfig.HomeworkWeightPercentage = configuration.HomeworkWeightPercentage;
                existingConfig.QuizWeightPercentage = configuration.QuizWeightPercentage;
                existingConfig.FinalExamWeightPercentage = configuration.FinalExamWeightPercentage;
            }
            else
            {
                learningPath.CourseGradingConfigurations.Add(configuration);
            }

            _context.SaveChanges();

            var savedConfig = existingConfig ?? configuration;
            UpdateCourseGradeConfigurations(learningPath.Id, configuration.Course, savedConfig);
        }

        private void UpdateCourseGradeConfigurations(int learningPathId, string course, CourseGradingConfiguration config)
        {
            var affectedCourseGrades = _context.Students
                .Include(s => s.CourseGrades)
                    .ThenInclude(cg => cg.TestGrades)
                .SelectMany(s => s.CourseGrades)
                .Where(cg => cg.LearningPathId == learningPathId && cg.Course == course)
                .ToList();

            foreach (var courseGrade in affectedCourseGrades)
            {
                courseGrade.GradingConfiguration = config;

                if (!courseGrade.IsFinalized)
                {
                    LogicMethods.RecalculateCourseGrade(courseGrade);
                }
            }

            _context.SaveChanges();
        }

        public CourseGradingConfiguration? GetCourseGradingConfiguration(int learningPathId, string courseName)
        {
            var learningPath = _context.LearningPaths
                .Include(lp => lp.CourseGradingConfigurations)
                .FirstOrDefault(lp => lp.Id == learningPathId);

            return learningPath?.CourseGradingConfigurations
                .FirstOrDefault(c => c.Course == courseName);
        }

        public List<CourseGradingConfiguration> GetAllCourseGradingConfigurations(int learningPathId)
        {
            var learningPath = _context.LearningPaths
                .Include(lp => lp.CourseGradingConfigurations)
                .FirstOrDefault(lp => lp.Id == learningPathId);

            return learningPath?.CourseGradingConfigurations ?? new List<CourseGradingConfiguration>();
        }

        public List<string> GetCoursesWithoutGradingConfiguration(int learningPathId)
        {
            var learningPath = _context.LearningPaths
                .Include(lp => lp.CourseGradingConfigurations)
                .FirstOrDefault(lp => lp.Id == learningPathId);

            if (learningPath == null) return new List<string>();

            var allCourses = CourseDefaults.GetCourseNames(learningPath.EducationLevel);
            var configuredCourses = learningPath.CourseGradingConfigurations.Select(c => c.Course).ToList();

            return allCourses.Except(configuredCourses).ToList();
        }

        public List<LearningPath> GetSubmittedLearningPaths(string academicYear, string semester)
        {
            var academicPeriod = GetAcademicPeriodByYearAndSemester(academicYear, semester);

            if (academicPeriod == null)
                return new List<LearningPath>();

            return _context.LearningPaths
                .AsNoTracking()
                .Include(lp => lp.Students)
                .Where(lp => lp.AcademicPeriodId == academicPeriod.Id &&
                             (lp.ApprovalStatus == PrincipalApprovalStatus.Review ||
                              lp.ApprovalStatus == PrincipalApprovalStatus.Approved))
                .ToList();
        }

        public AcademicPeriod? GetAcademicPeriodByYearAndSemester(string academicYear, string semester)
        {
            var yearParts = academicYear.Split('-');

            if (yearParts.Length != 2 || !int.TryParse(yearParts[0], out int startYear))
                return null;

            if (!Enum.TryParse<Semester>(semester, out var semesterEnum))
                return null;

            return _context.AcademicPeriods
                .AsNoTracking()
                .FirstOrDefault(ap => ap.AcademicYearStart.Year == startYear && ap.Semester == semesterEnum);
        }

        public void UpdateTestGradeScore(int testGradeId, double score)
        {
            var testGrade = _context.TestGrades
                .Include(tg => tg.CourseGrade)
                    .ThenInclude(cg => cg.GradingConfiguration)
                .Include(tg => tg.CourseGrade)
                    .ThenInclude(cg => cg.TestGrades)
                .FirstOrDefault(tg => tg.Id == testGradeId);

            if (testGrade == null || testGrade.CourseGrade == null) return;
            if (testGrade.CourseGrade.IsFinalized) return;

            testGrade.Score = score;

            if (testGrade.CourseGrade.GradingConfiguration != null)
            {
                LogicMethods.RecalculateCourseGrade(testGrade.CourseGrade);
            }

            _context.SaveChanges();
        }

        public void AddTestGrade(int studentId, string course, double score, GradeType gradeType, int teacherId, string teacherRemark, int learningPathId, DateTime? date = null)
        {
            var courseGrade = _context.CourseGrades
                .Include(cg => cg.TestGrades)
                .Include(cg => cg.GradingConfiguration)
                .FirstOrDefault(cg => cg.StudentId == studentId &&
                                     cg.Course == course &&
                                     cg.LearningPathId == learningPathId);

            if (courseGrade == null)
            {
                var gradingConfig = _context.LearningPaths
                    .Include(lp => lp.CourseGradingConfigurations)
                    .FirstOrDefault(lp => lp.Id == learningPathId)?
                    .CourseGradingConfigurations
                    .FirstOrDefault(c => c.Course == course);

                courseGrade = new CourseGrade
                {
                    Course = course,
                    StudentId = studentId,
                    LearningPathId = learningPathId,
                    GradingConfiguration = gradingConfig,
                    TestGrades = new List<TestGrade>()
                };
                _context.CourseGrades.Add(courseGrade);
                _context.SaveChanges();
            }

            var testGrade = new TestGrade
            {
                Score = score,
                GradeType = gradeType,
                TeacherId = teacherId,
                Date = date ?? DateTime.Now,
                TeacherRemark = teacherRemark,
                CourseGradeId = courseGrade.Id
            };

            _context.TestGrades.Add(testGrade);

            if (courseGrade.GradingConfiguration != null)
            {
                LogicMethods.RecalculateCourseGrade(courseGrade);
                _context.CourseGrades.Update(courseGrade);
            }

            _context.SaveChanges();
        }

        public async Task<TestGrade> AddHomeworkSubmissionGradeAsync( int studentId, string course, double score, int teacherId, string teacherRemark, int learningPathId, DateTime? date = null, int? submissionId = null)
        {
            var courseGrade = await _context.CourseGrades
                .Include(cg => cg.TestGrades)
                .Include(cg => cg.GradingConfiguration)
                .FirstOrDefaultAsync(cg => cg.StudentId == studentId &&
                                           cg.Course == course &&
                                           cg.LearningPathId == learningPathId);

            if (courseGrade == null)
            {
                var gradingConfig = await _context.LearningPaths
                    .Include(lp => lp.CourseGradingConfigurations)
                    .Where(lp => lp.Id == learningPathId)
                    .SelectMany(lp => lp.CourseGradingConfigurations)
                    .FirstOrDefaultAsync(c => c.Course == course);

                courseGrade = new CourseGrade
                {
                    Course = course,
                    StudentId = studentId,
                    LearningPathId = learningPathId,
                    GradingConfiguration = gradingConfig,
                    TestGrades = new List<TestGrade>()
                };

                _context.CourseGrades.Add(courseGrade);
                await _context.SaveChangesAsync();
            }

            var testGrade = new TestGrade
            {
                Score = score,
                GradeType = GradeType.Homework,
                TeacherId = teacherId,
                Date = date ?? DateTime.Now,
                TeacherRemark = teacherRemark,
                CourseGradeId = courseGrade.Id
            };

            _context.TestGrades.Add(testGrade);

            if (courseGrade.GradingConfiguration != null)
            {
                LogicMethods.RecalculateCourseGrade(courseGrade);
                _context.CourseGrades.Update(courseGrade);
            }

            if (submissionId.HasValue)
            {
                var submission = await _context.HomeworkSubmissions
                    .FirstOrDefaultAsync(hs => hs.Id == submissionId.Value);

                if (submission != null)
                {
                    submission.HomeworkGrade = testGrade;
                    submission.IsGraded = true;
                    submission.FeedbackComment = teacherRemark;
                }
            }

            await _context.SaveChangesAsync();

            return testGrade;
        }

        public Dictionary<(string Course, GradeType GradeType), int> GetGradeCountsByLearningPath(int learningPathId)
        {
            var batches = _context.TestGrades
                .Where(tg => tg.CourseGrade.LearningPathId == learningPathId)
                .Select(tg => new
                {
                    tg.CourseGrade.Course,
                    tg.GradeType,
                    tg.Date
                })
                .Distinct()
                .ToList();

            return batches
                .GroupBy(b => (b.Course, b.GradeType))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public void SaveFinalizedGrades(LearningPath learningPath)
        {
            LogicMethods.FinalizeSemesterGrades(learningPath);
            foreach (var student in learningPath.Students)
            {
                foreach (var courseGrade in student.CourseGrades
                    .Where(cg => cg.LearningPathId == learningPath.Id))
                {
                    _context.CourseGrades.Update(courseGrade);
                }
            }
            _context.SaveChanges();
        }

        public Dictionary<Semester, double> GetStudentAllSemesterGrades(int studentId, EducationLevel educationLevel, ClassLevel classLevel)
        {
            var student = _context.Students
                .AsNoTracking()
                .Include(s => s.CourseGrades)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
                return new Dictionary<Semester, double>();

            var learningPathIds = student.CourseGrades
                .Where(cg => cg.LearningPathId > 0)
                .Select(cg => cg.LearningPathId)
                .Distinct()
                .ToList();

            var learningPaths = _context.LearningPaths
                .AsNoTracking()
                .Where(lp => learningPathIds.Contains(lp.Id) &&
                             lp.EducationLevel == educationLevel &&
                             lp.ClassLevel == classLevel)
                .OrderBy(lp => lp.Semester)
                .ToList();

            var semesterGrades = new Dictionary<Semester, double>();

            foreach (var learningPath in learningPaths)
            {
                var grade = LogicMethods.CalculateSemesterOverallGrade(student, learningPath);
                semesterGrades[learningPath.Semester] = grade;
            }

            return semesterGrades;
        }

        public bool AreLearningPathGradesFinalized(int learningPathId)
        {
            var finalizedFlags = _context.StudentReportCards
                .AsNoTracking()
                .Where(rc => rc.LearningPathId == learningPathId)
                .Select(rc => rc.IsFinalized)
                .ToList();

            return finalizedFlags.Any() && finalizedFlags.All(isFinalized => isFinalized);
        }
        #endregion

        #region Curriculum
        public List<ClassSession> GetCurriculumSessions(ClassLevel classLevel, Semester semester)
        {
            return _context.ClassSessions
                .AsNoTracking()
                .Where(cs => cs.ClassLevel == classLevel && cs.Semester == semester)
                .OrderBy(cs => cs.Course)
                .ThenBy(cs => cs.SessionNumber)
                .ToList();
        }
        #endregion

        #region Attendance
        public DailyAttendanceLogEntry SaveAttendance(int learningPathId, List<int> presentStudentIds, int teacherId, DateTime? attendanceDate = null)
        {
            var learningPath = GetLearningPathForAttendance(learningPathId);
            if (learningPath == null)
                throw new ArgumentException($"Learning path with ID {learningPathId} not found.");

            var presentStudents = learningPath.Students
                .Where(s => presentStudentIds.Contains(s.Id))
                .ToList();

            var allStudents = learningPath.Students ?? new List<Student>();
            var absentStudents = allStudents.Where(s => !presentStudentIds.Contains(s.Id)).ToList();

            var targetDate = attendanceDate ?? DateTime.Now;

            var existingAttendance = learningPath.AttendanceLog?
                .FirstOrDefault(log => log.TimeStamp.Date == targetDate.Date);

            if (existingAttendance != null)
            {
                existingAttendance.PresentStudents = presentStudents;
                existingAttendance.AbsentStudents = absentStudents;
                existingAttendance.TimeStamp = targetDate;
                _context.SaveChanges();
                return existingAttendance;
            }

            var attendanceEntry = new DailyAttendanceLogEntry
            {
                LearningPathId = learningPath.Id,
                LearningPath = learningPath,
                TeacherId = teacherId,
                PresentStudents = presentStudents,
                AbsentStudents = absentStudents,
                TimeStamp = targetDate
            };

            _context.DailyAttendanceLogEntries.Add(attendanceEntry);
            _context.SaveChanges();

            return attendanceEntry;
        }

        public LearningPath? GetLearningPathForAttendance(int id)
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                .Include(lp => lp.AttendanceLog)
                    .ThenInclude(al => al.PresentStudents)
                .Include(lp => lp.AttendanceLog)
                    .ThenInclude(al => al.AbsentStudents)
                .AsSplitQuery()
                .FirstOrDefault(lp => lp.Id == id);
        }

        public bool HasAttendanceBeenTaken(int learningPathId, DateTime date)
        {
            return _context.DailyAttendanceLogEntries
                .Any(log => log.LearningPathId == learningPathId && log.TimeStamp.Date == date.Date);
        }

        public DailyAttendanceLogEntry? GetAttendanceForDate(int learningPathId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return _context.DailyAttendanceLogEntries
                .Include(al => al.Teacher)
                    .ThenInclude(t => t.Person)
                .Include(al => al.PresentStudents)
                    .ThenInclude(s => s.Person)
                .Include(al => al.AbsentStudents)
                    .ThenInclude(s => s.Person)
                .AsSplitQuery()
                .FirstOrDefault(log =>
                    log.LearningPathId == learningPathId &&
                    log.TimeStamp >= start &&
                    log.TimeStamp < end);
        }

        public int GetNumberOfAttendanceDaysRecorded(int learningPathId)
        {
            return _context.DailyAttendanceLogEntries
                .Count(log => log.LearningPathId == learningPathId);
        }

        public double GetDailyAttendanceAverage(int learningPathId, DateTime date)
        {
            var log = _context.DailyAttendanceLogEntries
                .Include(l => l.PresentStudents)
                .Include(l => l.AbsentStudents)
                .FirstOrDefault(l => l.LearningPathId == learningPathId
                                  && l.TimeStamp.Date == date.Date);

            if (log == null)
                return 0;

            var totalStudents = (log.PresentStudents?.Count ?? 0) + (log.AbsentStudents?.Count ?? 0);

            if (totalStudents == 0)
                return 0;

            return (double)(log.PresentStudents?.Count ?? 0) / totalStudents * FcmsConstants.PERCENTAGE_MULTIPLIER;
        }
        #endregion

        #region Student Report Cards
        public StudentReportCard? GetStudentReportCard(int studentId, int learningPathId)
        {
            return _context.StudentReportCards
                .Include(rc => rc.Student)
                .Include(rc => rc.LearningPath)
                .Include(rc => rc.GeneratedByTeacher)
                .Include(rc => rc.FinalizedByPrincipal)
                .FirstOrDefault(rc => rc.StudentId == studentId && rc.LearningPathId == learningPathId);
        }

        public List<StudentReportCard> GetStudentReportCardsForLearningPath(int learningPathId)
        {
            return _context.StudentReportCards
                .Include(rc => rc.Student)
                .Include(rc => rc.LearningPath)
                .Where(rc => rc.LearningPathId == learningPathId)
                .ToList();
        }

        public StudentReportCard SaveStudentReportCard(StudentReportCard reportCard)
        {
            var existingReportCard = GetStudentReportCard(reportCard.StudentId, reportCard.LearningPathId);

            if (existingReportCard != null)
            {
                existingReportCard.TeacherRemarks = reportCard.TeacherRemarks;
                existingReportCard.PrincipalRemarks = reportCard.PrincipalRemarks;
                existingReportCard.SemesterOverallGrade = reportCard.SemesterOverallGrade;
                existingReportCard.PromotionGrade = reportCard.PromotionGrade;
                existingReportCard.StudentRank = reportCard.StudentRank;
                existingReportCard.PresentDays = reportCard.PresentDays;
                existingReportCard.TotalDays = reportCard.TotalDays;
                existingReportCard.AttendanceRate = reportCard.AttendanceRate;
                existingReportCard.IsPromoted = reportCard.IsPromoted;
                existingReportCard.PromotionStatus = reportCard.PromotionStatus;
                existingReportCard.DateFinalized = reportCard.DateFinalized;
                existingReportCard.IsFinalized = reportCard.IsFinalized;
                existingReportCard.GeneratedByTeacherId = reportCard.GeneratedByTeacherId;
                existingReportCard.FinalizedByPrincipalId = reportCard.FinalizedByPrincipalId;

                _context.SaveChanges();
                return existingReportCard;
            }

            _context.StudentReportCards.Add(reportCard);
            _context.SaveChanges();
            return reportCard;
        }

        public void UpdateStudentReportCardRemarks(int studentId, int learningPathId, string? teacherRemarks = null, string? principalRemarks = null)
        {
            var reportCard = GetStudentReportCard(studentId, learningPathId);

            if (reportCard == null)
            {
                reportCard = new StudentReportCard
                {
                    StudentId = studentId,
                    LearningPathId = learningPathId,
                    TeacherRemarks = teacherRemarks ?? "",
                    PrincipalRemarks = principalRemarks ?? ""
                };
                _context.StudentReportCards.Add(reportCard);
            }
            else
            {
                if (teacherRemarks != null)
                    reportCard.TeacherRemarks = teacherRemarks;
                if (principalRemarks != null)
                    reportCard.PrincipalRemarks = principalRemarks;
                _context.StudentReportCards.Update(reportCard);
            }

            _context.SaveChanges();
        }
        #endregion

        #region Archives
        public void ArchiveStudent(Student student)
        {
            if (student == null) return;

            var school = _context.School
                .Include(s => s.Students)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.Students)
                .Include(s => s.LearningPaths)
                .FirstOrDefault();

            if (school == null) return;

            foreach (var learningPath in school.LearningPaths)
            {
                var studentInLearningPath = learningPath.Students.FirstOrDefault(s => s.Id == student.Id);
                if (studentInLearningPath != null)
                {
                    learningPath.Students.Remove(studentInLearningPath);
                }
            }

            var schoolStudent = school.Students.FirstOrDefault(s => s.Id == student.Id);
            if (schoolStudent != null)
            {
                school.Students.Remove(schoolStudent);
            }

            student.Person.IsArchived = true;
            student.Person.IsActive = false;
            student.ArchivedDate = DateTime.Now;
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public List<Student> GetArchivedStudents()
        {
            return _context.Students
                .AsNoTracking()
                .Include(s => s.Person)
                .Include(s => s.CourseGrades)
                .Include(s => s.ReportCards)
                    .ThenInclude(rc => rc.LearningPath)
                .Where(s => s.Person.IsArchived == true)
                .OrderByDescending(s => s.ArchivedDate)
                .ToList();
        }

        public List<string> GetArchivedPaymentsAcademicYears()
        {
            return _context.ArchivedStudentPayments
                .AsNoTracking()
                .Select(asp => asp.AcademicYear)
                .Distinct()
                .OrderByDescending(year => year)
                .ToList();
        }

        public void ArchiveStudentPayments(LearningPath learningPath)
        {
            bool alreadyArchived = _context.ArchivedStudentPayments
                .AsNoTracking()
                .Any(a => a.LearningPathId == learningPath.Id &&
                          a.AcademicYear == learningPath.AcademicYear &&
                          a.Semester == learningPath.Semester);

            if (alreadyArchived)
                return;

            var studentIds = learningPath.Students.Select(s => s.Id).ToList();
            var studentsWithFees = _context.Students
                .Include(s => s.Person)
                .Include(s => s.SchoolFees)
                    .ThenInclude(sf => sf.Payments)
                .Include(s => s.LearningPath)
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            foreach (var student in studentsWithFees)
            {
                var paymentReport = LogicMethods.GenerateStudentPaymentReportEntry(student, learningPath.Id);

                var archivedPayment = new ArchivedStudentPayment
                {
                    StudentId = student.Id,
                    StudentName = Util.GetFullName(student.Person),
                    StudentAddress = paymentReport.StudentAddress,
                    LearningPathId = learningPath.Id,
                    LearningPathName = Util.GetLearningPathName(learningPath),
                    EducationLevel = learningPath.EducationLevel,
                    ClassLevel = learningPath.ClassLevel,
                    Semester = learningPath.Semester,
                    AcademicYear = learningPath.AcademicYear,
                    TotalFees = paymentReport.TotalFees,
                    TermFee = paymentReport.TermFee,
                    Discount = paymentReport.Discount,
                    BroughtForward = paymentReport.BroughtForwardOutstanding,
                    TotalPayable = paymentReport.TotalPayable,
                    CarriedForward = paymentReport.TotalOutstanding,
                    TotalPaid = paymentReport.TotalPaid,
                    OutstandingBalance = paymentReport.OutstandingBalance,
                    PaymentCompletionRate = paymentReport.StudentPaymentCompletionRate,
                    TimelyCompletionRate = paymentReport.StudentTimelyCompletionRate,
                    ArchivedDate = DateTime.Now
                };

                _context.ArchivedStudentPayments.Add(archivedPayment);

                foreach (var paymentDetail in paymentReport.PaymentDetails)
                {
                    var archivedDetail = new ArchivedPaymentDetail
                    {
                        ArchivedStudentPayment = archivedPayment,
                        Date = paymentDetail.Date,
                        Amount = paymentDetail.Amount,
                        PaymentMethod = Enum.TryParse<PaymentMethod>(paymentDetail.PaymentMethod, out var pm) ? pm : PaymentMethod.Cash,
                        Reference = paymentDetail.Reference
                    };

                    _context.ArchivedPaymentDetails.Add(archivedDetail);
                }
            }

            _context.SaveChanges();
        }

        public List<ArchivedStudentPayment> GetArchivedStudentPayments(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            return _context.ArchivedStudentPayments
                .AsNoTracking()
                .Include(asp => asp.PaymentDetails)
                .Where(asp => asp.AcademicYear == academicYear &&
                              asp.EducationLevel == educationLevel &&
                              asp.ClassLevel == classLevel &&
                              asp.Semester == semester)
                .OrderBy(asp => asp.StudentName)
                .ToList();
        }

        public List<ArchivedPaymentDetail> GetArchivedPaymentDetails(int archivedStudentPaymentId)
        {
            return _context.ArchivedPaymentDetails
                .AsNoTracking()
                .Where(apd => apd.ArchivedStudentPaymentId == archivedStudentPaymentId)
                .OrderByDescending(apd => apd.Date)
                .ToList();
        }

        public void ArchiveStudentAttendance(LearningPath learningPath)
        {
            bool alreadyArchived = _context.AttendanceArchives
                .AsNoTracking()
                .Any(a => a.LearningPathId == learningPath.Id &&
                          a.AcademicYear == learningPath.AcademicYear &&
                          a.Semester == learningPath.Semester);

            if (alreadyArchived)
                return;

            var attendanceLogs = GetDailyAttendanceForLearningPath(learningPath.Id);

            foreach (var student in learningPath.Students)
            {
                foreach (var log in attendanceLogs)
                {
                    var attendanceArchive = new AttendanceArchive
                    {
                        StudentId = student.Id,
                        StudentName = Util.GetFullName(student.Person),
                        LearningPathId = learningPath.Id,
                        LearningPathName = Util.GetLearningPathName(learningPath),
                        Date = log.TimeStamp,
                        IsPresent = log.PresentStudents.Any(s => s.Id == student.Id),
                        AcademicYear = learningPath.AcademicYear,
                        Semester = learningPath.Semester,
                        EducationLevel = learningPath.EducationLevel,
                        ClassLevel = learningPath.ClassLevel,
                        ArchivedDate = DateTime.Now
                    };

                    _context.AttendanceArchives.Add(attendanceArchive);
                }
            }

            _context.SaveChanges();
        }

        public List<AttendanceArchive> GetArchivedStudentAttendance(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            return _context.AttendanceArchives
                .AsNoTracking()
                .Where(aa =>
                    aa.AcademicYear == academicYear &&
                    aa.EducationLevel == educationLevel &&
                    aa.ClassLevel == classLevel &&
                    aa.Semester == semester)
                .OrderBy(aa => aa.StudentName)
                .ToList();
        }

        public List<AttendanceArchive> GetArchivedStudentAttendanceDetails(int studentId, int learningPathId)
        {
            return _context.AttendanceArchives
                .AsNoTracking()
                .Where(aa => aa.StudentId == studentId && aa.LearningPathId == learningPathId)
                .OrderBy(aa => aa.Date)
                .ToList();
        }

        public List<string> GetArchivedAttendanceAcademicYears()
        {
            return _context.AttendanceArchives
                .AsNoTracking()
                .Select(aa => aa.AcademicYear)
                .Distinct()
                .OrderByDescending(year => year)
                .ToList();
        }

        public List<DailyAttendanceLogEntry> GetDailyAttendanceForLearningPath(int learningPathId)
        {
            return _context.DailyAttendanceLogEntries
                .AsNoTracking()
                .Include(log => log.PresentStudents)
                .Include(log => log.AbsentStudents)
                .Where(log => log.LearningPathId == learningPathId)
                .OrderBy(log => log.TimeStamp)
                .ToList();
        }

        public List<string> GetGradeArchiveAcademicYears()
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Include(lp => lp.ReportCards)
                .Where(lp => lp.ReportCards.Any(rc => rc.IsFinalized))
                .Select(lp => lp.AcademicYearStart.Year)
                .Distinct()
                .OrderByDescending(year => year)
                .Select(year => $"{year}-{year + 1}")
                .ToList();
        }

        public List<string> GetArchivedLearningPathAcademicYears()
        {
            return _context.ArchivedLearningPathPayments
                .AsNoTracking()
                .Select(alp => alp.AcademicYear)
                .Distinct()
                .OrderByDescending(year => year)
                .ToList();
        }

        public List<ArchivedLearningPathPayment> GetArchivedLearningPathPayments(string academicYear, Semester semester)
        {
            return _context.ArchivedLearningPathPayments
                .AsNoTracking()
                .Where(alp => alp.AcademicYear == academicYear && alp.Semester == semester)
                .OrderBy(alp => alp.EducationLevel)
                .ThenBy(alp => alp.ClassLevel)
                .ToList();
        }

        public ArchivedSchoolPaymentSummary? GetArchivedSchoolPaymentSummary(string academicYear, Semester semester)
        {
            return _context.ArchivedSchoolPaymentSummaries
                .AsNoTracking()
                .FirstOrDefault(a =>
                    a.AcademicYear == academicYear &&
                    a.Semester == semester
                );
        }

        public void ArchiveSchoolPayments()
        {
            var currentPeriod = GetCurrentAcademicPeriod();
            if (currentPeriod == null)
                throw new InvalidOperationException("No active academic period found.");

            var existing = _context.ArchivedSchoolPaymentSummaries
                .AsNoTracking()
                .FirstOrDefault(a =>
                    a.AcademicYear == currentPeriod.AcademicYear &&
                    a.Semester == currentPeriod.Semester);

            if (existing != null)
                return;

            var learningPaths = GetLearningPathsForPeriod(currentPeriod.Id);
            var allFees = GetSchoolFeesForPeriodStudents(currentPeriod.Id);

            var summary = LogicMethods.CalculateSchoolPaymentSummary(learningPaths, allFees);

            var learningPathIds = learningPaths.Select(learningPath => learningPath.Id).ToHashSet();
            var currentFees = allFees
                .Where(fees => learningPathIds.Contains(fees.LearningPathId))
                .ToList();

            var archive = new ArchivedSchoolPaymentSummary
            {
                AcademicYear = currentPeriod.AcademicYear,
                Semester = currentPeriod.Semester,
                SemesterStartDate = currentPeriod.SemesterStartDate,
                SemesterEndDate = currentPeriod.SemesterEndDate,
                ArchivedDate = DateTime.Now,
                TotalLearningPaths = summary.TotalLearningPaths,
                TotalStudents = summary.TotalStudents,
                FullyPaidStudents = summary.FullyPaidStudents,
                StudentsWithBalance = summary.StudentsWithBalance,
                TotalExpectedRevenue = summary.TotalExpectedRevenue,
                TotalAmountReceived = summary.TotalAmountReceived,
                TotalOutstandingBalance = summary.TotalOutstanding,
                TotalBroughtForwardOutstanding = summary.TotalBroughtForwardOutstanding,
                SchoolWidePaymentCompletionRate = summary.PaymentCompletionRate,
                SchoolWideTimelyCompletionRate = summary.TimelyCompletionRate
            };

            _context.ArchivedSchoolPaymentSummaries.Add(archive);
            _context.SaveChanges();
        }

        public void ArchiveLearningPathPayments(LearningPath lp)
        {
            if (lp == null)
                throw new ArgumentNullException(nameof(lp));

            var exists = _context.ArchivedLearningPathPayments
                .AsNoTracking()
                .Any(a =>
                    a.LearningPathId == lp.Id &&
                    a.AcademicYear == lp.AcademicYear &&
                    a.Semester == lp.Semester);

            if (exists)
                return;

            var feesInPath = GetSchoolFeesForLearningPath(lp.Id);
            var enrolledCount = GetEnrolledStudentCount(lp.Id);

            var studentIds = feesInPath.Select(fees => fees.StudentId).Distinct().ToList();

            var allStudentFees = _context.SchoolFees
                .AsNoTracking()
                .Where(fees => studentIds.Contains(fees.StudentId))
                .Include(fees => fees.Payments)
                .AsSplitQuery()
                .ToList();

            var summary = LogicMethods.CalculateLearningPathPaymentSummary(lp, allStudentFees, feesInPath, enrolledCount);

            var archive = new ArchivedLearningPathPayment
            {
                LearningPathId = lp.Id,
                LearningPathName = $"{lp.EducationLevel} - {lp.ClassLevel}",
                EducationLevel = lp.EducationLevel,
                ClassLevel = lp.ClassLevel,
                Semester = lp.Semester,
                AcademicYear = lp.AcademicYear,
                TotalStudentsInPath = summary.StudentCount,
                FeePerStudent = summary.FeePerSemester,
                LearningPathExpectedRevenue = summary.ExpectedRevenue,
                TotalPaid = summary.TotalPaid,
                Outstanding = summary.Outstanding,
                LearningPathPaymentCompletionRate = summary.PaymentCompletionRate,
                LearningPathTimelyCompletionRate = summary.TimelyCompletionRate,
                SemesterStartDate = lp.SemesterStartDate,
                SemesterEndDate = lp.SemesterEndDate,
                ArchivedDate = DateTime.Now
            };

            _context.ArchivedLearningPathPayments.Add(archive);
            _context.SaveChanges();
        }

        public ArchivedLearningPathGrade? GetArchivedLearningPathGrade(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            return _context.ArchivedLearningPathGrades
                .AsNoTracking()
                .Include(a => a.StudentGrades)
                    .ThenInclude(sg => sg.CourseGrades)
                        .ThenInclude(cg => cg.TestGrades)
                .Include(a => a.StudentGrades)
                    .ThenInclude(sg => sg.ArchivedReportCard)
                .FirstOrDefault(a =>
                    a.AcademicYear == academicYear &&
                    a.EducationLevel == educationLevel &&
                    a.ClassLevel == classLevel &&
                    a.Semester == semester
                );
        }

        public void ArchiveStudentReportCard(StudentReportCard reportCard)
        {
            var archivedPath = _context.ArchivedLearningPathGrades
                .Include(a => a.StudentGrades)
                    .ThenInclude(sg => sg.CourseGrades)
                .FirstOrDefault(a =>
                    a.LearningPathId == reportCard.LearningPathId &&
                    a.AcademicYear == reportCard.LearningPath.AcademicYearStart.Year + "-" + (reportCard.LearningPath.AcademicYearStart.Year + 1) &&
                    a.EducationLevel == reportCard.LearningPath.EducationLevel &&
                    a.ClassLevel == reportCard.LearningPath.ClassLevel &&
                    a.Semester == reportCard.LearningPath.Semester
                );

            if (archivedPath == null)
                return;

            var archivedStudent = archivedPath.StudentGrades
                .FirstOrDefault(s => s.StudentId == reportCard.StudentId);

            if (archivedStudent == null)
                return;

            archivedStudent.ArchivedReportCard = reportCard;

            _context.ArchivedStudentGrades.Update(archivedStudent);
            _context.SaveChanges();
        }

        public void ArchiveLearningPathGrades(LearningPath learningPath)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));

            var dbLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.GradingConfiguration)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                        .ThenInclude(cg => cg.TestGrades)
                .Include(lp => lp.AttendanceLog)
                .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (dbLearningPath == null)
                throw new InvalidOperationException("Learning path not found for archiving grades.");

            bool alreadyArchived = _context.ArchivedLearningPathGrades
                .AsNoTracking()
                .Any(a =>
                    a.LearningPathId == dbLearningPath.Id &&
                    a.AcademicYear == dbLearningPath.AcademicYear &&
                    a.EducationLevel == dbLearningPath.EducationLevel &&
                    a.ClassLevel == dbLearningPath.ClassLevel &&
                    a.Semester == dbLearningPath.Semester);

            if (alreadyArchived)
                return;

            var gradeReport = LogicMethods.GenerateLearningPathGradeReport(dbLearningPath);

            var rankLookup = new Dictionary<int, (int Rank, double Grade)>();
            int rankPosition = 1;
            foreach (var summary in gradeReport.RankedStudents)
            {
                rankLookup[summary.Student.Id] = (rankPosition, summary.SemesterOverallGrade);
                rankPosition++;
            }

            var archivedLearningPath = new ArchivedLearningPathGrade
            {
                LearningPathId = dbLearningPath.Id,
                LearningPathName = Util.GetLearningPathName(dbLearningPath),
                AcademicYear = dbLearningPath.AcademicYear,
                EducationLevel = dbLearningPath.EducationLevel,
                ClassLevel = dbLearningPath.ClassLevel,
                Semester = dbLearningPath.Semester,
                SemesterStartDate = dbLearningPath.SemesterStartDate,
                SemesterEndDate = dbLearningPath.SemesterEndDate,
                TotalStudentsInPath = dbLearningPath.Students.Count,
                ArchivedDate = DateTime.Now,
                StudentGrades = new List<ArchivedStudentGrade>()
            };

            double totalSemesterGrade = 0;
            int countedStudents = 0;

            foreach (var student in dbLearningPath.Students)
            {
                if (!rankLookup.TryGetValue(student.Id, out var rankInfo))
                {
                    var fallbackGrade = LogicMethods.CalculateSemesterOverallGrade(student, dbLearningPath);
                    rankInfo = (0, fallbackGrade);
                }

                var semesterGrades = GetStudentAllSemesterGrades(
                    student.Id,
                    dbLearningPath.EducationLevel,
                    dbLearningPath.ClassLevel);

                double firstSemesterGrade = semesterGrades.GetValueOrDefault(Semester.First, 0);
                double secondSemesterGrade = semesterGrades.GetValueOrDefault(Semester.Second, 0);
                double thirdSemesterGrade = semesterGrades.GetValueOrDefault(Semester.Third, 0);

                double promotionGrade = LogicMethods.CalculatePromotionGrade(semesterGrades);

                bool isPromoted = promotionGrade >= FcmsConstants.PASSING_GRADE;

                string promotionStatus = Util.GetPromotionStatusForArchive(dbLearningPath, isPromoted);

                var (presentDays, totalDays, attendanceRate) =
                    LogicMethods.CalculateStudentAttendance(dbLearningPath.AttendanceLog, student.Id);

                var guardian = GetGuardianByStudentId(student.Id);

                var archivedStudent = new ArchivedStudentGrade
                {
                    StudentId = student.Id,
                    StudentName = Util.GetFullName(student.Person),
                    StudentAge = student.Person.Age,
                    StudentEmail = student.Person.Email ?? string.Empty,

                    GuardianName = guardian != null ? Util.GetFullName(guardian.Person) : string.Empty,
                    GuardianEmail = guardian?.Person.Email ?? string.Empty,
                    GuardianPhoneNumber = guardian?.Person.PhoneNumber ?? string.Empty,

                    SemesterOverallGrade = rankInfo.Grade,
                    StudentRank = rankInfo.Rank,
                    PromotionGrade = promotionGrade,

                    PresentDays = presentDays,
                    TotalDays = totalDays,
                    AttendanceRate = attendanceRate,

                    IsPromoted = isPromoted,
                    PromotionStatus = promotionStatus,

                    FirstSemesterGrade = firstSemesterGrade,
                    SecondSemesterGrade = secondSemesterGrade,
                    ThirdSemesterGrade = thirdSemesterGrade,

                    CourseGrades = new List<ArchivedCourseGrade>()
                };

                totalSemesterGrade += rankInfo.Grade;
                countedStudents++;

                var courseGrades = student.CourseGrades
                    .Where(cg => cg.LearningPathId == dbLearningPath.Id)
                    .ToList();

                foreach (var courseGrade in courseGrades)
                {
                    var config = courseGrade.GradingConfiguration;
                    double homeworkWeight = config?.HomeworkWeightPercentage ?? FcmsConstants.DEFAULT_HOMEWORK_WEIGHT;
                    double quizWeight = config?.QuizWeightPercentage ?? FcmsConstants.DEFAULT_QUIZ_WEIGHT;
                    double examWeight = config?.FinalExamWeightPercentage ?? FcmsConstants.DEFAULT_EXAM_WEIGHT;

                    var homeworkTests = courseGrade.TestGrades
                        .Where(t => t.GradeType == GradeType.Homework)
                        .ToList();
                    var quizTests = courseGrade.TestGrades
                        .Where(t => t.GradeType == GradeType.Quiz)
                        .ToList();
                    var examTests = courseGrade.TestGrades
                        .Where(t => t.GradeType == GradeType.Exam)
                        .ToList();

                    double homeworkAverage = homeworkTests.Any() ? homeworkTests.Average(t => t.Score) : 0;
                    double quizAverage = quizTests.Any() ? quizTests.Average(t => t.Score) : 0;
                    double examAverage = examTests.Any() ? examTests.Average(t => t.Score) : 0;

                    var archivedCourse = new ArchivedCourseGrade
                    {
                        Course = courseGrade.Course,
                        TotalGrade = courseGrade.TotalGrade,
                        FinalGradeCode = string.IsNullOrEmpty(courseGrade.FinalGradeCode)
                            ? Util.GetGradeCode(courseGrade.TotalGrade)
                            : courseGrade.FinalGradeCode,

                        HomeworkWeightPercentage = homeworkWeight,
                        QuizWeightPercentage = quizWeight,
                        FinalExamWeightPercentage = examWeight,

                        HomeworkAverage = homeworkAverage,
                        QuizAverage = quizAverage,
                        ExamAverage = examAverage,

                        TestGrades = new List<ArchivedTestGrade>()
                    };

                    foreach (var test in courseGrade.TestGrades)
                    {
                        archivedCourse.TestGrades.Add(new ArchivedTestGrade
                        {
                            Score = test.Score,
                            Date = test.Date,
                            GradeType = test.GradeType,
                            TeacherName = test.Teacher?.Person != null
                                    ? Util.GetFullName(test.Teacher.Person)
                                    : string.Empty,
                            TeacherRemark = test.TeacherRemark
                        });
                    }

                    archivedStudent.CourseGrades.Add(archivedCourse);
                }

                archivedLearningPath.StudentGrades.Add(archivedStudent);
            }

            if (countedStudents > 0)
            {
                archivedLearningPath.AverageClassGrade = Math.Round(
                    totalSemesterGrade / countedStudents,
                    FcmsConstants.GRADE_ROUNDING_DIGIT);
            }

            _context.ArchivedLearningPathGrades.Add(archivedLearningPath);
            _context.SaveChanges();
        }

        public List<ArchivedStudentGrade> GetStudentArchivedGrades(int studentId)
        {
            return _context.ArchivedStudentGrades
                .AsNoTracking()
                .Where(asg => asg.StudentId == studentId)
                .Include(asg => asg.ArchivedLearningPathGrade)
                .Include(asg => asg.CourseGrades)
                .AsSplitQuery()
                .ToList();
        }

        public List<ArchivedLearningPathGrade> GetArchivedLearningPathsForStudent(int studentId)
        {
            return _context.ArchivedStudentGrades
                .AsNoTracking()
                .Where(sg => sg.StudentId == studentId && sg.ArchivedLearningPathGrade != null)
                .Select(sg => sg.ArchivedLearningPathGrade!)
                .OrderBy(a => a.SemesterStartDate)
                .ToList();
        }
        #endregion

        #region Announcements
        public List<Announcement> GetAllAnnouncements()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.Announcements
                .AsNoTracking()
                .Include(a => a.PostedBy)
                .OrderByDescending(a => a.PostedAt)
                .ToList();
        }


        public List<Announcement> GetActiveAnnouncements()
        {
            using var context = _contextFactory.CreateDbContext();

            var today = DateTime.Today;

            return context.Announcements
                .AsNoTracking()
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .OrderByDescending(a => a.PostedAt)
                .ToList();
        }

        public Announcement CreateAnnouncement(Announcement announcement)
        {
            using var context = _contextFactory.CreateDbContext();
            var totalAnnouncements = context.Announcements.Count();
            if (totalAnnouncements >= FcmsConstants.MAX_ANNOUNCEMENTS_COUNT)
                throw new BusinessRuleException($"You can only keep up to {FcmsConstants.MAX_ANNOUNCEMENTS_COUNT} active announcements. Please delete an existing one before adding a new announcement.");

            announcement.PostedAt = DateTime.Now;
            context.Announcements.Add(announcement);
            context.SaveChanges();

            return announcement;
        }

        public Announcement UpdateAnnouncement(Announcement announcement)
        {
            using var context = _contextFactory.CreateDbContext();
            var existing = context.Announcements.Find(announcement.Id);
            if (existing == null)
                throw new InvalidOperationException("Announcement not found.");

            existing.Message = announcement.Message;
            existing.StartDate = announcement.StartDate;
            existing.EndDate = announcement.EndDate;

            context.SaveChanges();
            return existing;
        }

        public void DeleteAnnouncement(int announcementId)
        {
            using var context = _contextFactory.CreateDbContext();

            var announcement = context.Announcements.Find(announcementId);
            if (announcement != null)
            {
                context.Announcements.Remove(announcement);
                context.SaveChanges();
            }
        }
        #endregion

        #region Quotes
        public List<Quote> GetAllQuotes()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.Quotes
                .AsNoTracking()
                .Include(q => q.AddedBy)
                .OrderByDescending(q => q.DateAdded)
                .ToList();
        }

        public Quote CreateQuote(Quote quote)
        {
            using var context = _contextFactory.CreateDbContext();

            var totalQuotes = context.Quotes.Count();
            if (totalQuotes >= FcmsConstants.MAX_QUOTES_COUNT)
                throw new BusinessRuleException($"You can only store up to {FcmsConstants.MAX_QUOTES_COUNT} quotes. Please delete one before adding another.");

            quote.DateAdded = DateTime.Now;
            context.Quotes.Add(quote);
            context.SaveChanges();

            return quote;
        }

        public Quote UpdateQuote(Quote quote)
        {
            using var context = _contextFactory.CreateDbContext();

            var existing = context.Quotes.Find(quote.Id);
            if (existing == null)
                throw new InvalidOperationException("Quote not found.");

            existing.Text = quote.Text;
            existing.Author = quote.Author;

            context.SaveChanges();
            return existing;
        }

        public void DeleteQuote(int quoteId)
        {
            using var context = _contextFactory.CreateDbContext();

            var quote = context.Quotes.Find(quoteId);
            if (quote != null)
            {
                context.Quotes.Remove(quote);
                context.SaveChanges();
            }
        }
        #endregion

        #region Academic Period Management
        public void SetSchoolAcademicPeriod(AcademicPeriod academicPeriod)
        {
            if (academicPeriod.SemesterStartDate >= academicPeriod.SemesterEndDate)
                throw new BusinessRuleException("Semester start date must be before semester end date.");

            if (academicPeriod.ExamsStartDate.HasValue &&
                (academicPeriod.ExamsStartDate < academicPeriod.SemesterStartDate ||
                 academicPeriod.ExamsStartDate > academicPeriod.SemesterEndDate))
                throw new BusinessRuleException("Exams start date must fall within the semester dates.");

            using var context = _contextFactory.CreateDbContext();

            var school = context.School.FirstOrDefault();
            if (school == null)
                throw new InvalidOperationException("No school found.");

            var existingPeriod = context.AcademicPeriods
                .FirstOrDefault(ap => ap.AcademicYearStart.Year == academicPeriod.AcademicYearStart.Year &&
                                      ap.Semester == academicPeriod.Semester);

            if (existingPeriod == null)
            {
                context.AcademicPeriods.Add(academicPeriod);
                school.CurrentAcademicPeriod = academicPeriod;
            }
            else
            {
                existingPeriod.SemesterStartDate = academicPeriod.SemesterStartDate;
                existingPeriod.SemesterEndDate = academicPeriod.SemesterEndDate;
                existingPeriod.ExamsStartDate = academicPeriod.ExamsStartDate;
                school.CurrentAcademicPeriodId = existingPeriod.Id;
            }

            context.SaveChanges();

            InvalidateAcademicPeriodCache();
        }

        private void InvalidateAcademicPeriodCache()
        {
            Interlocked.Increment(ref _academicPeriodVersion);
            _cachedCurrentAcademicPeriod = null;
            _academicPeriodLoaded = false;
        }

        public AcademicPeriod? GetCurrentAcademicPeriod()
        {
            if (_academicPeriodLoaded && _cachedAcademicPeriodVersion == _academicPeriodVersion)
                return _cachedCurrentAcademicPeriod;

            using var context = _contextFactory.CreateDbContext();
            _cachedCurrentAcademicPeriod = context.School
                .AsNoTracking()
                .Select(s => s.CurrentAcademicPeriod)
                .FirstOrDefault();

            _academicPeriodLoaded = true;
            _cachedAcademicPeriodVersion = _academicPeriodVersion;
            return _cachedCurrentAcademicPeriod;
        }

        public AcademicPeriod? GetAcademicPeriodByYearAndSemester(int academicYearStartYear, Semester semester)
        {
            using var context = _contextFactory.CreateDbContext();

            return context.AcademicPeriods
                .AsNoTracking()
                .FirstOrDefault(ap => ap.AcademicYearStart.Year == academicYearStartYear && ap.Semester == semester);
        }

        public List<AcademicPeriod> GetAllAcademicPeriods()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.AcademicPeriods
                .AsNoTracking()
                .OrderByDescending(ap => ap.AcademicYearStart)
                .ThenBy(ap => ap.Semester)
                .ToList();
        }
        #endregion

        #region Account Management
        public void ActivatePerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person), "Person cannot be null.");

            person.IsActive = true;
            _context.SaveChanges();
        }

        public void DeactivatePerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person), "Person cannot be null.");

            person.IsActive = false;
            _context.SaveChanges();
        }
        #endregion

        #region Dashboard
        public ScheduleEntry? GetLatestEventOrMeetingForDate(DateTime date)
        {
            using var context = _contextFactory.CreateDbContext();

            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            return context.ScheduleEntries
                .AsNoTracking()
                .Where(e =>
                    e.DateTime >= dayStart && e.DateTime < dayEnd &&
                    (e.Event != null || e.Meeting != null))
                .OrderByDescending(e => e.DateTime)
                .FirstOrDefault();
        }

        public List<ClassSchedule> GetTodayClassSessionsForStudent(int studentId, int maxCount)
        {
            var student = _context.Students
                .AsNoTracking()
                .Include(s => s.LearningPath)
                .FirstOrDefault(s => s.Id == studentId);

            if (student?.LearningPath == null)
                return new List<ClassSchedule>();

            var now = DateTime.Now;
            var tomorrow = DateTime.Today.AddDays(1);

            return _context.ClassSchedules
                .AsNoTracking()
                .Include(sched => sched.ClassSession)
                .Where(sched => sched.ClassLevel == student.LearningPath.ClassLevel &&
                                sched.Semester == student.LearningPath.Semester &&
                                sched.ClassSessionId != null &&
                                sched.DateTime >= now &&
                                sched.DateTime < tomorrow)
                .OrderBy(sched => sched.DateTime)
                .Take(maxCount)
                .ToList();
        }

        public List<PendingHomeworkItem> GetPendingHomeworkForStudent(int studentId, int maxCount)
        {
            var student = _context.Students
                .AsNoTracking()
                .Include(s => s.LearningPath)
                .FirstOrDefault(s => s.Id == studentId);

            if (student?.LearningPath == null)
                return new List<PendingHomeworkItem>();

            return _context.ClassSessions
                .AsNoTracking()
                .Where(cs => cs.ClassLevel == student.LearningPath.ClassLevel &&
                             cs.Semester == student.LearningPath.Semester &&
                             cs.HomeworkDetails != null &&
                             !cs.HomeworkDetails.Submissions.Any(sub => sub.StudentId == studentId))
                .OrderBy(cs => cs.HomeworkDetails!.DueDate)
                .Select(cs => new PendingHomeworkItem
                {
                    HomeworkId = cs.HomeworkDetails!.Id,
                    Title = cs.HomeworkDetails.Title,
                    Course = cs.Course,
                    AssignedDate = cs.HomeworkDetails.AssignedDate,
                    DueDate = cs.HomeworkDetails.DueDate
                })
                .Take(maxCount)
                .ToList();
        }

        public List<(string Course, GradeType GradeType, double Score, int SortKey)> GetRecentGradesForStudent(int studentId, int maxCount)
        {
            var student = _context.Students
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null || student.LearningPathId == null || student.LearningPathId == 0)
                return new List<(string, GradeType, double, int)>();

            return _context.TestGrades
                .AsNoTracking()
                .Include(tg => tg.CourseGrade)
                .Where(tg => tg.CourseGrade != null &&
                             tg.CourseGrade.StudentId == studentId &&
                             tg.CourseGrade.LearningPathId == student.LearningPathId)
                .OrderByDescending(tg => tg.Id)
                .Take(maxCount)
                .Select(tg => new ValueTuple<string, GradeType, double, int>(
                    tg.CourseGrade!.Course,
                    tg.GradeType,
                    tg.Score,
                    tg.Id))
                .ToList();
        }

        public List<(string LearningPathName, DateTime Timestamp)> GetTodayAttendanceReports(int maxCount)
        {
            var academicPeriod = GetCurrentAcademicPeriod();
            if (academicPeriod == null)
                return new List<(string, DateTime)>();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.DailyAttendanceLogEntries
                .AsNoTracking()
                .Include(log => log.LearningPath)
                .Where(log => log.TimeStamp >= today &&
                              log.TimeStamp < tomorrow &&
                              log.LearningPath.AcademicPeriodId == academicPeriod.Id)
                .OrderByDescending(log => log.TimeStamp)
                .Take(maxCount)
                .Select(log => new ValueTuple<string, DateTime>(
                    log.LearningPath.EducationLevel.ToDisplayName() + " " + log.LearningPath.ClassLevel.ToDisplayName(),
                    log.TimeStamp))
                .ToList();
        }

        public List<(string Course, string ClassLevelName, DateTime Timestamp)> GetTodayTeacherRemarks(int maxCount)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.ClassSessions
                .AsNoTracking()
                .Where(cs => cs.TeacherRemarks != "" &&
                             cs.RemarksSubmittedAt.HasValue &&
                             cs.RemarksSubmittedAt.Value >= today &&
                             cs.RemarksSubmittedAt.Value < tomorrow)
                .OrderByDescending(cs => cs.RemarksSubmittedAt)
                .Take(maxCount)
                .Select(cs => new
                {
                    cs.Course,
                    cs.ClassLevel,
                    Timestamp = cs.RemarksSubmittedAt!.Value
                })
                .ToList()
                .Select(x => new ValueTuple<string, string, DateTime>(
                    x.Course,
                    x.ClassLevel.ToDisplayName(),
                    x.Timestamp))
                .ToList();
        }

        public List<(string ClassLevelName, string AcademicYear, string Term, DateTime DateSubmitted)> GetRecentlySubmittedLearningPaths(int maxCount)
        {
            var academicPeriod = GetCurrentAcademicPeriod();
            if (academicPeriod == null)
                return new List<(string, string, string, DateTime)>();

            return _context.LearningPaths
                .AsNoTracking()
                .Where(lp => lp.AcademicPeriodId == academicPeriod.Id &&
                             lp.DateSubmitted.HasValue &&
                             lp.ApprovalStatus == PrincipalApprovalStatus.Review)
                .OrderByDescending(lp => lp.DateSubmitted)
                .Take(maxCount)
                .Select(lp => new
                {
                    lp.ClassLevel,
                    lp.AcademicYearStart,
                    lp.Semester,
                    DateSubmitted = lp.DateSubmitted!.Value
                })
                .ToList()
                .Select(lp => new ValueTuple<string, string, string, DateTime>(
                    lp.ClassLevel.ToDisplayName(),
                    $"{lp.AcademicYearStart.Year}-{lp.AcademicYearStart.Year + 1}",
                    lp.Semester.ToTermDisplay(),
                    lp.DateSubmitted))
                .ToList();
        }

        public int GetStaffCount()
        {
            return _context.Staff.AsNoTracking().Count();
        }

        public int GetStudentCount()
        {
            return _context.Students.AsNoTracking()
                .Include(s => s.Person)
                .Count(s => !s.Person.IsArchived);
        }

        public int GetGuardianCount()
        {
            return _context.Guardians.AsNoTracking().Count();
        }

        public int GetActiveClassCount()
        {
            var academicPeriod = GetCurrentAcademicPeriod();
            if (academicPeriod == null) return 0;

            return _context.LearningPaths.AsNoTracking()
                .Count(lp => lp.AcademicPeriodId == academicPeriod.Id && 
                             lp.ApprovalStatus != PrincipalApprovalStatus.Approved &&
                             lp.ApprovalStatus == PrincipalApprovalStatus.Pending);
        }

        public List<ClassSchedule> GetTodayClassSessionsForTeacher(int teacherId, int maxCount)
        {
            var now = DateTime.Now;
            var tomorrow = DateTime.Today.AddDays(1);

            return _context.ClassSchedules
                .AsNoTracking()
                .Include(sched => sched.ClassSession)
                .Where(sched => sched.ClassSession != null &&
                                sched.ClassSession.TeacherId == teacherId &&
                                sched.DateTime >= now &&
                                sched.DateTime < tomorrow)
                .OrderBy(sched => sched.DateTime)
                .Take(maxCount)
                .ToList();
        }

        public List<TeacherSubmissionItem> GetRecentHomeworkSubmissionsForTeacher(int teacherId, int maxCount)
        {
            var academicPeriod = GetCurrentAcademicPeriod();
            if (academicPeriod == null)
                return new List<TeacherSubmissionItem>();

            return _context.HomeworkSubmissions
                .AsNoTracking()
                .Where(hs => hs.Homework != null &&
                             hs.Homework.ClassSession != null &&
                             hs.Homework.ClassSession.TeacherId == teacherId &&
                             hs.Student != null &&
                             !hs.IsGraded &&
                             hs.SubmissionDate >= academicPeriod.SemesterStartDate)
                .OrderByDescending(hs => hs.SubmissionDate)
                .Select(hs => new TeacherSubmissionItem
                {
                    StudentName = hs.Student!.Person.FirstName + " " + hs.Student.Person.LastName,
                    ClassLevel = hs.Student.LearningPath != null ? hs.Student.LearningPath.ClassLevel : (ClassLevel?)null,
                    Course = hs.Homework!.ClassSession!.Course,
                    Title = hs.Homework.Title,
                    AssignedDate = hs.Homework.AssignedDate,
                    DueDate = hs.Homework.DueDate,
                    SubmittedDate = hs.SubmissionDate
                })
                .Take(maxCount)
                .ToList();
        }
        #endregion

        #region Pagination and Filtering
        public IQueryable<StudentListItem> GetStudentsForList()
        {
            return _context.Students
                .AsNoTracking()
                .Where(s => !s.Person.IsArchived)
                .OrderBy(s => s.Person.FirstName)
                .Select(s => new StudentListItem
                {
                    Id = s.Id,
                    FirstName = s.Person.FirstName,
                    MiddleName = s.Person.MiddleName,
                    LastName = s.Person.LastName,
                    ProfilePictureUrl = s.Person.ProfilePictureUrl,
                    DateOfBirth = s.Person.DateOfBirth,
                    EducationLevel = s.Person.EducationLevel,
                    ClassLevel = s.Person.ClassLevel,
                    IsActive = s.Person.IsActive,
                    LearningPathId = s.LearningPathId
                });
        }

        public IQueryable<StaffListItem> GetStaffForList()
        {
            return _context.Staff
                .AsNoTracking()
                .OrderBy(s => s.Person.FirstName)
                .Select(s => new StaffListItem
                {
                    Id = s.Id,
                    FirstName = s.Person.FirstName,
                    MiddleName = s.Person.MiddleName,
                    LastName = s.Person.LastName,
                    ProfilePictureUrl = s.Person.ProfilePictureUrl,
                    Email = s.Person.Email,
                    PhoneNumber = s.Person.PhoneNumber,
                    UserRole = s.UserRole,
                    IsActive = s.Person.IsActive
                });
        }

        public IQueryable<GuardianListItem> GetGuardiansForList()
        {
            return _context.Guardians
                .AsNoTracking()
                .OrderBy(g => g.Person.FirstName)
                .Select(g => new GuardianListItem
                {
                    Id = g.Id,
                    FirstName = g.Person.FirstName,
                    MiddleName = g.Person.MiddleName,
                    LastName = g.Person.LastName,
                    ProfilePictureUrl = g.Person.ProfilePictureUrl,
                    Email = g.Person.Email,
                    PhoneNumber = g.Person.PhoneNumber,
                    IsActive = g.Person.IsActive
                });
        }

        public IQueryable<LearningPathListItem> GetLearningPathsForList()
        {
            return _context.LearningPaths
                .AsNoTracking()
                
                .OrderBy(lp => lp.ClassLevel)
                    .ThenBy(lp => lp.Id)
                .Select(lp => new LearningPathListItem
                {
                    Id = lp.Id,
                    EducationLevel = lp.EducationLevel,
                    ClassLevel = lp.ClassLevel,
                    Semester = lp.Semester,
                    AcademicYearStart = lp.AcademicYearStart,
                    AcademicPeriodId = lp.AcademicPeriodId,
                    ApprovalStatus = lp.ApprovalStatus
                });
        }

        public List<LearningPathListItem> GetEnrollableLearningPaths(EducationLevel educationLevel, ClassLevel classLevel)
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(lp =>lp.ApprovalStatus != PrincipalApprovalStatus.Approved &&
                             lp.EducationLevel == educationLevel &&
                             lp.ClassLevel == classLevel)
                .OrderByDescending(lp => lp.AcademicYearStart)
                    .ThenBy(lp => lp.Semester)
                .Select(lp => new LearningPathListItem
                {
                    Id = lp.Id,
                    EducationLevel = lp.EducationLevel,
                    ClassLevel = lp.ClassLevel,
                    Semester = lp.Semester,
                    AcademicYearStart = lp.AcademicYearStart,
                    AcademicPeriodId = lp.AcademicPeriodId,
                    ApprovalStatus = lp.ApprovalStatus
                })
                .ToList();
        }
        #endregion
    }
}

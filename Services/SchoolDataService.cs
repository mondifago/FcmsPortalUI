using FcmsPortal.Constants;
using FcmsPortal.Enums;
using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;


namespace FcmsPortal.Services
{
    public class SchoolDataService : ISchoolDataService
    {
        private readonly FcmsPortalUIContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly Dictionary<string, List<(int referenceId, FileAttachment attachment)>> _attachmentReferences = new();
        private int _nextAttachmentId = 1;
        private List<Payment> _payments = new List<Payment>();
        private List<SchoolFees> _schoolFees = new List<SchoolFees>();
        private List<Student> _archivedStudents = new List<Student>();
        private List<Address> _addresses = new List<Address>();
        private static readonly object _idLock = new object();
        private static readonly Dictionary<string, int> _entityCounters = new Dictionary<string, int>();

        public SchoolDataService(FcmsPortalUIContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        #region School

        public School? GetSchool()
        {
            return _context.School
                .Include(s => s.Address)
                .Include(s => s.Staff)
                    .ThenInclude(st => st.Person)
                .Include(s => s.Students)
                    .ThenInclude(st => st.Person)
                .Include(s => s.Guardians)
                    .ThenInclude(g => g.Person)
                .Include(s => s.LearningPaths)
                .Include(s => s.SchoolCalendar)
                .FirstOrDefault();
        }

        public bool HasSchool()
        {
            return _context.School.Any();
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
            _context.SaveChangesAsync();
            return school;
        }

        public async Task UpdateSchoolAsync(School updatedSchool)
        {
            var existingSchool = await _context.School
                .Include(s => s.Address)
                .FirstOrDefaultAsync();

            if (existingSchool != null)
            {
                existingSchool.Name = updatedSchool.Name;
                existingSchool.LogoUrl = updatedSchool.LogoUrl;
                existingSchool.Email = updatedSchool.Email;
                existingSchool.PhoneNumber = updatedSchool.PhoneNumber;
                existingSchool.WebsiteUrl = updatedSchool.WebsiteUrl;

                if (existingSchool.Address != null && updatedSchool.Address != null)
                {
                    existingSchool.Address.Street = updatedSchool.Address.Street;
                    existingSchool.Address.City = updatedSchool.Address.City;
                    existingSchool.Address.State = updatedSchool.Address.State;
                    existingSchool.Address.PostalCode = updatedSchool.Address.PostalCode;
                    existingSchool.Address.Country = updatedSchool.Address.Country;
                }

                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Addresses
        public Address AddAddress(Address address)
        {
            _context.Addresses.Add(address);
            _context.SaveChanges();
            return address;
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
                existingStaff.JobRole = staff.JobRole;
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

                _context.SaveChanges();
            }
        }

        public bool DeleteStaff(int staffId)
        {
            var staff = _context.Staff.Find(staffId);
            if (staff == null)
            {
                return false;
            }

            _context.Staff.Remove(staff);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Guardians
        public IEnumerable<Guardian> GetGuardians()
        {
            return _context.Guardians
                .Include(g => g.Person)
                    .ThenInclude(p => p.Addresses)
                .Include(g => g.Wards)
                    .ThenInclude(w => w.Person)
                .ToList();
        }

        public Guardian? GetGuardianById(int id)
        {
            return _context.Guardians
                .Include(g => g.Person)
                    .ThenInclude(p => p.Addresses)
                .Include(g => g.Wards)
                    .ThenInclude(w => w.Person)
                .FirstOrDefault(g => g.Id == id);
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
            var guardian = _context.Guardians.Find(guardianId);
            if (guardian == null)
            {
                return false;
            }

            _context.Guardians.Remove(guardian);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Students
        public IEnumerable<Student> GetStudents()
        {
            return _context.Students
                .Include(s => s.Person)
                    .ThenInclude(p => p.Addresses)
                .Include(s => s.CourseGrades)
                .ToList();
        }

        public Student? GetStudentById(int id)
        {
            return _context.Students
                .Include(s => s.Person)
                    .ThenInclude(p => p.Addresses)
                .Include(s => s.CourseGrades)
                .FirstOrDefault(s => s.Id == id);
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
                existingStudent.PositionAmongSiblings = student.PositionAmongSiblings;
                existingStudent.LastSchoolAttended = student.LastSchoolAttended;
                existingStudent.GuardianId = student.GuardianId;

                _context.SaveChanges();
            }
        }

        public Student AddStudent(Student student)
        {
            if (student.Person.SchoolFees == null)
            {
                student.Person.SchoolFees = new SchoolFees
                {
                    TotalAmount = 0,
                    Payments = new List<Payment>(),
                };
            }
            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }


        public bool DeleteStudent(int studentId)
        {
            var student = _context.Students.Find(studentId);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            _context.SaveChanges();
            return true;
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

        public LearningPath? GetLearningPathById(int id)
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.StudyMaterials)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.DiscussionThreads)
                .Include(lp => lp.StudentsWithAccess)
                    .ThenInclude(s => s.Person)
                .FirstOrDefault(lp => lp.Id == id);
        }

        public IEnumerable<LearningPath> GetAllLearningPaths()
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                .Where(lp => !lp.IsTemplate)
                .ToList();
        }

        public LearningPath? GetLearningPathByScheduleEntry(int scheduleEntryId)
        {
            return _context.LearningPaths
                .Include(lp => lp.Schedule)
                .FirstOrDefault(lp => lp.Schedule.Any(s => s.Id == scheduleEntryId));
        }

        public LearningPath? GetLearningPathByClassSessionId(int classSessionId)
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                .FirstOrDefault(lp => lp.Schedule.Any(s => s.ClassSession != null && s.ClassSession.Id == classSessionId));
        }

        public bool DeleteLearningPath(int id)
        {
            var learningPath = _context.LearningPaths.Find(id);
            if (learningPath == null)
            {
                return false;
            }

            _context.LearningPaths.Remove(learningPath);
            _context.SaveChanges();
            return true;
        }

        public void UpdateLearningPath(LearningPath learningPath)
        {
            var existingLearningPath = _context.LearningPaths
                   .Include(lp => lp.Students)
                   .Include(lp => lp.StudentsWithAccess)
                   .Include(lp => lp.Schedule)
                   .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath != null)
            {
                existingLearningPath.SemesterStartDate = learningPath.SemesterStartDate;
                existingLearningPath.SemesterEndDate = learningPath.SemesterEndDate;
                existingLearningPath.ExamsStartDate = learningPath.ExamsStartDate;
                existingLearningPath.FeePerSemester = learningPath.FeePerSemester;
                existingLearningPath.ApprovalStatus = learningPath.ApprovalStatus;
                existingLearningPath.EducationLevel = learningPath.EducationLevel;
                existingLearningPath.ClassLevel = learningPath.ClassLevel;
                existingLearningPath.Semester = learningPath.Semester;
                existingLearningPath.AcademicYearStart = learningPath.AcademicYearStart;
                existingLearningPath.IsTemplate = learningPath.IsTemplate;
                existingLearningPath.TemplateKey = learningPath.TemplateKey;
                existingLearningPath.Students = learningPath.Students;
                existingLearningPath.StudentsWithAccess = learningPath.StudentsWithAccess;
                existingLearningPath.Schedule = learningPath.Schedule;

                _context.SaveChanges();
            }
        }

        public void AddStudentToLearningPath(LearningPath learningPath, Student student)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (learningPath.Students == null)
                learningPath.Students = new List<Student>();

            if (!learningPath.Students.Contains(student))
            {
                learningPath.Students.Add(student);
                student.Person.SchoolFees = new SchoolFees();
                student.Person.SchoolFees.Id = GetNextSchoolFeesId();
                student.Person.SchoolFees.TotalAmount = learningPath.FeePerSemester;

                if (student.CurrentLearningPathId == 0)
                {
                    student.CurrentLearningPathId = learningPath.Id;
                    student.CurrentLearningPath = learningPath;
                }

                if (student.Person.SchoolFees.TotalPaid >= learningPath.FeePerSemester * FcmsConstants.PAYMENT_THRESHOLD_FACTOR &&
                    !learningPath.StudentsWithAccess.Contains(student))
                {
                    learningPath.StudentsWithAccess.Add(student);
                }
            }
        }

        public void AddMultipleStudentsToLearningPath(LearningPath learningPath, List<Student> studentsToAdd)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));
            if (studentsToAdd == null || !studentsToAdd.Any())
                throw new ArgumentException("Students list cannot be null or empty.", nameof(studentsToAdd));

            foreach (var student in studentsToAdd)
            {
                if (student == null)
                    continue;

                AddStudentToLearningPath(learningPath, student);
            }
        }
        #endregion

        #region Learning Path Templates
        public void CreateTemplateFromLearningPath(LearningPath learningPath)
        {
            if (learningPath == null) return;

            var school = _context.School.FirstOrDefault();
            if (school == null)
                throw new InvalidOperationException("No school found. Cannot create template without a school.");

            string templateKey = GenerateTemplateKey(learningPath.EducationLevel, learningPath.ClassLevel, learningPath.Semester);

            var existingTemplate = _context.LearningPaths
                .FirstOrDefault(lp => lp.IsTemplate && lp.TemplateKey == templateKey);

            if (existingTemplate != null)
            {
                _context.LearningPaths.Remove(existingTemplate);
            }

            var template = new LearningPath
            {
                SchoolId = school.Id,
                School = school,
                EducationLevel = learningPath.EducationLevel,
                ClassLevel = learningPath.ClassLevel,
                Semester = learningPath.Semester,
                AcademicYearStart = learningPath.AcademicYearStart,
                SemesterStartDate = learningPath.SemesterStartDate,
                SemesterEndDate = learningPath.SemesterEndDate,
                ExamsStartDate = learningPath.ExamsStartDate,
                FeePerSemester = learningPath.FeePerSemester,
                IsTemplate = true,
                TemplateKey = templateKey,
                ApprovalStatus = PrincipalApprovalStatus.Approved,

                Schedule = learningPath.Schedule.Select(s => new ScheduleEntry
                {
                    Title = s.Title,
                    DateTime = s.DateTime,
                    Duration = s.Duration,
                    Venue = s.Venue,
                    ClassSession = s.ClassSession != null ? new ClassSession
                    {
                        Course = s.ClassSession.Course,
                        Topic = s.ClassSession.Topic,
                        Description = s.ClassSession.Description,
                        LessonPlan = s.ClassSession.LessonPlan,
                        Teacher = s.ClassSession.Teacher,
                        StudyMaterials = new List<FileAttachment>(),
                        DiscussionThreads = new List<DiscussionThread>()
                    } : null
                }).ToList(),

                Students = new List<Student>(),
                StudentsWithAccess = new List<Student>()
            };

            _context.LearningPaths.Add(template);
            _context.SaveChanges();
        }

        public string GenerateTemplateKey(EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            return $"{educationLevel}_{classLevel}_{semester}";
        }

        public LearningPath? GetTemplate(EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            string templateKey = GenerateTemplateKey(educationLevel, classLevel, semester);
            return _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                .FirstOrDefault(lp => lp.IsTemplate && lp.TemplateKey == templateKey);
        }

        public bool HasTemplate(EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            return GetTemplate(educationLevel, classLevel, semester) != null;
        }

        public LearningPath? ApplyTemplateToNewLearningPath(LearningPath template, DateTime newAcademicYearStart)
        {
            if (template == null || !template.IsTemplate) return null;

            var templateYearStart = template.AcademicYearStart;
            var dateOffset = newAcademicYearStart - templateYearStart;

            var newLearningPath = new LearningPath
            {
                EducationLevel = template.EducationLevel,
                ClassLevel = template.ClassLevel,
                Semester = template.Semester,
                AcademicYearStart = newAcademicYearStart,
                SemesterStartDate = template.SemesterStartDate.Add(dateOffset),
                SemesterEndDate = template.SemesterEndDate.Add(dateOffset),
                ExamsStartDate = template.ExamsStartDate.Add(dateOffset),
                FeePerSemester = template.FeePerSemester,
                IsTemplate = false,
                TemplateKey = null,
                ApprovalStatus = PrincipalApprovalStatus.Pending,

                Schedule = template.Schedule.Select(s => new ScheduleEntry
                {
                    Title = s.Title,
                    DateTime = s.DateTime.Add(dateOffset),
                    Duration = s.Duration,
                    Venue = s.Venue,
                    ClassSession = s.ClassSession != null ? new ClassSession
                    {
                        Course = s.ClassSession.Course,
                        Topic = s.ClassSession.Topic,
                        Description = s.ClassSession.Description,
                        LessonPlan = s.ClassSession.LessonPlan,
                        Teacher = s.ClassSession.Teacher,
                        StudyMaterials = new List<FileAttachment>(),
                        DiscussionThreads = new List<DiscussionThread>()
                    } : null
                }).ToList(),

                Students = new List<Student>(),
                StudentsWithAccess = new List<Student>()
            };

            return newLearningPath;
        }
        #endregion

        #region Calendar & Scheduling
        public ScheduleEntry? AddScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath == null)
                return null;

            learningPath.Schedule.Add(scheduleEntry);
            UpdateLearningPath(learningPath);
            AddGeneralScheduleEntry(scheduleEntry);

            return scheduleEntry;
        }

        public IEnumerable<ScheduleEntry> GetAllSchoolCalendarSchedules()
        {
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            var allSchedules = new List<ScheduleEntry>();

            if (school?.SchoolCalendar != null)
            {
                foreach (var calendar in school.SchoolCalendar)
                {
                    if (calendar.ScheduleEntries != null)
                    {
                        allSchedules.AddRange(calendar.ScheduleEntries);
                    }
                }
            }

            return allSchedules;
        }

        public bool UpdateScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath == null)
                return false;

            var existingEntry = learningPath.Schedule.FirstOrDefault(s => s.Id == scheduleEntry.Id);
            if (existingEntry == null)
                return false;

            learningPath.Schedule.Remove(existingEntry);
            learningPath.Schedule.Add(scheduleEntry);
            UpdateLearningPath(learningPath);
            UpdateScheduleInSchoolCalendar(scheduleEntry);

            return true;
        }

        public bool DeleteScheduleEntry(int learningPathId, int scheduleEntryId)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath == null)
                return false;

            var entry = learningPath.Schedule.FirstOrDefault(s => s.Id == scheduleEntryId);
            if (entry == null)
                return false;

            learningPath.Schedule.Remove(entry);
            UpdateLearningPath(learningPath);
            RemoveScheduleFromSchoolCalendar(entry);

            return true;
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
                    Id = 1,
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
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            if (school?.SchoolCalendar == null)
                return false;

            foreach (var calendar in school.SchoolCalendar)
            {
                if (calendar.ScheduleEntries != null)
                {
                    var existingIndex = calendar.ScheduleEntries.FindIndex(s => s.Id == scheduleEntry.Id);
                    if (existingIndex >= 0)
                    {
                        calendar.ScheduleEntries[existingIndex] = scheduleEntry;
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DeleteGeneralCalendarScheduleEntry(int scheduleEntryId)
        {
            var school = _context.School
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                .FirstOrDefault();

            if (school?.SchoolCalendar == null)
                return false;

            foreach (var calendar in school.SchoolCalendar)
            {
                if (calendar.ScheduleEntries != null)
                {
                    var scheduleToRemove = calendar.ScheduleEntries.FirstOrDefault(s => s.Id == scheduleEntryId);
                    if (scheduleToRemove != null)
                    {
                        calendar.ScheduleEntries.Remove(scheduleToRemove);
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Class Sessions
        public ClassSession? GetClassSessionById(int classSessionId)
        {
            var learningPaths = _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.StudyMaterials)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.DiscussionThreads)
                            .ThenInclude(dt => dt.Replies)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.HomeworkDetails)
                            .ThenInclude(h => h.Submissions)
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.Teacher)
                            .ThenInclude(t => t.Person)
                .ToList();

            foreach (var learningPath in learningPaths)
            {
                foreach (var schedule in learningPath.Schedule)
                {
                    if (schedule.ClassSession != null && schedule.ClassSession.Id == classSessionId)
                    {
                        return schedule.ClassSession;
                    }
                }
            }
            return null;
        }

        public bool UpdateClassSession(ClassSession classSession)
        {
            try
            {
                var learningPaths = _context.LearningPaths
                    .Include(lp => lp.Schedule)
                        .ThenInclude(s => s.ClassSession)
                    .ToList();

                var found = false;

                foreach (var learningPath in learningPaths)
                {
                    foreach (var schedule in learningPath.Schedule)
                    {
                        if (schedule.ClassSession?.Id == classSession.Id)
                        {
                            schedule.ClassSession = classSession;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (found)
                {
                    _context.SaveChanges();
                }

                return found;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating class session: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Homework
        public Homework? GetHomeworkById(int id)
        {
            var learningPaths = _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.HomeworkDetails)
                            .ThenInclude(h => h.Submissions)
                                .ThenInclude(sub => sub.Student)
                                    .ThenInclude(st => st.Person)
                .ToList();

            foreach (var learningPath in learningPaths)
            {
                foreach (var schedule in learningPath.Schedule)
                {
                    if (schedule.ClassSession?.HomeworkDetails != null)
                    {
                        if (schedule.ClassSession.HomeworkDetails.Id == id)
                            return schedule.ClassSession.HomeworkDetails;
                    }
                }
            }
            return null;
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

        public void UpdateHomework(Homework homework)
        {
            if (homework == null)
                return;

            var learningPaths = _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.HomeworkDetails)
                .ToList();

            foreach (var learningPath in learningPaths)
            {
                foreach (var schedule in learningPath.Schedule)
                {
                    if (schedule.ClassSession?.HomeworkDetails != null &&
                        schedule.ClassSession.HomeworkDetails.Id == homework.Id)
                    {
                        schedule.ClassSession.HomeworkDetails.Title = homework.Title;
                        schedule.ClassSession.HomeworkDetails.AssignedDate = homework.AssignedDate;
                        schedule.ClassSession.HomeworkDetails.DueDate = homework.DueDate;
                        schedule.ClassSession.HomeworkDetails.Question = homework.Question;
                        schedule.ClassSession.HomeworkDetails.Submissions = homework.Submissions;

                        _context.SaveChanges();
                        return;
                    }
                }
            }
        }

        public bool DeleteHomework(int id)
        {
            var learningPaths = _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.HomeworkDetails)
                .ToList();

            foreach (var learningPath in learningPaths)
            {
                foreach (var schedule in learningPath.Schedule)
                {
                    if (schedule.ClassSession?.HomeworkDetails != null &&
                        schedule.ClassSession.HomeworkDetails.Id == id)
                    {
                        schedule.ClassSession.HomeworkDetails = null;
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public HomeworkSubmission? GetHomeworkSubmissionById(int id)
        {
            var learningPaths = _context.LearningPaths
                .Include(lp => lp.Schedule)
                    .ThenInclude(s => s.ClassSession)
                        .ThenInclude(cs => cs.HomeworkDetails)
                            .ThenInclude(h => h.Submissions)
                                .ThenInclude(sub => sub.Student)
                                    .ThenInclude(st => st.Person)
                .ToList();

            foreach (var learningPath in learningPaths)
            {
                foreach (var schedule in learningPath.Schedule)
                {
                    if (schedule.ClassSession?.HomeworkDetails != null)
                    {
                        var submission = schedule.ClassSession.HomeworkDetails.Submissions?.FirstOrDefault(s => s.Id == id);
                        if (submission != null)
                            return submission;
                    }
                }
            }
            return null;
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
        public async Task AddDiscussionThread(DiscussionThread thread, int classSessionId)
        {
            var classSession = GetClassSessionById(classSessionId);
            if (classSession == null)
                throw new ArgumentException("Class session not found.");

            if (classSession.DiscussionThreads == null)
                classSession.DiscussionThreads = new List<DiscussionThread>();

            classSession.DiscussionThreads.Add(thread);
            _context.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task UpdateDiscussionThread(DiscussionThread thread, int classSessionId)
        {
            var classSession = GetClassSessionById(classSessionId);
            if (classSession == null || classSession.DiscussionThreads == null)
                throw new ArgumentException("Class session or discussion threads not found.");

            var existingIndex = classSession.DiscussionThreads.FindIndex(t => t.Id == thread.Id);
            if (existingIndex >= 0)
            {
                classSession.DiscussionThreads[existingIndex] = thread;
            }
            else
            {
                throw new ArgumentException("Discussion thread not found.");
            }
            _context.SaveChanges();
            await Task.CompletedTask;
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

            var targetFolder = Path.Combine(_environment.WebRootPath, folderName);
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

            var publicUrl = $"/{folderName}/{uniqueFileName}";

            var attachment = new FileAttachment
            {
                Id = GetNextAttachmentId(),
                FileName = file.Name,
                FilePath = publicUrl,
                FileSize = file.Size,
                UploadDate = DateTime.Now
            };

            return attachment;
        }

        private int GetNextAttachmentId()
        {
            return _nextAttachmentId++;
        }

        public Task DeleteFileAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            var filePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            foreach (var categoryDict in _attachmentReferences)
            {
                categoryDict.Value.RemoveAll(x => x.attachment.Id == attachment.Id);
            }

            return Task.CompletedTask;
        }

        public Task<List<FileAttachment>> GetAttachmentsAsync(string category, int referenceId)
        {
            if (_attachmentReferences.TryGetValue(category, out var references))
            {
                var attachments = references
                    .Where(x => x.referenceId == referenceId)
                    .Select(x => x.attachment)
                    .ToList();
                return Task.FromResult(attachments);
            }

            return Task.FromResult(new List<FileAttachment>());
        }

        public Task SaveAttachmentReferenceAsync(FileAttachment attachment, string category, int referenceId)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            if (!_attachmentReferences.ContainsKey(category))
            {
                _attachmentReferences[category] = new List<(int, FileAttachment)>();
            }

            _attachmentReferences[category].Add((referenceId, attachment));
            return Task.CompletedTask;
        }
        #endregion

        #region Payments
        public Payment AddPayment(Payment payment)
        {
            var schoolFees = GetSchoolFees(payment.SchoolFeesId);
            if (schoolFees != null)
            {
                payment.Id = GetNextPaymentId();
                schoolFees.Payments.Add(payment);

                var student = GetStudentBySchoolFeesId(payment.SchoolFeesId);
                if (student != null && student.CurrentLearningPath != null)
                {
                    LogicMethods.UpdatePaymentStatus(student, student.CurrentLearningPath);
                }
            }
            return payment;
        }

        public void UpdatePayment(Payment payment)
        {
            var schoolFees = GetSchoolFees(payment.SchoolFeesId);
            if (schoolFees != null)
            {
                var existingPayment = schoolFees.Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (existingPayment != null)
                {
                    existingPayment.Amount = payment.Amount;
                    existingPayment.Date = payment.Date;
                    existingPayment.PaymentMethod = payment.PaymentMethod;
                    existingPayment.Reference = payment.Reference;
                    existingPayment.Semester = payment.Semester;
                    existingPayment.AcademicYearStart = payment.AcademicYearStart;
                    existingPayment.LearningPathId = payment.LearningPathId;

                    var student = GetStudentBySchoolFeesId(payment.SchoolFeesId);
                    if (student != null && student.CurrentLearningPath != null)
                    {
                        LogicMethods.UpdatePaymentStatus(student, student.CurrentLearningPath);
                    }
                }
            }
        }

        public void DeletePayment(int paymentId)
        {
            var school = GetSchool();
            foreach (var student in school.Students)
            {
                if (student.Person.SchoolFees?.Payments != null)
                {
                    var payment = student.Person.SchoolFees.Payments.FirstOrDefault(p => p.Id == paymentId);
                    if (payment != null)
                    {
                        student.Person.SchoolFees.Payments.Remove(payment);

                        if (student.CurrentLearningPath != null)
                        {
                            LogicMethods.UpdatePaymentStatus(student, student.CurrentLearningPath);
                        }
                        break;
                    }
                }
            }
        }

        private int GetNextPaymentId()
        {
            var school = GetSchool();
            int maxId = 0;
            foreach (var student in school.Students)
            {
                if (student.Person.SchoolFees?.Payments != null)
                {
                    foreach (var payment in student.Person.SchoolFees.Payments)
                    {
                        if (payment.Id > maxId)
                            maxId = payment.Id;
                    }
                }
            }
            return maxId + 1;
        }

        public Payment PrepareNewPayment(Student student)
        {
            var payment = new Payment { Date = DateTime.Today };

            if (student.Person.SchoolFees != null)
            {
                payment.SchoolFeesId = student.Person.SchoolFees.Id;
            }

            if (student.CurrentLearningPath != null)
            {
                payment.AcademicYearStart = student.CurrentLearningPath.AcademicYearStart;
                payment.Semester = student.CurrentLearningPath.Semester;
                payment.LearningPathId = student.CurrentLearningPath.Id;
            }

            return payment;
        }

        public SchoolFees? GetSchoolFees(int id)
        {
            var schoolFees = _schoolFees.FirstOrDefault(sf => sf.Id == id);
            if (schoolFees == null)
            {
                var student = GetStudentBySchoolFeesId(id);
                if (student != null)
                {
                    schoolFees = student.Person.SchoolFees;
                }
            }
            return schoolFees;
        }

        public int GetNextSchoolFeesId()
        {
            var existingIds = new List<int>();

            foreach (var student in GetStudents())
            {
                if (student.Person != null &&
                    student.Person.SchoolFees != null &&
                    student.Person.SchoolFees.Id > 0)
                {
                    existingIds.Add(student.Person.SchoolFees.Id);
                }
            }

            if (_schoolFees != null)
            {
                foreach (var sf in _schoolFees)
                {
                    existingIds.Add(sf.Id);
                }
            }

            return existingIds.Count > 0 ? existingIds.Max() + 1 : 1;
        }

        public Student? GetStudentBySchoolFeesId(int schoolFeesId)
        {
            var school = GetSchool();
            return school.Students.FirstOrDefault(s => s.Person.SchoolFees?.Id == schoolFeesId);
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

            UpdateCourseGradeConfigurations(learningPath.Id, configuration.Course, configuration);
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

        public List<GradesReport> GetGradesReports(string academicYear, string semester)
        {
            var school = _context.School
                .Include(s => s.Students)
                    .ThenInclude(st => st.CourseGrades)
                        .ThenInclude(cg => cg.TestGrades)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.CourseGradingConfigurations)
                .FirstOrDefault();

            return LogicMethods.GetGradesReports(school, academicYear, semester);
        }
        #endregion

        #region Curriculum
        public List<Curriculum> GetFullCurriculum()
        {
            var learningPaths = GetAllLearningPaths().ToList();
            return LogicMethods.GenerateCurriculumFromLearningPaths(learningPaths);
        }

        public List<Curriculum> FilterCurriculum(
                                 List<Curriculum> curriculum,
                                 EducationLevel educationLevel,
                                 ClassLevel classLevel,
                                 Semester? semester = null
                             )
        {

            var filteredCurricula = curriculum
                .Where(c => c.EducationLevel == educationLevel && c.ClassLevel == classLevel)
                .Select(c => new Curriculum
                {
                    AcademicYear = c.AcademicYear,
                    EducationLevel = c.EducationLevel,
                    ClassLevel = c.ClassLevel,
                    Semesters = semester == null
                        ? c.Semesters
                        : c.Semesters
                            .Where(s => s.Semester == semester)
                            .Select(s => new SemesterCurriculum
                            {
                                Semester = s.Semester,
                                ClassSessions = s.ClassSessions
                            }).ToList()
                })
                .ToList();

            return filteredCurricula;
        }
        #endregion

        #region Attendance
        // Save attendance for a learning path
        public DailyAttendanceLogEntry SaveAttendance(int learningPathId, List<int> presentStudentIds, int teacherId, DateTime? attendanceDate = null)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath == null)
                throw new ArgumentException($"Learning path with ID {learningPathId} not found.");

            var teacher = GetStaffById(teacherId);
            if (teacher == null)
                throw new ArgumentException($"Teacher with ID {teacherId} not found.");

            var presentStudents = learningPath.Students
                .Where(s => presentStudentIds.Contains(s.Id))
                .ToList();

            return LogicMethods.TakeAttendanceForLearningPath(learningPath, presentStudents, teacher, attendanceDate);
        }

        // Check if attendance has been taken for a specific date
        public bool HasAttendanceBeenTaken(int learningPathId, DateTime date)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath?.AttendanceLog == null)
                return false;

            return learningPath.AttendanceLog
                .Any(log => log.TimeStamp.Date == date.Date);
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
                    .ThenInclude(lp => lp.StudentsWithAccess)
                .FirstOrDefault();

            if (school == null) return;

            foreach (var learningPath in school.LearningPaths)
            {
                var studentInLearningPath = learningPath.Students.FirstOrDefault(s => s.Id == student.Id);
                if (studentInLearningPath != null)
                {
                    learningPath.Students.Remove(studentInLearningPath);
                }

                var studentWithAccess = learningPath.StudentsWithAccess.FirstOrDefault(s => s.Id == student.Id);
                if (studentWithAccess != null)
                {
                    learningPath.StudentsWithAccess.Remove(studentWithAccess);
                }
            }

            var schoolStudent = school.Students.FirstOrDefault(s => s.Id == student.Id);
            if (schoolStudent != null)
            {
                school.Students.Remove(schoolStudent);
            }

            student.Person.IsArchived = true;
            student.ArchivedDate = DateTime.Now;
            _archivedStudents.Add(student);
            _context.Students.Update(student);
            _context.SaveChanges();
            // TODO: 
            // 1. Create an Archive database table
            // 2. Move student data to archive table
            // 4. Send graduation notification
            // 5. Update school statistics
        }

        public List<Student> GetArchivedStudents()
        {
            // Adapt to DbContext: Query for archived students from database
            return _context.Students
                .Include(s => s.Person)
                .Include(s => s.CourseGrades)
                .Where(s => s.Person.IsArchived == true)
                .OrderByDescending(s => s.ArchivedDate)
                .ToList();
        }
        #endregion  
    }
}

using FcmsPortal;
using FcmsPortal.Constants;
using FcmsPortal.Enums;
using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;


namespace FcmsPortalUI.Services
{
    public class SchoolDataService : ISchoolDataService
    {
        private readonly FcmsPortalUIContext _context;
        private readonly IWebHostEnvironment _environment;
        private List<Student> _archivedStudents = new List<Student>();


        public SchoolDataService(FcmsPortalUIContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        #region School
        public School GetSchool()
        {
            var school = _context.School
                .Include(s => s.Staff)
                    .ThenInclude(st => st.Person)
                .Include(s => s.Students)
                    .ThenInclude(st => st.Person)
                        .ThenInclude(p => p.SchoolFees)
                            .ThenInclude(sf => sf.Payments)
                .Include(s => s.Students)
                    .ThenInclude(st => st.Guardian)
                        .ThenInclude(g => g.Person)
                .Include(s => s.Students)
                    .ThenInclude(st => st.LearningPath)
                .Include(s => s.Guardians)
                    .ThenInclude(g => g.Person)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.Students)
                        .ThenInclude(st => st.Person)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.Schedule)
                        .ThenInclude(se => se.ClassSession)
                            .ThenInclude(cs => cs.Teacher)
                                .ThenInclude(t => t.Person)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.Schedule)
                        .ThenInclude(se => se.ClassSession)
                            .ThenInclude(cs => cs.StudyMaterials)
                .Include(s => s.LearningPaths)
                    .ThenInclude(lp => lp.Schedule)
                        .ThenInclude(se => se.ClassSession)
                            .ThenInclude(cs => cs.DiscussionThreads)
                .Include(s => s.SchoolCalendar)
                    .ThenInclude(c => c.ScheduleEntries)
                        .ThenInclude(se => se.ClassSession)
                            .ThenInclude(cs => cs.Teacher)
                                .ThenInclude(t => t.Person)
                .FirstOrDefault();

            return school;
        }

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
            _context.SaveChanges();
            return school;
        }

        public async Task UpdateSchoolAsync(School updatedSchool)
        {
            var existingSchool = await _context.School
                .FirstOrDefaultAsync();

            if (existingSchool != null)
            {
                existingSchool.Name = updatedSchool.Name;
                existingSchool.LogoUrl = updatedSchool.LogoUrl;
                existingSchool.Email = updatedSchool.Email;
                existingSchool.PhoneNumber = updatedSchool.PhoneNumber;
                existingSchool.WebsiteUrl = updatedSchool.WebsiteUrl;
                existingSchool.Address = updatedSchool.Address;
                await _context.SaveChangesAsync();
            }
        }

        public bool HasPrincipal()
        {
            int? principalRoleId = _context.Roles
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

        public Guardian? GetGuardianById(int id)
        {
            return _context.Guardians
                .Include(g => g.Person)
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
                    .ThenInclude(p => p.SchoolFees)
                        .ThenInclude(sf => sf.Payments)
                .Include(s => s.CourseGrades)
                .Include(s => s.LearningPath)
                .Include(s => s.Guardian)
                    .ThenInclude(g => g.Person)
                .FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Student> GetStudents()
        {
            return _context.Students
                .Include(s => s.Person)
                    .ThenInclude(p => p.SchoolFees)
                        .ThenInclude(sf => sf.Payments)
                .Include(s => s.CourseGrades)
                .Include(s => s.LearningPath)
                .Include(s => s.Guardian)
                    .ThenInclude(g => g.Person)
                .ToList();
        }

        public void UpdateStudent(Student student)
        {
            var existingStudent = _context.Students
                .Include(s => s.Person)
                    .ThenInclude(p => p.SchoolFees)
                        .ThenInclude(sf => sf.Payments)
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
                existingStudent.Person.SchoolFees = student.Person.SchoolFees;
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

        public bool DeleteStudent(int studentId)
        {
            var student = _context.Students
                .Include(s => s.Person)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                return false;
            }

            var person = student.Person;

            _context.Students.Remove(student);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            _context.SaveChanges();
            return true;
        }

        public void ActivateStudent(Student student)
        {
            if (student?.Person == null)
                throw new ArgumentNullException(nameof(student), "Student and Person cannot be null.");

            student.Person.IsActive = true;
            _context.SaveChanges();
        }

        public void DeActivateStudent(Student student)
        {
            if (student?.Person == null)
                throw new ArgumentNullException(nameof(student), "Student and Person cannot be null.");

            student.Person.IsActive = false;
            _context.SaveChanges();
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

        public void SetStudentSchoolFees(Student student, double feeAmount)
        {
            if (student?.Person == null)
                throw new ArgumentNullException(nameof(student), "Student and Person cannot be null.");

            if (student.Person.SchoolFees == null)
            {
                student.Person.SchoolFees = new SchoolFees
                {
                    PersonId = student.Person.Id,
                    TotalAmount = feeAmount,
                    Payments = new List<Payment>()
                };
                _context.SchoolFees.Add(student.Person.SchoolFees);
            }
            else
            {
                student.Person.SchoolFees.TotalAmount = feeAmount;
            }
            _context.SaveChanges();

            if (feeAmount > 0)
            {
                ActivateStudent(student);
            }
            else
            {
                DeActivateStudent(student);
            }
        }

        public void RemoveStudentFromLearningPath(LearningPath learningPath, Student student)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                .Include(lp => lp.StudentsWithAccess)
                .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (!existingLearningPath.Students.Contains(student))
                return;

            existingLearningPath.Students.Remove(student);
            existingLearningPath.StudentsWithAccess.Remove(student);

            SetStudentSchoolFees(student, 0);
            student.Person.IsActive = false;
            UpdateStudent(student);

            _context.SaveChanges();
        }

        public LearningPath? GetLearningPathById(int id)
        {
            return _context.LearningPaths
            .Include(lp => lp.Students)
                .ThenInclude(s => s.Person)
            .Include(lp => lp.Students)
                .ThenInclude(s => s.CourseGrades)
                    .ThenInclude(cg => cg.TestGrades)
                        .ThenInclude(tg => tg.Teacher)
            .Include(lp => lp.Students)
                .ThenInclude(s => s.CourseGrades)
                    .ThenInclude(cg => cg.GradingConfiguration)
            .Include(lp => lp.CourseGradingConfigurations)
            .Include(lp => lp.StudentsWithAccess)
                .ThenInclude(s => s.Person)
            .Include(lp => lp.AttendanceLog)
                .ThenInclude(al => al.Teacher)
            .Include(lp => lp.AttendanceLog)
                .ThenInclude(al => al.PresentStudents)
            .Include(lp => lp.AttendanceLog)
                .ThenInclude(al => al.AbsentStudents)
            .FirstOrDefault(lp => lp.Id == id);
        }

        public LearningPath? GetCurrentActiveLearningPath()
        {
            return _context.LearningPaths
                .AsNoTracking()
                .Where(lp => lp.ApprovalStatus == PrincipalApprovalStatus.Pending)
                .OrderByDescending(lp => lp.SemesterStartDate)
                .FirstOrDefault();
        }

        public IEnumerable<LearningPath> GetAllLearningPaths()
        {
            return _context.LearningPaths
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                        .ThenInclude(p => p.SchoolFees)
                            .ThenInclude(sf => sf.Payments)
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
            var scheduleEntry = _context.ScheduleEntries
                .Include(se => se.LearningPath)
                    .ThenInclude(lp => lp.Students)
                        .ThenInclude(s => s.Person)
                .FirstOrDefault(se => se.ClassSessionId == classSessionId);

            return scheduleEntry?.LearningPath;
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

            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                .Include(lp => lp.StudentsWithAccess)
                .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (existingLearningPath.Students == null)
                existingLearningPath.Students = new List<Student>();

            if (!existingLearningPath.Students.Any(s => s.Id == student.Id))
            {
                existingLearningPath.Students.Add(student);

                SetStudentSchoolFees(student, existingLearningPath.FeePerSemester);

                if (student.LearningPathId == 0)
                {
                    student.LearningPathId = existingLearningPath.Id;
                    student.LearningPath = existingLearningPath;
                }

                if (student.Person.SchoolFees.TotalPaid >= existingLearningPath.FeePerSemester * FcmsConstants.PAYMENT_THRESHOLD_FACTOR &&
                    !existingLearningPath.StudentsWithAccess.Any(s => s.Id == student.Id))
                {
                    existingLearningPath.StudentsWithAccess.Add(student);
                }
                _context.SaveChanges();
            }
        }

        public void AddMultipleStudentsToLearningPath(LearningPath learningPath, List<Student> studentsToAdd)
        {
            if (learningPath == null)
                throw new ArgumentNullException(nameof(learningPath));
            if (studentsToAdd == null || !studentsToAdd.Any())
                throw new ArgumentException("Students list cannot be null or empty.", nameof(studentsToAdd));

            var existingLearningPath = _context.LearningPaths
                .Include(lp => lp.Students)
                .Include(lp => lp.StudentsWithAccess)
                .FirstOrDefault(lp => lp.Id == learningPath.Id);

            if (existingLearningPath == null)
                throw new ArgumentException("Learning path not found in database.");

            if (existingLearningPath.Students == null)
                existingLearningPath.Students = new List<Student>();

            bool hasChanges = false;

            foreach (var student in studentsToAdd)
            {
                if (student == null)
                    continue;

                if (!existingLearningPath.Students.Any(s => s.Id == student.Id))
                {
                    existingLearningPath.Students.Add(student);

                    SetStudentSchoolFees(student, existingLearningPath.FeePerSemester);
                    if (student.LearningPathId == 0)
                    {
                        student.LearningPathId = existingLearningPath.Id;
                        student.LearningPath = existingLearningPath;
                    }

                    if (student.Person.SchoolFees.TotalPaid >= existingLearningPath.FeePerSemester * FcmsConstants.PAYMENT_THRESHOLD_FACTOR &&
                        !existingLearningPath.StudentsWithAccess.Any(s => s.Id == student.Id))
                    {
                        existingLearningPath.StudentsWithAccess.Add(student);
                    }

                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                _context.SaveChanges();
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
                ApprovalStatus = PrincipalApprovalStatus.Pending,

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
        public ScheduleEntry? GetLatestEventOrMeetingForDate(DateTime date)
        {
            var targetDate = date.Date;

            return _context.ScheduleEntries
                .AsNoTracking()
                .Where(e => e.DateTime.Date == targetDate &&
                           (e.Event != null || e.Meeting != null))
                .OrderByDescending(e => e.DateTime)
                .FirstOrDefault();
        }

        public ScheduleEntry? AddScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry)
        {
            var learningPath = GetLearningPathById(learningPathId);
            if (learningPath == null)
                return null;

            scheduleEntry.LearningPathId = learningPathId;

            _context.ScheduleEntries.Add(scheduleEntry);
            _context.SaveChanges();

            AddGeneralScheduleEntry(scheduleEntry);
            return scheduleEntry;
        }

        public IEnumerable<ScheduleEntry> GetAllSchoolCalendarSchedules()
        {
            return _context.ScheduleEntries
                .Include(se => se.ClassSession)
                    .ThenInclude(cs => cs.Teacher)
                        .ThenInclude(t => t.Person)
                .OrderBy(se => se.DateTime)
                .ToList();
        }


        public bool UpdateScheduleEntry(int learningPathId, ScheduleEntry scheduleEntry)
        {
            var existing = _context.ScheduleEntries
                .FirstOrDefault(se => se.Id == scheduleEntry.Id && se.LearningPathId == learningPathId);

            if (existing == null)
                return false;

            existing.DateTime = scheduleEntry.DateTime;
            existing.Duration = scheduleEntry.Duration;
            existing.Venue = scheduleEntry.Venue;
            existing.Title = scheduleEntry.Title;
            existing.Event = scheduleEntry.Event;
            existing.Meeting = scheduleEntry.Meeting;
            existing.IsRecurring = scheduleEntry.IsRecurring;
            existing.RecurrencePattern = scheduleEntry.RecurrencePattern;
            existing.RecurrenceInterval = scheduleEntry.RecurrenceInterval;
            existing.EndDate = scheduleEntry.EndDate;
            existing.DaysOfWeek = scheduleEntry.DaysOfWeek;
            existing.ClassSessionId = scheduleEntry.ClassSessionId;

            _context.SaveChanges();
            UpdateScheduleInSchoolCalendar(existing);

            return true;
        }

        public bool DeleteScheduleEntry(int learningPathId, int scheduleEntryId)
        {
            var entry = _context.ScheduleEntries
                .Include(se => se.ClassSession)
                .FirstOrDefault(se => se.Id == scheduleEntryId && se.LearningPathId == learningPathId);

            if (entry == null)
                return false;

            _context.ScheduleEntries.Remove(entry);
            _context.SaveChanges();
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
            return _context.ClassSessions
                .Include(cs => cs.Teacher)
                    .ThenInclude(t => t.Person)
                .Include(cs => cs.StudyMaterials)
                .Include(cs => cs.HomeworkDetails)
                    .ThenInclude(h => h.Submissions)
                        .ThenInclude(s => s.Student)
                            .ThenInclude(st => st.Person)
                .Include(cs => cs.DiscussionThreads)
                    .ThenInclude(dt => dt.FirstPost)
                        .ThenInclude(fp => fp.Author)
                .Include(cs => cs.DiscussionThreads)
                    .ThenInclude(dt => dt.Replies)
                        .ThenInclude(r => r.Author)
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
                FileName = file.Name,
                FilePath = publicUrl,
                FileSize = file.Size,
                UploadDate = DateTime.Now
            };

            _context.FileAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task DeleteFileAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            var filePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/'));
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
                .Include(sf => sf.Payments)
                .FirstOrDefault(sf => sf.Id == payment.SchoolFeesId);

            if (schoolFees != null)
            {
                schoolFees.Payments.Add(payment);
                _context.SaveChanges();

                var student = GetStudentBySchoolFeesId(payment.SchoolFeesId);
                if (student != null && student.LearningPath != null)
                {
                    LogicMethods.UpdatePaymentStatus(student, student.LearningPath);
                }
            }
            return payment;
        }

        public void UpdatePayment(Payment payment)
        {
            var existingPayment = _context.Payments.FirstOrDefault(p => p.Id == payment.Id);
            if (existingPayment != null)
            {
                existingPayment.Amount = payment.Amount;
                existingPayment.Date = payment.Date;
                existingPayment.PaymentMethod = payment.PaymentMethod;
                existingPayment.Reference = payment.Reference;
                existingPayment.Semester = payment.Semester;
                existingPayment.AcademicYearStart = payment.AcademicYearStart;
                existingPayment.LearningPathId = payment.LearningPathId;

                _context.SaveChanges();

                var student = GetStudentBySchoolFeesId(payment.SchoolFeesId);
                if (student != null && student.LearningPath != null)
                {
                    LogicMethods.UpdatePaymentStatus(student, student.LearningPath);
                }
            }
        }

        public void DeletePayment(int paymentId)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment != null)
            {
                var schoolFeesId = payment.SchoolFeesId;
                _context.Payments.Remove(payment);
                _context.SaveChanges();

                var student = GetStudentBySchoolFeesId(schoolFeesId);
                if (student != null && student.LearningPath != null)
                {
                    LogicMethods.UpdatePaymentStatus(student, student.LearningPath);
                }
            }
        }

        public Payment PrepareNewPayment(Student student)
        {
            var payment = new Payment { Date = DateTime.Today };

            if (student.Person.SchoolFees != null)
            {
                payment.SchoolFeesId = student.Person.SchoolFees.Id;
            }

            if (student.LearningPath != null)
            {
                payment.AcademicYearStart = student.LearningPath.AcademicYearStart;
                payment.Semester = student.LearningPath.Semester;
                payment.LearningPathId = student.LearningPath.Id;
            }

            return payment;
        }

        public SchoolFees? GetSchoolFees(int id)
        {
            var schoolFees = _context.SchoolFees
                .Include(sf => sf.Payments)
                .FirstOrDefault(sf => sf.Id == id);

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

        public Student? GetStudentBySchoolFeesId(int schoolFeesId)
        {
            return _context.Students
                .Include(s => s.Person)
                    .ThenInclude(p => p.SchoolFees)
                        .ThenInclude(sf => sf.Payments)
                .Include(s => s.LearningPath)
                .FirstOrDefault(s => s.Person.SchoolFees != null && s.Person.SchoolFees.Id == schoolFeesId);
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

        public void SaveTestGrade(TestGrade testGrade)
        {
            if (testGrade == null) return;

            if (testGrade.Id == 0)
            {
                _context.TestGrades.Add(testGrade);
            }
            else
            {
                _context.TestGrades.Update(testGrade);
            }
            _context.SaveChanges();
        }

        public void AddTestGrade(int studentId, string course, double score, GradeType gradeType,
                 int teacherId, string teacherRemark, int learningPathId)
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
                Date = DateTime.Now,
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

        public async Task<TestGrade> AddHomeworkSubmissionGradeAsync(
                int studentId,
                string course,
                double score,
                int teacherId,
                string teacherRemark,
                int learningPathId)
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
                Date = DateTime.Now,
                TeacherRemark = teacherRemark,
                CourseGradeId = courseGrade.Id
            };

            _context.TestGrades.Add(testGrade);

            if (courseGrade.GradingConfiguration != null)
            {
                LogicMethods.RecalculateCourseGrade(courseGrade);
                _context.CourseGrades.Update(courseGrade);
            }

            await _context.SaveChangesAsync();

            return testGrade;
        }

        public int GetGradeCountByType(int learningPathId, string course, GradeType gradeType)
        {
            return _context.TestGrades
                .Where(tg => tg.CourseGrade.Course == course &&
                            tg.CourseGrade.LearningPathId == learningPathId &&
                            tg.GradeType == gradeType)
                .GroupBy(tg => new { tg.Date.Date, tg.Date.Hour, tg.Date.Minute, tg.Date.Second })
                .Count();
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
        public DailyAttendanceLogEntry SaveAttendance(int learningPathId, List<int> presentStudentIds, int teacherId, DateTime? attendanceDate = null)
        {
            var learningPath = GetLearningPathById(learningPathId);
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

            if (learningPath.AttendanceLog == null)
                learningPath.AttendanceLog = new List<DailyAttendanceLogEntry>();

            learningPath.AttendanceLog.Add(attendanceEntry);

            _context.SaveChanges();

            return attendanceEntry;
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
                .Include(al => al.PresentStudents)
                .Include(al => al.AbsentStudents)
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

            var student = _context.Students
                .Include(s => s.ReportCards)
                .FirstOrDefault(s => s.Id == reportCard.StudentId);

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

                _context.StudentReportCards.Update(existingReportCard);

                if (student != null && !student.ReportCards.Any(rc => rc.Id == existingReportCard.Id))
                {
                    student.ReportCards.Add(existingReportCard);
                }

                _context.SaveChanges();
                return existingReportCard;
            }
            else
            {
                _context.StudentReportCards.Add(reportCard);

                if (student != null)
                {
                    student.ReportCards.Add(reportCard);
                }

                _context.SaveChanges();
                return reportCard;
            }
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
            foreach (var student in learningPath.Students)
            {
                var paymentReport = LogicMethods.GenerateStudentPaymentReportEntry(student);

                var archivedPayment = new ArchivedStudentPayment
                {
                    StudentId = student.Id,
                    StudentName = Util.GetFullName(student.Person),
                    LearningPathId = learningPath.Id,
                    LearningPathName = Util.GetLearningPathName(learningPath),
                    EducationLevel = learningPath.EducationLevel,
                    ClassLevel = learningPath.ClassLevel,
                    Semester = learningPath.Semester,
                    AcademicYear = learningPath.AcademicYear,
                    TotalFees = paymentReport.TotalFees,
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
                        Reference = paymentDetail.Reference.ToString()
                    };

                    _context.ArchivedPaymentDetails.Add(archivedDetail);
                }
            }

            _context.SaveChanges();
        }

        public List<ArchivedStudentPayment> GetArchivedStudentPayments(
            string academicYear,
            EducationLevel educationLevel,
            ClassLevel classLevel,
            Semester semester)
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

        public List<AttendanceArchive> GetArchivedStudentAttendance(
            string academicYear,
            EducationLevel educationLevel,
            ClassLevel classLevel,
            Semester semester)
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

        public List<string> GetArchivedGradesAcademicYears()
        {
            return _context.StudentReportCards
                .AsNoTracking()
                .Include(rc => rc.LearningPath)
                .Where(rc => rc.LearningPath != null && rc.IsFinalized)
                .Select(rc => rc.LearningPath.AcademicYearStart.ToString())
                .Distinct()
                .OrderByDescending(year => year)
                .ToList();
        }

        public LearningPath? GetLearningPathByFilter(string academicYear, EducationLevel educationLevel, ClassLevel classLevel, Semester semester)
        {
            if (string.IsNullOrEmpty(academicYear))
                return null;

            // Extract start year from format "2024-2025"
            var yearParts = academicYear.Split('-');
            if (yearParts.Length != 2 || !int.TryParse(yearParts[0], out int startYear))
                return null;

            return _context.LearningPaths
                .AsNoTracking()
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.Person)
                .Include(lp => lp.Students)
                    .ThenInclude(s => s.CourseGrades)
                .Include(lp => lp.ReportCards)
                .FirstOrDefault(lp => lp.AcademicYearStart.Year == startYear &&
                                     lp.EducationLevel == educationLevel &&
                                     lp.ClassLevel == classLevel &&
                                     lp.Semester == semester);
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
        #endregion

        #region Announcements
        public List<Announcement> GetAllAnnouncements()
        {
            return _context.Announcements
                .Include(a => a.PostedBy)
                .OrderByDescending(a => a.PostedAt)
                .ToList();
        }

        public List<Announcement> GetActiveAnnouncements()
        {
            var today = DateTime.Today;
            return _context.Announcements
                .AsNoTracking()
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .OrderByDescending(a => a.PostedAt)
                .ToList();
        }

        public Announcement CreateAnnouncement(Announcement announcement, int userId)
        {
            announcement.PostedAt = DateTime.Now;
            announcement.PostedById = userId;

            _context.Announcements.Add(announcement);
            _context.SaveChanges();

            return announcement;
        }

        public Announcement UpdateAnnouncement(Announcement announcement)
        {
            var existing = _context.Announcements.Find(announcement.Id);
            if (existing == null)
                throw new InvalidOperationException("Announcement not found.");

            existing.Message = announcement.Message;
            existing.StartDate = announcement.StartDate;
            existing.EndDate = announcement.EndDate;

            _context.SaveChanges();
            return existing;
        }

        public void DeleteAnnouncement(int announcementId)
        {
            var announcement = _context.Announcements.Find(announcementId);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                _context.SaveChanges();
            }
        }
        #endregion

        #region Quotes
        public List<Quote> GetAllQuotes()
        {
            return _context.Quotes
                .Include(q => q.AddedBy)
                .OrderByDescending(q => q.DateAdded)
                .ToList();
        }

        public Quote CreateQuote(Quote quote, int userId)
        {
            quote.DateAdded = DateTime.Now;
            quote.AddedById = userId;

            _context.Quotes.Add(quote);
            _context.SaveChanges();

            return quote;
        }

        public Quote UpdateQuote(Quote quote)
        {
            var existing = _context.Quotes.Find(quote.Id);
            if (existing == null)
                throw new InvalidOperationException("Quote not found.");

            existing.Text = quote.Text;
            existing.Author = quote.Author;

            _context.SaveChanges();
            return existing;
        }

        public void DeleteQuote(int quoteId)
        {
            var quote = _context.Quotes.Find(quoteId);
            if (quote != null)
            {
                _context.Quotes.Remove(quote);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}

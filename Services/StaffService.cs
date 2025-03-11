using FcmsPortal.Enums;
using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class StaffService
    {
        private readonly List<Staff> _staffList = new();

        public StaffService()
        {
            // Create admin staff
            Staff staff1 = new Staff
            {
                Id = 1,
                Person = new Person
                {
                    FirstName = "Mr. Fin",
                    MiddleName = "F",
                    LastName = "Fen",
                    Email = "fin@fcms.com",
                    PhoneNumber = "08012345678",
                },
                JobRole = JobRole.Admin,
                JobDescription = "Principal"
            };

            // Create Biology teacher
            Staff staff2 = new Staff
            {
                Id = 2,
                Person = new Person
                {
                    FirstName = "Mr Eric",
                    MiddleName = "E",
                    LastName = "Een",
                    EducationLevel = EducationLevel.SeniorCollege,
                    ClassLevel = ClassLevel.SC_3
                },
                JobRole = JobRole.Teacher,
                JobDescription = "Biology Teacher",
                AreaOfSpecialization = CourseDefaults.GetCourseNames(EducationLevel.SeniorCollege)[3]
            };

            // Create Geography teacher
            Staff staff3 = new Staff
            {
                Id = 3,
                Person = new Person
                {
                    FirstName = "Mrs Qin",
                    MiddleName = "Q",
                    LastName = "Que",
                    EducationLevel = EducationLevel.SeniorCollege,
                    ClassLevel = ClassLevel.SC_3
                },
                JobRole = JobRole.Teacher,
                JobDescription = "Geography Teacher",
                AreaOfSpecialization = CourseDefaults.GetCourseNames(EducationLevel.SeniorCollege)[12]
            };

            _staffList.Add(staff1);
            _staffList.Add(staff2);
            _staffList.Add(staff3);
        }

        private int _nextId = 1;

        public async Task<List<Staff>> GetStaffAsync()
        {
            await Task.Delay(100);
            return _staffList;
        }

        public Task<Staff?> GetStaffByIdAsync(int id)
        {
            var staff = _staffList.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(staff);
        }

        public async Task<Staff> AddStaffAsync(Staff staff)
        {
            if (staff == null)
                throw new ArgumentNullException(nameof(staff));

            staff.Person ??= new Person();

            await Task.Delay(100);

            staff.Id = _nextId++;
            staff.Person.Id = _nextId;

            _staffList.Add(staff);
            return staff;
        }

        public Task<bool> UpdateStaffAsync(Staff staff)
        {
            var existingStaff = _staffList.FirstOrDefault(s => s.Id == staff.Id);
            if (existingStaff == null)
            {
                return Task.FromResult(false);
            }

            var index = _staffList.IndexOf(existingStaff);
            _staffList[index] = staff;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteStaffAsync(int id)
        {
            var staff = _staffList.FirstOrDefault(s => s.Id == id);
            if (staff == null)
            {
                return Task.FromResult(false);
            }

            _staffList.Remove(staff);
            return Task.FromResult(true);
        }
    }
}
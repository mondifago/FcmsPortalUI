using FcmsPortal.Enums;
using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class StaffService
    {
        private readonly List<Staff> _staffList = new();
        private int _nextId = 4;

        public StaffService()
        {
            _staffList.AddRange(new[]
            {
                new Staff
                {
                    Id = 1,
                    Person = new Person
                    {
                        FirstName = "Mr. Fin",
                        MiddleName = "F",
                        LastName = "Fen",
                        Email = "fin@fcms.com",
                        PhoneNumber = "08012345678"
                    },
                    JobRole = JobRole.Admin,
                    JobDescription = "Principal"
                },
                new Staff
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
                },
                new Staff
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
                }
            });
        }

        public async Task<List<Staff>> GetStaffAsync()
        {
            await Task.Delay(100);
            return _staffList;
        }

        public Task<Staff?> GetStaffByIdAsync(int id)
        {
            return Task.FromResult(_staffList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<Staff> AddStaffAsync(Staff staff)
        {
            if (staff == null)
                throw new ArgumentNullException(nameof(staff));

            staff.Person ??= new Person();

            await Task.Delay(100);

            staff.Id = _nextId++;
            staff.Person.Id = staff.Id;

            _staffList.Add(staff);
            return staff;
        }

        public async Task<bool> UpdateStaffAsync(Staff staff)
        {
            if (staff == null)
                return false;

            await Task.Delay(100);

            var existingStaff = _staffList.FirstOrDefault(s => s.Id == staff.Id);
            if (existingStaff == null)
                return false;

            var index = _staffList.IndexOf(existingStaff);
            _staffList[index] = staff;
            return true;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            await Task.Delay(100);

            var staff = _staffList.FirstOrDefault(s => s.Id == id);
            if (staff == null)
                return false;

            _staffList.Remove(staff);
            return true;
        }
    }
}

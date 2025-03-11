using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class StaffService
    {
        private List<Staff> _staffList = new();
        private int _nextId = 1;

        public async Task<List<Staff>> GetGuardiansAsync()
        {
            await Task.Delay(100);
            return _staffList;
        }

        public Task<List<Staff>> GetAllStaffAsync()
        {
            return Task.FromResult(_staffList);
        }

        public Task<Staff?> GetStaffByIdAsync(int id)
        {
            var staff = _staffList.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(staff);
        }

        public Task<Staff> CreateStaffAsync(Staff staff)
        {
            staff.Id = _nextId++;
            staff.Person.Id = _nextId;

            _staffList.Add(staff);
            return Task.FromResult(staff);
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
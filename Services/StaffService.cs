﻿using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class StaffService
    {
        private readonly List<Staff> _staffList = new();
        private int _nextId;

        public StaffService()
        {

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

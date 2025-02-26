using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class GuardianService
    {
        // In a real application, this would interact with a database
        private static List<Guardian> _guardians = new List<Guardian>();
        private static int _nextId = 1;

        public async Task<List<Guardian>> GetGuardiansAsync()
        {
            // Simulate async database call
            await Task.Delay(100);
            return _guardians;
        }

        public async Task<Guardian> GetGuardianByIdAsync(int id)
        {
            await Task.Delay(100);
            return _guardians.FirstOrDefault(g => g.Id == id);
        }

        public async Task AddGuardianAsync(Guardian guardian)
        {
            await Task.Delay(100);

            // Set IDs
            guardian.Id = _nextId++;
            guardian.Person.Id = _nextId++;

            // Set IDs for addresses
            if (guardian.Person.Addresses != null)
            {
                foreach (var address in guardian.Person.Addresses)
                {
                    address.Id = _nextId++;
                }
            }

            _guardians.Add(guardian);
        }

        public async Task UpdateGuardianAsync(Guardian guardian)
        {
            await Task.Delay(100);

            var existingGuardian = _guardians.FirstOrDefault(g => g.Id == guardian.Id);
            if (existingGuardian != null)
            {
                int index = _guardians.IndexOf(existingGuardian);
                _guardians[index] = guardian;

                // Ensure new addresses have IDs
                if (guardian.Person.Addresses != null)
                {
                    foreach (var address in guardian.Person.Addresses.Where(a => a.Id == 0))
                    {
                        address.Id = _nextId++;
                    }
                }
            }
        }

        public async Task DeleteGuardianAsync(int id)
        {
            await Task.Delay(100);

            var guardian = _guardians.FirstOrDefault(g => g.Id == id);
            if (guardian != null)
            {
                _guardians.Remove(guardian);
            }
        }
    }
}
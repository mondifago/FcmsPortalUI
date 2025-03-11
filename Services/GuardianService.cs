using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public class GuardianService
    {
        private readonly List<Guardian> _guardians = new();
        private int _nextId = 1;

        public async Task<List<Guardian>> GetGuardiansAsync()
        {
            await Task.Delay(100);
            return _guardians;
        }

        public async Task<Guardian?> GetGuardianByIdAsync(int id)
        {
            await Task.Delay(100);
            return _guardians.FirstOrDefault(g => g.Id == id);
        }

        public async Task AddGuardianAsync(Guardian guardian)
        {
            if (guardian == null)
                throw new ArgumentNullException(nameof(guardian));

            await Task.Delay(100);

            guardian.Id = _nextId;
            guardian.Person ??= new Person();
            guardian.Person.Id = _nextId;
            _nextId++;

            if (guardian.Person.Addresses != null)
            {
                foreach (var address in guardian.Person.Addresses)
                {
                    if (address.Id == 0)
                        address.Id = _nextId++;
                }
            }

            _guardians.Add(guardian);
        }

        public async Task UpdateGuardianAsync(Guardian guardian)
        {
            if (guardian == null)
                throw new ArgumentNullException(nameof(guardian));

            await Task.Delay(100);

            var existingGuardian = _guardians.FirstOrDefault(g => g.Id == guardian.Id);
            if (existingGuardian != null)
            {
                int index = _guardians.IndexOf(existingGuardian);
                _guardians[index] = guardian;

                if (guardian.Person?.Addresses != null)
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

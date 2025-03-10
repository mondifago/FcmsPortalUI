using FcmsPortal.Enums;
using FcmsPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace FcmsPortalUI.Services
{
    public class GuardianService
    {
        private readonly List<Guardian> _guardians = new();
        private int _nextGuardianId = 1;
        private int _nextAddressId = 1;
        private readonly object _lock = new();

        public GuardianService()
        {
            // Seed two guardian samples
            var guardian1 = new Guardian
            {
                Person = new Person
                {
                    ProfilePictureUrl = "https://example.com/photo1.jpg",
                    FirstName = "Ugochukwu",
                    MiddleName = "Nwaolisa",
                    LastName = "Maduakor",
                    Sex = Gender.Male,
                    StateOfOrigin = "Anambra",
                    LgaOfOrigin = "Ihiala",
                    Email = "mondifago1@yahoo.com",
                    PhoneNumber = "08034567345",
                    EmergencyContact = "15 Sterling Close, Asaba",
                    Addresses = new List<Address>
                    {
                        new Address { Id = _nextAddressId++, Street = "123 Sky Street", City = "Lagos", State = "Lagos", Country = "Nigeria", AddressType = AddressType.Home }
                    },
                    IsActive = true
                },
                RelationshipToStudent = Relationship.Father,
                Occupation = "Engineer"
            };
            var guardian2 = new Guardian
            {
                Person = new Person
                {
                    ProfilePictureUrl = null,
                    FirstName = "Chinedu",
                    MiddleName = "Okeke",
                    LastName = "Ibe",
                    Sex = Gender.Male,
                    StateOfOrigin = "Imo",
                    LgaOfOrigin = "Owerri",
                    Email = "chinedu.ibe@gmail.com",
                    PhoneNumber = "08123456789",
                    EmergencyContact = "10 Lagos Road, Abuja",
                    Addresses = new List<Address>
                    {
                        new Address { Id = _nextAddressId++, Street = "456 Ocean Road", City = "Abuja", State = "FCT", Country = "Nigeria", AddressType = AddressType.HomeTown }
                    },
                    IsActive = true
                },
                RelationshipToStudent = Relationship.Mother,
                Occupation = "Teacher"
            };

            AddGuardian(guardian1);
            AddGuardian(guardian2);
        }

        public List<Guardian> GetGuardians()
        {
            lock (_lock)
            {
                return _guardians.ToList();
            }
        }

        public Guardian GetGuardianById(int id)
        {
            lock (_lock)
            {
                return _guardians.FirstOrDefault(g => g.Id == id) ?? throw new KeyNotFoundException($"Guardian with ID {id} not found.");
            }
        }

        public void AddGuardian(Guardian guardian)
        {
            var validationResults = new List<ValidationResult>();
            if (!ValidateGuardian(guardian, out validationResults))
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }

            lock (_lock)
            {
                guardian.Id = _nextGuardianId++;
                if (guardian.Person.Addresses != null)
                {
                    foreach (var address in guardian.Person.Addresses.Where(a => a.Id == 0))
                    {
                        address.Id = _nextAddressId++;
                    }
                }
                _guardians.Add(guardian);
            }
        }

        public void UpdateGuardian(Guardian guardian)
        {
            var validationResults = new List<ValidationResult>();
            if (!ValidateGuardian(guardian, out validationResults))
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }

            lock (_lock)
            {
                int index = _guardians.FindIndex(g => g.Id == guardian.Id);
                if (index == -1)
                    throw new KeyNotFoundException($"Guardian with ID {guardian.Id} not found.");

                if (guardian.Person.Addresses != null)
                {
                    foreach (var address in guardian.Person.Addresses.Where(a => a.Id == 0))
                    {
                        address.Id = _nextAddressId++;
                    }
                }
                _guardians[index] = guardian;
            }
        }

        public void DeleteGuardian(int id)
        {
            lock (_lock)
            {
                int removedCount = _guardians.RemoveAll(g => g.Id == id);
                if (removedCount == 0)
                    throw new KeyNotFoundException($"Guardian with ID {id} not found.");
            }
        }

        public bool ValidateGuardian(Guardian guardian, out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            var context = new ValidationContext(guardian);
            if (!Validator.TryValidateObject(guardian, context, validationResults, true))
                return false;

            context = new ValidationContext(guardian.Person);
            if (!Validator.TryValidateObject(guardian.Person, context, validationResults, true))
                return false;

            foreach (var address in guardian.Person.Addresses)
            {
                context = new ValidationContext(address);
                if (!Validator.TryValidateObject(address, context, validationResults, true))
                    return false;
            }

            return true;
        }
    }
}
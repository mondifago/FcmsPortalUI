using FcmsPortal.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FcmsPortalUI.Services
{
    public class ValidationService
    {
        public bool ValidateGuardian(EditContext context, Guardian guardian, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, guardian.Person, messageStore);

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        public bool ValidateStudent(EditContext context, Student student, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, student.Person, messageStore);

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        public bool ValidateStaff(EditContext context, Staff staff, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, staff.Person, messageStore);

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        private bool ValidatePerson(EditContext context, Person person, ValidationMessageStore messageStore)
        {
            bool isValid = true;

            void Require(string propertyName, string value, string error)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    var field = new FieldIdentifier(person, propertyName);
                    messageStore.Add(field, error);
                    isValid = false;
                }
            }

            Require(nameof(person.FirstName), person.FirstName, "First name is required.");
            Require(nameof(person.LastName), person.LastName, "Last name is required.");
            Require(nameof(person.Email), person.Email, "Email is required.");
            Require(nameof(person.PhoneNumber), person.PhoneNumber, "Phone number is required.");
            Require(nameof(person.StateOfOrigin), person.StateOfOrigin, "State of origin is required.");
            Require(nameof(person.LgaOfOrigin), person.LgaOfOrigin, "LGA of origin is required.");

            if (person.Addresses == null || person.Addresses.Count == 0)
            {
                var field = new FieldIdentifier(person, nameof(person.Addresses));
                messageStore.Add(field, "At least one address is required.");
                isValid = false;
            }

            return isValid;
        }
    }
}
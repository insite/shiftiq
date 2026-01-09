using System;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class TPersonFieldStore
    {
        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public static void Update(Guid organization, Guid user, string fieldName, string[] values)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TPersonFields
                    .Where(x => x.OrganizationIdentifier == organization && x.UserIdentifier == user && x.FieldName == fieldName)
                    .OrderBy(x => x.FieldSequence)
                    .ToList();

                if (values == null || values.Length == 0)
                {
                    db.TPersonFields.RemoveRange(existing);
                    db.SaveChanges();
                }

                for (int i = existing.Count - 1; i >= 0; i--)
                {
                    var item = existing[i];
                    if (!values.All(x => string.Equals(item.FieldName, x, StringComparison.OrdinalIgnoreCase)))
                    {
                        db.TPersonFields.Remove(item);
                        existing.RemoveAt(i);
                    }
                }

                var sequence = existing.Count + 1;

                foreach (var value in values)
                {
                    if (!existing.Any(x => string.Equals(x.FieldName, value, StringComparison.OrdinalIgnoreCase)))
                    {
                        db.TPersonFields.Add(new TPersonField
                        {
                            FieldIdentifier = UniqueIdentifier.Create(),
                            OrganizationIdentifier = organization,
                            UserIdentifier = user,
                            FieldName = fieldName,
                            FieldValue = value,
                            FieldSequence = sequence++
                        });
                    }
                }

                for (int i = 0; i < existing.Count; i++)
                    existing[i].FieldSequence = i + 1;

                db.SaveChanges();
            }
        }

        public static void SetHideSkillsCheckLearnerBanner(Guid organizationId, Guid userId, bool hide)
        {
            const string ValueTrue = "true";
            const string ValueFalse = "false";

            SetValue(organizationId, userId,
                TPersonFieldName.HideSkillsCheckLearnerBanner,
                hide ? ValueTrue : ValueFalse, ValueFalse,
                StringComparison.OrdinalIgnoreCase);
        }

        public static void SetSkillsCheckPurchasedCount(Guid organizationId, Guid userId, int value)
        {
            if (value < 0)
                return;

            SetValue(organizationId, userId,
                TPersonFieldName.SkillsCheckPurchasedCount,
                value.ToString(), 0.ToString());
        }

        private static void SetValue(
            Guid organizationId, Guid userId,
            string fieldName, string value, string defaultValue,
            StringComparison comparison = StringComparison.Ordinal)
        {
            var isDefaultValue = string.Equals(value, defaultValue, comparison);

            using (var db = new InternalDbContext())
            {
                var existing = db.TPersonFields
                    .Where(x => x.OrganizationIdentifier == organizationId && x.UserIdentifier == userId && x.FieldName == fieldName)
                    .FirstOrDefault();

                if (existing == null)
                {
                    if (isDefaultValue)
                        return;

                    db.TPersonFields.Add(new TPersonField
                    {
                        FieldIdentifier = UniqueIdentifier.Create(),
                        OrganizationIdentifier = organizationId,
                        UserIdentifier = userId,
                        FieldName = fieldName,
                        FieldValue = value
                    });
                }
                else
                {
                    if (string.Equals(existing.FieldValue, value, comparison))
                        return;

                    if (isDefaultValue)
                        db.TPersonFields.Remove(existing);
                    else
                        existing.FieldValue = value;
                }

                db.SaveChanges();
            }
        }

        internal static void Delete(Guid organization, Guid user, string fieldName)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TPersonFields.SingleOrDefault(x =>
                        x.OrganizationIdentifier == organization &&
                        x.UserIdentifier == user &&
                        x.FieldName == fieldName);

                if (entity == null)
                    return;

                db.TPersonFields.Remove(entity);

                db.SaveChanges();
            }
        }

        internal static void Save(Guid organization, Guid user, string field, string value)
        {
            if (SaveUserField(organization, user, field, value))
                return;

            var person = GetPerson(user, organization);
            if (person == null)
                return;

            if (field.StartsWith("HomeAddress.", StringComparison.OrdinalIgnoreCase))
            {
                SaveHomeAddressField(person.PersonIdentifier, person.HomeAddressIdentifier, field, value);
                return;
            }

            if (SavePersonField(person.PersonIdentifier, field, value))
                return;

            if (field.Length > 100)
                field = field.Substring(0, 100);

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand("contacts.SavePersonField @OrganizationIdentifier, @UserIdentifier, @FieldName, @FieldValue",
                    new SqlParameter("@OrganizationIdentifier", organization),
                    new SqlParameter("@UserIdentifier", user),
                    new SqlParameter("@FieldName", field),
                    new SqlParameter("@FieldValue", value));
            }
        }

        private static QPerson GetPerson(Guid userId, Guid organizationId)
        {
            using (var db = new InternalDbContext())
                return db.QPersons.Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId).FirstOrDefault();
        }

        private static bool SaveUserField(Guid organization, Guid user, string field, string value)
        {
            if (!Enum.TryParse<UserField>(field, out var userField))
                return false;

            int maxLength = 100;

            switch (userField)
            {
                case UserField.Email:
                    if (UserSearch.Exists(new UserFilter { EmailExact = value }))
                        return true;
                    break;
                case UserField.EmailAlternate:
                    break;
                case UserField.Honorific:
                case UserField.Initials:
                    maxLength = 32;
                    break;
                case UserField.PhoneMobile:
                    maxLength = 38;
                    break;
                case UserField.FirstName:
                case UserField.LastName:
                case UserField.MiddleName:
                    ChangeName(organization, user, userField, value);
                    return true;
                default:
                    return false;
            }

            value = value.MaxLength(maxLength);

            _commander.Send(new ModifyUserFieldText(user, userField, value));

            return true;
        }

        /// <summary>
        /// TODO: Do we need to update full name here?
        /// </summary>
        private static void ChangeName(Guid organizationId, Guid userId, UserField field, string value)
        {
            var fullNamePolicy = OrganizationSearch.GetPersonFullNamePolicy(organizationId);

            value = field == UserField.MiddleName
                ? value.MaxLength(38)
                : value.MaxLength(40);

            QUser user;

            using (var db = new InternalDbContext())
            {
                user = db.QUsers
                    .Where(x => x.UserIdentifier == userId)
                    .Single();
            }

            switch (field)
            {
                case UserField.FirstName:
                    user.FirstName = value;
                    break;
                case UserField.LastName:
                    user.LastName = value;
                    break;
                case UserField.MiddleName:
                    user.MiddleName = value;
                    break;
                default:
                    throw new ArgumentException($"Invalid field: ${field}");
            }

            _commander.Send(new ModifyUserName(userId, user.FirstName, user.LastName, user.MiddleName, fullNamePolicy));
        }

        private static bool SavePersonField(Guid personId, string field, string value)
        {
            if (!Enum.TryParse<PersonField>(field, out var personField))
                return false;

            return SavePersonFieldDate(personId, personField, value)
                || SavePersonFieldBool(personId, personField, value)
                || SavePersonFieldText(personId, personField, value);
        }

        private static bool SavePersonFieldDate(Guid personId, PersonField personField, string value)
        {
            switch (personField)
            {
                case PersonField.Birthdate:
                case PersonField.ImmigrationLandingDate:
                    break;
                default:
                    return false;
            }

            if (DateTime.TryParse(value, out var d))
                _commander.Send(new ModifyPersonFieldDate(personId, personField, d));

            return true;
        }

        private static bool SavePersonFieldDateTimeOffset(Guid personId, PersonField personField, string value)
        {
            switch (personField)
            {
                case PersonField.Created:
                    break;
                default:
                    return false;
            }

            if (DateTimeOffset.TryParse(value, out var d))
                _commander.Send(new ModifyPersonFieldDateOffset(personId, personField, d));

            return true;
        }

        private static bool SavePersonFieldBool(Guid personId, PersonField personField, string value)
        {
            switch (personField)
            {
                case PersonField.CandidateIsActivelySeeking:
                case PersonField.CandidateIsWillingToRelocate:
                    break;
                default:
                    return false;
            }

            if (bool.TryParse(value, out var b))
                _commander.Send(new ModifyPersonFieldBool(personId, personField, b));

            return true;
        }

        private static bool SavePersonFieldText(Guid personId, PersonField personField, string value)
        {
            int maxLength = 100;

            switch (personField)
            {
                case PersonField.CandidateLinkedInUrl:
                case PersonField.CandidateOccupationList:
                case PersonField.Citizenship:
                case PersonField.CredentialingCountry:
                case PersonField.EmergencyContactName:
                case PersonField.ImmigrationDestination:
                case PersonField.JobDivision:
                case PersonField.JobTitle:
                case PersonField.Referrer:
                case PersonField.ReferrerOther:
                case PersonField.WebSiteUrl:
                    maxLength = 100;
                    break;
                case PersonField.ConsentConsultation:
                case PersonField.ConsentToShare:
                case PersonField.ImmigrationCategory:
                case PersonField.Phone:
                case PersonField.PhoneFax:
                case PersonField.PhoneHome:
                case PersonField.PhoneOther:
                case PersonField.PhoneWork:
                    maxLength = 30;
                    break;
                case PersonField.EducationLevel:
                    maxLength = 80;
                    break;
                case PersonField.EmergencyContactPhone:
                case PersonField.EmployeeUnion:
                    maxLength = 32;
                    break;
                case PersonField.EmergencyContactRelationship:
                case PersonField.Region:
                    maxLength = 50;
                    break;
                case PersonField.AgeGroup:
                case PersonField.EmployeeType:
                case PersonField.FirstLanguage:
                case PersonField.Gender:
                case PersonField.ImmigrationApplicant:
                case PersonField.ImmigrationDisability:
                case PersonField.PersonCode:
                case PersonField.PersonType:
                case PersonField.ShippingPreference:
                case PersonField.TradeworkerNumber:
                    maxLength = 20;
                    break;
                case PersonField.ImmigrationNumber:
                    maxLength = 64;
                    break;
                case PersonField.Language:
                    maxLength = 2;
                    break;
                default:
                    return false;
            }

            value = value.MaxLength(maxLength);

            _commander.Send(new ModifyPersonFieldText(personId, personField, value));

            return true;
        }

        private static void SaveHomeAddressField(Guid personId, Guid? addressId, string field, string value)
        {
            PersonAddress address;

            if (addressId.HasValue)
            {
                using (var db = new InternalDbContext())
                    address = db.QPersonAddresses
                        .Where(x => x.AddressIdentifier == addressId)
                        .Select(x => new PersonAddress
                        {
                            Identifier = x.AddressIdentifier,
                            City = x.City,
                            Country = x.Country,
                            Description = x.Description,
                            PostalCode = x.PostalCode,
                            Province = x.Province,
                            Street1 = x.Street1,
                            Street2 = x.Street2
                        })
                        .FirstOrDefault();
            }
            else
                address = null;

            if (address == null)
                address = new PersonAddress();

            if (string.Equals(field, "HomeAddress.City", StringComparison.OrdinalIgnoreCase))
                address.City = GetEntityValue(value, 128);
            else if (string.Equals(field, "HomeAddress.Country", StringComparison.OrdinalIgnoreCase))
                address.Country = GetEntityValue(value, 32);
            else if (string.Equals(field, "HomeAddress.Province", StringComparison.OrdinalIgnoreCase))
                address.Province = GetEntityValue(value, 64);
            else if (string.Equals(field, "HomeAddress.PostalCode", StringComparison.OrdinalIgnoreCase))
                address.PostalCode = GetEntityValue(value, 20);
            else if (string.Equals(field, "HomeAddress.Street1", StringComparison.OrdinalIgnoreCase))
                address.Street1 = GetEntityValue(value, 200);
            else if (string.Equals(field, "HomeAddress.Street2", StringComparison.OrdinalIgnoreCase))
                address.Street2 = GetEntityValue(value, 200);

            _commander.Send(new ModifyPersonAddress(personId, AddressType.Home, address));

            string GetEntityValue(string inputValue, int maxLength)
            {
                return inputValue?
                    .Replace("\t", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim()
                    .NullIfEmpty()?
                    .MaxLength(maxLength);
            }
        }
    }
}
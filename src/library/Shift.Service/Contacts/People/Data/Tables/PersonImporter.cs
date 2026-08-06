using InSite.Application.Memberships.Write;
using InSite.Application.People.Write;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Microsoft.Extensions.Logging;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.Service;
using Shift.Service.Security;

using ModifyPersonAddress = InSite.Application.People.Write.ModifyPersonAddress;
using CreateUser = InSite.Application.Users.Write.CreateUser;
using CreatePerson = InSite.Application.People.Write.CreatePerson;

namespace Shift.Service.Directory;

public class PersonImporter(
    GroupReader groupReader,
    PersonReader personReader,
    UserReader userReader,
    MembershipReader membershipReader,
    PendingPersonReader pendingPersonReader,
    PendingPersonWriter pendingPersonWriter,
    ICommanderAsync commander,
    ILogger<PersonImporter> logger
) : IPersonImporter
{
    #region Import

    public async Task<ImportPersonResult[]> ImportAsync(
        Guid organizationId,
        string? fullNamePolicy,
        string timeZone,
        Guid submittedBy,
        string submittedByName,
        IEnumerable<ImportPerson> imports
        )
    {
        var result = new List<ImportPersonResult>();

        foreach (var import in imports)
            result.Add(await ImportAsync(organizationId, fullNamePolicy, timeZone, submittedBy, submittedByName, import));

        return result.ToArray();
    }

    private async Task<ImportPersonResult> ImportAsync(
        Guid organizationId,
        string? fullNamePolicy,
        string timeZone,
        Guid submittedBy,
        string submittedByName,
        ImportPerson input
        )
    {
        var failure = new ValidationFailure();

        var groupId = await GetGroupIdAsync(organizationId, input.GroupCode, failure);
        if (failure.Errors.Count > 0)
            return new ImportPersonResult(input) { Failure = failure, Status = ImportPersonResult.StatusEnum.Error };

        var existing = await GetPersonByCodeAsync(organizationId, input);
        if (existing != null)
        {
            var original = await ModifyAsync(existing, groupId, fullNamePolicy, submittedByName, input, failure);
            return failure.Errors.Count > 0
                ? new ImportPersonResult(input) { Failure = failure, Status = ImportPersonResult.StatusEnum.Error }
                : original != null
                    ? new ImportPersonResult(input) { UserId = existing.UserIdentifier, Status = ImportPersonResult.StatusEnum.Modified, Original = original }
                    : new ImportPersonResult(input) { UserId = existing.UserIdentifier, Status = ImportPersonResult.StatusEnum.NotChanged };
        }

        if (await HasPersonByNameAsync(organizationId, input))
        {
            var pendingPersonId = await InsertPendingPersonAsync(organizationId, groupId, submittedBy, input, failure);
            return failure.Errors.Count > 0
                ? new ImportPersonResult(input) { Failure = failure, Status = ImportPersonResult.StatusEnum.Error }
                : new ImportPersonResult(input) { PendingPersonId = pendingPersonId, Status = ImportPersonResult.StatusEnum.Pending };
        }

        var userId = await CreateAsync(organizationId, groupId, fullNamePolicy, timeZone, submittedByName, input, failure);
        return failure.Errors.Count > 0
            ? new ImportPersonResult(input) { Failure = failure, Status = ImportPersonResult.StatusEnum.Error }
            : new ImportPersonResult(input) { UserId = userId, Status = ImportPersonResult.StatusEnum.Created };
    }

    #endregion

    #region Import PendingPerson

    public async Task<ImportPersonResult[]> ImportPendingPeopleAsync(
        Guid organizationId,
        string? fullNamePolicy,
        string timeZone,
        string submittedByName,
        IEnumerable<ImportPendingPerson> imports
        )
    {
        var pendingPeople = await GetPendingPeopleAsync(organizationId);
        var result = new List<ImportPersonResult>();

        foreach (var import in imports)
        {
            var pendingPerson = pendingPeople.Find(x => x.PendingPersonIdentifier == import.PendingPersonId);
            if (pendingPerson == null)
                continue;

            var importPerson = CreateImportPerson(pendingPerson);

            if (import.Action == ImportPendingPerson.ActionEnum.Ignore)
            {
                result.Add(new ImportPersonResult(importPerson) { PendingPersonId = import.PendingPersonId, Status = ImportPersonResult.StatusEnum.Pending });
                continue;
            }

            PersonEntity? matchPerson;

            if (import.Action == ImportPendingPerson.ActionEnum.Match)
            {
                if (import.MatchUserId == null)
                    continue;

                var people = await personReader.CollectAsync(new CollectPeople { OrganizationId = organizationId, UserId = import.MatchUserId });
                if (people.Count == 0)
                    continue;

                matchPerson = people[0];
            }
            else
                matchPerson = null;

            result.Add(await ImportPendingPersonAsync(organizationId, fullNamePolicy, timeZone, submittedByName, import.PendingPersonId, pendingPerson.GroupIdentifier, importPerson, matchPerson));
        }

        return result.ToArray();
    }

    private async Task<ImportPersonResult> ImportPendingPersonAsync(
        Guid organizationId,
        string? fullNamePolicy,
        string timeZone,
        string submittedByName,
        Guid pendingPersonId,
        Guid? groupId,
        ImportPerson input,
        PersonEntity? matchPerson
        )
    {
        var failure = new ValidationFailure();

        ImportPersonResult result;

        if (matchPerson != null)
        {
            var original = await ModifyAsync(matchPerson, groupId, fullNamePolicy, submittedByName, input, failure);
            result = failure.Errors.Count > 0
                ? new ImportPersonResult(input) { PendingPersonId = pendingPersonId, Failure = failure, Status = ImportPersonResult.StatusEnum.Error }
                : original != null
                    ? new ImportPersonResult(input) { PendingPersonId = pendingPersonId, UserId = matchPerson.UserIdentifier, Status = ImportPersonResult.StatusEnum.Modified, Original = original }
                    : new ImportPersonResult(input) { PendingPersonId = pendingPersonId, UserId = matchPerson.UserIdentifier, Status = ImportPersonResult.StatusEnum.NotChanged };            
        }
        else
        {
            var userId = await CreateAsync(organizationId, groupId, fullNamePolicy, timeZone, submittedByName, input, failure);
            result = failure.Errors.Count > 0
                ? new ImportPersonResult(input) { PendingPersonId = pendingPersonId, Failure = failure, Status = ImportPersonResult.StatusEnum.Error }
                : new ImportPersonResult(input) { PendingPersonId = pendingPersonId, UserId = userId, Status = ImportPersonResult.StatusEnum.Created };
        }

        if (result.Status != ImportPersonResult.StatusEnum.Error)
            await pendingPersonWriter.DeleteAsync(pendingPersonId);

        return result;
    }

    private async Task<List<PendingPersonEntity>> GetPendingPeopleAsync(Guid organizationId)
    {
        var criteria = new CollectPendingPeople { OrganizationId = organizationId };
        criteria.DisablePaging();

        return await pendingPersonReader.CollectAsync(criteria);
    }

    private static ImportPerson CreateImportPerson(PendingPersonEntity entity)
    {
        return new ImportPerson
        {
            PersonCode = entity.PersonCode,
            UserEmail = entity.UserEmail,
            UserFirstName = entity.UserFirstName,
            UserLastName = entity.UserLastName,
            UserMiddleName = entity.UserMiddleName,
            JobDivision = entity.JobDivision,
            JobTitle = entity.JobTitle,
            WorkAddressStreet1 = entity.WorkAddressStreet1,
            WorkAddressStreet2 = entity.WorkAddressStreet2,
            WorkAddressCity = entity.WorkAddressCity,
            WorkAddressProvince = entity.WorkAddressProvince,
            WorkAddressPostalCode = entity.WorkAddressPostalCode,
            EmployeeStatus = entity.EmployeeStatus.ToEnum<ImportPerson.EmployeeStatusEnum>()
        };
    }

    #endregion

    #region Shared methods

    private async Task<Guid?> InsertPendingPersonAsync(Guid organizationId, Guid? groupId, Guid submittedBy, ImportPerson input, ValidationFailure failure)
    {
        var entity = new PendingPersonEntity
        {
            PendingPersonIdentifier = UniqueIdentifier.Create(),
            OrganizationIdentifier = organizationId,
            GroupIdentifier = groupId,
            SubmittedBy = submittedBy,

            PersonCode = input.PersonCode,
            UserEmail = input.UserEmail,
            UserFirstName = input.UserFirstName,
            UserLastName = input.UserLastName,
            UserMiddleName = input.UserMiddleName.NullIfEmpty(),
            JobDivision = input.JobDivision.NullIfEmpty(),
            JobTitle = input.JobTitle.NullIfEmpty(),
            WorkAddressStreet1 = input.WorkAddressStreet1.NullIfEmpty(),
            WorkAddressStreet2 = input.WorkAddressStreet2.NullIfEmpty(),
            WorkAddressCity = input.WorkAddressCity.NullIfEmpty(),
            WorkAddressProvince = input.WorkAddressProvince.NullIfEmpty(),
            WorkAddressPostalCode = input.WorkAddressPostalCode.NullIfEmpty(),
            EmployeeStatus = input.EmployeeStatus.ToString(),

            SubmittedAt = DateTimeOffset.UtcNow
        };

        if (await pendingPersonWriter.CreateAsync(entity))
            return entity.PendingPersonIdentifier;

        failure.AddError("Unexpected error while creating pending person");

        return null;
    }

    private async Task<PersonEntity?> GetPersonByCodeAsync(Guid organizationId, ImportPerson input)
    {
        var people = await personReader.CollectAsync(new CollectPeople
        {
            OrganizationId = organizationId,
            PersonCode = input.PersonCode
        });

        return people.Count > 0 ? people[0] : null;
    }

    private async Task<bool> HasPersonByNameAsync(Guid organizationId, ImportPerson input)
    {
        return await personReader.ExistsAsync(new CountPeople
        {
            OrganizationId = organizationId,
            FirstNameExact = input.UserFirstName,
            LastNameExact = input.UserLastName
        });
    }

    private async Task<ImportPerson?> ModifyAsync(
        PersonEntity person,
        Guid? groupId,
        string? fullNamePolicy,
        string submittedByName,
        ImportPerson input,
        ValidationFailure failure
        )
    {
        var user = await userReader.RetrieveAsync(person.UserIdentifier) ?? throw new ArgumentNullException("user");

        if (!string.Equals(user.Email, input.UserEmail, StringComparison.OrdinalIgnoreCase)
            && await userReader.ExistsAsync(new CountUsers { UserEmailExact = input.UserEmail })
        )
        {
            failure.AddError($"The user with email '{input.UserEmail}' is already registered");
            return null;
        }

        var commands = new List<ICommand>();

        AddModifyCommands(user, person, fullNamePolicy, input, commands);
        await AddMembershipCommandsAsync(person.UserIdentifier, groupId, commands);
        await ApplyEmployeeStatusAsync(person.OrganizationIdentifier, person.UserIdentifier, person.PersonIdentifier, person, submittedByName, input.EmployeeStatus, commands);

        if (commands.Count == 0)
            return null;

        try
        {
            await commander.SendCommandsAsync(commands);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed while modifying the person");
            failure.AddError("Unexpected error");
        }

        return GetOriginal(person, user);
    }

    private static ImportPerson GetOriginal(PersonEntity person, UserEntity user)
    {
        var address = person.WorkAddress;

        return new ImportPerson
        {
            PersonCode = person.PersonCode,
            UserEmail = user.Email,
            UserFirstName = user.FirstName,
            UserLastName = user.LastName,
            UserMiddleName = user.MiddleName,
            JobDivision = person.JobDivision,
            JobTitle = person.JobTitle,
            WorkAddressStreet1 = address?.Street1,
            WorkAddressStreet2 = address?.Street2,
            WorkAddressCity = address?.City,
            WorkAddressProvince = address?.Province,
            WorkAddressPostalCode = address?.PostalCode
        };
    }

    private static void AddModifyCommands(UserEntity user, PersonEntity person, string? fullNamePolicy, ImportPerson input, List<ICommand> commands)
    {
        var userId = user.UserIdentifier;
        var personId = person.PersonIdentifier;

        if (!string.Equals(user.Email, input.UserEmail))
            commands.Add(new ModifyUserFieldText(userId, UserField.Email, input.UserEmail));

        if (!string.Equals(user.FirstName, input.UserFirstName)
            || !string.Equals(user.LastName, input.UserLastName)
            || !string.Equals(user.MiddleName ?? "", input.UserMiddleName ?? "")
        )
        {
            commands.Add(new ModifyUserName(userId, input.UserFirstName, input.UserLastName, input.UserMiddleName, fullNamePolicy));
        }

        if (!string.Equals(person.JobDivision, input.JobDivision))
            commands.Add(new ModifyPersonFieldText(personId, PersonField.JobDivision, input.JobDivision));

        if (!string.Equals(person.JobTitle, input.JobTitle))
            commands.Add(new ModifyPersonFieldText(personId, PersonField.JobTitle, input.JobTitle));

        var oldAddress = person.WorkAddress ?? new AddressEntity();
        var newAddress = CreatePersonAddress(input);

        if (!string.Equals(oldAddress.Country, newAddress.Country)
            || !string.Equals(oldAddress.Street1 ?? "", newAddress.Street1 ?? "")
            || !string.Equals(oldAddress.Street2 ?? "", newAddress.Street2 ?? "")
            || !string.Equals(oldAddress.City ?? "", newAddress.City ?? "")
            || !string.Equals(oldAddress.Province ?? "", newAddress.Province ?? "")
            || !string.Equals(oldAddress.PostalCode ?? "", newAddress.PostalCode ?? "")
        )
        {
            commands.Add(new ModifyPersonAddress(personId, AddressType.Work, newAddress));
        }
    }

    private async Task AddMembershipCommandsAsync(Guid userId, Guid? groupId, List<ICommand> commands)
    {
        if (groupId == null)
            return;

        if (await membershipReader.ExistsAsync(new CountMemberships { UserId = userId, GroupId = groupId }))
            return;

        var membershipId = await membershipReader.RetrieveDeletedMembershipIdAsync(userId, groupId.Value);
        
        if (membershipId != null)
            commands.Add(new ResumeMembership(membershipId.Value, userId, groupId.Value, null, DateTimeOffset.UtcNow));
        else
            commands.Add(new StartMembership(UniqueIdentifier.Create(), userId, groupId.Value, null, DateTimeOffset.UtcNow));
    }

    private async Task<Guid?> CreateAsync(
        Guid organizationId,
        Guid? groupId,
        string? fullNamePolicy,
        string timeZone,
        string submittedByName,
        ImportPerson input,
        ValidationFailure failure
        )
    {
        if (await userReader.ExistsAsync(new CountUsers { UserEmailExact = input.UserEmail }))
        {
            failure.AddError($"The user with email '{input.UserEmail}' is already registered");
            return null;
        }

        var userId = UniqueIdentifier.Create();
        var personId = UniqueIdentifier.Create();

        var commands = new List<ICommand>
        {
            new CreateUser(userId, input.UserEmail, input.UserFirstName, input.UserLastName, input.UserMiddleName, fullNamePolicy, timeZone, false),
            new CreatePerson(personId, userId, organizationId),
            new ModifyPersonFieldText(personId, PersonField.PersonCode, input.PersonCode),
            new ModifyPersonFieldText(personId, PersonField.JobDivision, input.JobDivision),
            new ModifyPersonFieldText(personId, PersonField.JobTitle, input.JobTitle),
            new ModifyPersonAddress(personId, AddressType.Work, CreatePersonAddress(input)),
        };

        if (groupId.HasValue)
            commands.Add(new StartMembership(UniqueIdentifier.Create(), userId, groupId.Value, null, DateTimeOffset.UtcNow));

        await ApplyEmployeeStatusAsync(organizationId, userId, personId, null, submittedByName, input.EmployeeStatus, commands);

        try
        {
            await commander.SendCommandsAsync(commands);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed while creating the person");
            failure.AddError("Unexpected error");
            return null;
        }

        return userId;
    }

    private async Task ApplyEmployeeStatusAsync(
        Guid organizationId,
        Guid userId,
        Guid personId,
        PersonEntity? person,
        string submittedByName,
        ImportPerson.EmployeeStatusEnum employeeStatus,
        List<ICommand> commands
    )
    {
        switch (employeeStatus)
        {
            case ImportPerson.EmployeeStatusEnum.Active:
                if (person == null || !person.EmailEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailEnabled, true));
                if (person == null || person.EmailAlternateEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailAlternateEnabled, false));
                return;
            case ImportPerson.EmployeeStatusEnum.LaidOff:
                if (person == null || person.EmailEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailEnabled, false));
                if (person == null || !person.EmailAlternateEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailAlternateEnabled, true));
                return;
            case ImportPerson.EmployeeStatusEnum.Leave:
                return;

            case ImportPerson.EmployeeStatusEnum.Deceased:
            case ImportPerson.EmployeeStatusEnum.Retired:
                if (person == null || person.EmailEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailEnabled, false));
                if (person == null || person.EmailAlternateEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailAlternateEnabled, false));
                break;
            case ImportPerson.EmployeeStatusEnum.Terminated:
                if (person == null || person.EmailEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailEnabled, false));
                if (person == null || !person.EmailAlternateEnabled)
                    commands.Add(new ModifyPersonFieldBool(personId, PersonField.EmailAlternateEnabled, true));
                break;

            default:
                throw new ArgumentException($"Unknown EmployeeStatus: {employeeStatus}");
        }

        if (person == null || person.UserAccessGranted.HasValue)
            commands.Add(new RevokePersonAccess(personId, DateTimeOffset.UtcNow, submittedByName));

        if (person != null)
        {
            var memberships = await membershipReader.CollectAsync(new CollectMemberships { OrganizationId = organizationId, UserId = userId }, null);
            foreach (var m in memberships)
                commands.Add(new EndMembership(m.MembershipIdentifier));
        }
    }

    private static PersonAddress CreatePersonAddress(ImportPerson input)
    {
        return new PersonAddress
        {
            Country = "Canada",
            Street1 = input.WorkAddressStreet1,
            Street2 = input.WorkAddressStreet2,
            City = input.WorkAddressCity,
            Province = input.WorkAddressProvince,
            PostalCode = input.WorkAddressPostalCode,
        };
    }

    private async Task<Guid?> GetGroupIdAsync(Guid organizationId, string groupCode, ValidationFailure failure)
    {
        if (string.IsNullOrEmpty(groupCode))
            return null;

        var groups = await groupReader.CollectAsync(new CollectGroups
        {
           OrganizationId = organizationId,
           GroupCode = groupCode
        });

        if (groups.Count > 0)
            return groups[0].GroupIdentifier;

        failure.AddError($"Group '{groupCode}' is not found");

        return null;
    }

    #endregion
}

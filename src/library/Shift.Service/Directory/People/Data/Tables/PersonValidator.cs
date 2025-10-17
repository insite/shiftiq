using System.Text.RegularExpressions;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Service.Security;

namespace Shift.Service.Directory;

public class PersonValidator : IPersonValidator
{
    private static readonly Regex EmailPattern = new Regex(Pattern.ValidEmail, RegexOptions.Compiled);

    private readonly PersonService _personService;
    private readonly UserService _userService;

    public PersonValidator(PersonService personService, UserService userService)
    {
        _personService = personService;
        _userService = userService;
    }

    public async Task<ValidationFailure> ValidateCommandAsync(ImportPerson item, int index,
        CancellationToken cancellation)
    {
        var result = new ValidationFailure();

        if (item.Identifier == default)
            result.AddError($"{nameof(ImportPerson.Identifier)} is a required field. ({index})");

        if (await _personService.AssertAsync(item.Identifier, cancellation))
            result.AddError($"{nameof(ImportPerson.Identifier)} must be unique. ({index})");

        if (string.IsNullOrEmpty(item.Email))
            result.AddError($"{nameof(ImportPerson.Email)} is a required field. ({index})");

        if (!EmailPattern.IsMatch(item.Email))
            result.AddError($"{nameof(ImportPerson.Email)} field has an unexpected format. ({index})");

        if (string.IsNullOrEmpty(item.FirstName))
            result.AddError($"{nameof(ImportPerson.FirstName)} is a required field. ({index})");

        if (string.IsNullOrEmpty(item.LastName))
            result.AddError($"{nameof(ImportPerson.LastName)} is a required field. ({index})");

        if (!string.IsNullOrEmpty(item.EmployeeType) &&
            item.EmployeeType != "Employee" && item.EmployeeType != "Contractor")
        {
            result.AddError($"{nameof(ImportPerson.EmployeeType)} field has an unexpected value. ({index})");
        }

        if (!string.IsNullOrEmpty(item.Manager))
        {
            var isManagerExist = await _userService.CountAsync(new CountUsers { UserEmailExact = item.Manager }, cancellation) > 0;

            if (!isManagerExist)
                result.AddError($"The manager record does not exist in the database. ({index})");
        }

        return result;
    }
}

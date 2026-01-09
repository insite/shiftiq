using System;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    public static class QUserFilterExtensions
    {
        public static IQueryable<VUser> Filter(this IQueryable<VUser> query, QUserFilter filter)
        {
            if (filter.Code.HasValue())
                query = query.Where(x => x.Persons.Any(y => y.PersonCode.Contains(filter.Code)));

            if (filter.FullName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x =>
                            (x.UserFirstName + " " + x.UserLastName).Contains(filter.FullName)
                            || (x.UserLastName + ", " + x.UserFirstName).Contains(filter.FullName)
                            || x.UserFullName.Contains(filter.FullName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.UserFirstName == filter.FullName || x.UserLastName == filter.FullName || x.UserFullName == filter.FullName);
                        break;
                    case "Similar":
                        query = query.Where(x =>
                            x.UserFullName.Contains(filter.FullName)
                            || (x.UserFirstName + " " + x.UserLastName).Contains(filter.FullName)
                            || (x.UserLastName + ", " + x.UserFirstName).Contains(filter.FullName)
                        );
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.FirstName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x => x.UserFirstName.Contains(filter.FirstName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.UserFirstName == filter.FirstName);
                        break;
                    case "Similar":
                        query = query.Where(x => x.UserFirstName.Contains(filter.FirstName));
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.LastName.HasValue())
            {
                switch (filter.NameFilterType)
                {
                    case "Exact":
                        query = query.Where(x => x.UserLastName.Contains(filter.LastName));
                        break;
                    case "ExactName":
                        query = query.Where(x => x.UserLastName == filter.LastName);
                        break;
                    case "Similar":
                        query = query.Where(x => x.UserLastName.Contains(filter.LastName));
                        break;
                    default:
                        throw new NotImplementedException("Filter Not Found: " + filter.NameFilterType);
                }
            }

            if (filter.OrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => x.Persons.Any(y => filter.OrganizationIdentifiers.Contains(y.OrganizationIdentifier)));

            if (filter.EmailOrAlternate.IsNotEmpty())
                query = query.Where(x => x.UserEmail.Contains(filter.EmailOrAlternate) || x.UserEmailAlternate.Contains(filter.EmailOrAlternate));

            if (filter.MustHaveAttempts.HasValue)
            {
                if (filter.MustHaveAttempts.Value)
                    query = query.Where(x => x.LearnerAttempts.Any());
                else
                    query = query.Where(x => !x.LearnerAttempts.Any());
            }

            if (filter.NameOrCode.IsNotEmpty())
                query = query.Where(
                    x => x.UserFullName.Contains(filter.NameOrCode)
                      || x.Persons.Any(y => y.PersonCode != null && y.PersonCode.Contains(filter.NameOrCode)));

            if (filter.AttemptFormOrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => x.LearnerAttempts.Any(y => filter.AttemptFormOrganizationIdentifiers.Contains(y.Form.OrganizationIdentifier)));

            if (filter.AttemptFormIdentifier.HasValue)
                query = query.Where(x => x.LearnerAttempts.Any(y => y.FormIdentifier == filter.AttemptFormIdentifier.Value));

            if (filter.AttemptBankIdentifier.HasValue)
                query = query.Where(x => x.LearnerAttempts.Any(y => y.Form.BankIdentifier == filter.AttemptBankIdentifier.Value));

            if (filter.RegistrationEventIdentifier.HasValue)
            {
                query = query.Where(x => x.Persons.Any(y => y.Registrations.Any(z =>
                    z.EventIdentifier == filter.RegistrationEventIdentifier
                    && z.ApprovalStatus != "Moved"
                    && z.ApprovalStatus != "Waitlisted"
                )));
            }

            if (filter.ExcludeGradebookIdentifier.HasValue)
                query = query.Where(x => !x.Enrollments.Any(y => y.GradebookIdentifier == filter.ExcludeGradebookIdentifier));

            if (filter.ExcludeLearnerJournalSetupIdentifier.HasValue)
            {
                var learnerRole = JournalSetupUserRole.Learner.ToString();

                query = query.Where(x =>
                    !x.JournalSetupUsers.Any(y =>
                        y.JournalSetupIdentifier == filter.ExcludeLearnerJournalSetupIdentifier
                        && y.UserRole == learnerRole
                    )
                );
            }

            if (filter.UserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.UserIdentifiers.Contains(x.UserIdentifier));

            return query;
        }
    }
}

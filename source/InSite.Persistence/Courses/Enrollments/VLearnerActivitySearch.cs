using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class VLearnerActivitySearch
    {
        public static int Count(VLearnerActivityFilter filter)
        {
            using (var db = CreateContext())
                return Filter(filter, db).Count();
        }

        public static List<ListItem> GetGradebooks(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Where(x => x.OrganizationIdentifier == organizationId && x.GradebookIdentifier != null)
                    .Select(x => new
                    {
                        x.GradebookIdentifier,
                        x.GradebookTitle
                    })
                    .Distinct()
                    .OrderBy(x => x.GradebookTitle)
                    .Select(x => new ListItem
                    {
                        Value = x.GradebookIdentifier.ToString(),
                        Text = x.GradebookTitle
                    })
                    .ToList();
            }
        }

        public static List<ListItem> GetPrograms(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => new
                    {
                        x.ProgramIdentifier,
                        x.ProgramName
                    })
                    .Distinct()
                    .OrderBy(x => x.ProgramName)
                    .Select(x => new ListItem
                    {
                        Value = x.ProgramIdentifier.ToString(),
                        Text = x.ProgramName
                    })
                    .ToList();
            }
        }

        public static List<string> GetUserGenders(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Where(x => x.OrganizationIdentifier == organizationId && x.UserGender != null)
                    .Select(x => x.UserGender)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static List<string> GetUserCitizenships(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Where(x => x.OrganizationIdentifier == organizationId && x.UserCitizenship != null)
                    .Select(x => x.UserCitizenship)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static List<VLearnerActivityUserProgram> GetUserPrograms(VLearnerActivityFilter filter)
        {
            List<VLearnerActivity> records;

            using (var db = CreateContext())
            {
                var query = Filter(filter, db)
                    .OrderBy(x => x.UserFullName)
                    .ThenBy(x => x.UserIdentifier)
                    .ThenBy(x => x.ProgramName)
                    .ThenBy(x => x.ProgramIdentifier);

                records = ApplyPaging(query, filter).ToList();
            }

            return ToProgramList(records);
        }

        private static List<VLearnerActivityUserProgram> ToProgramList(List<VLearnerActivity> records)
        {
            var result = new List<VLearnerActivityUserProgram>();
            VLearnerActivityUserProgram p = null;
            foreach (var r in records)
            {
                if (p == null || p.UserIdentifier != r.UserIdentifier || p.ProgramIdentifier != r.ProgramIdentifier)
                {
                    result.Add(p = new VLearnerActivityUserProgram
                    {
                        UserIdentifier = r.UserIdentifier,
                        UserFullName = r.UserFullName,
                        UserEmail = r.UserEmail,
                        UserGender = r.UserGender,
                        UserPhone = r.UserPhone,
                        UserBirthdate = r.UserBirthdate,
                        PersonCode = r.PersonCode,
                        ProgramIdentifier = r.ProgramIdentifier,
                        ProgramName = r.ProgramName
                    });
                }
                else
                {
                    p.Gradebooks.Sort((a, b) => a.GradebookTitle.CompareTo(b.GradebookTitle));
                    p.Credentials.Sort((a, b) => a.CredentialGranted.CompareTo(b.CredentialGranted));
                }

                if (r.GradebookIdentifier.HasValue && !p.Gradebooks.Any(x => x.GradebookIdentifier == r.GradebookIdentifier))
                {
                    p.Gradebooks.Add(new VLearnerActivityUserProgram.Gradebook
                    {
                        GradebookIdentifier = r.GradebookIdentifier.Value,
                        GradebookTitle = r.GradebookTitle
                    });
                }

                if (r.AchievementIdentifier.HasValue && r.CredentialGranted.HasValue && !p.Credentials.Any(x => x.AchievementIdentifier == r.AchievementIdentifier))
                {
                    p.Credentials.Add(new VLearnerActivityUserProgram.Credential
                    {
                        AchievementIdentifier = r.AchievementIdentifier.Value,
                        CredentialGranted = r.CredentialGranted.Value
                    });
                }
            }

            if (p != null)
            {
                p.Gradebooks.Sort((a, b) => a.GradebookTitle.CompareTo(b.GradebookTitle));
                p.Credentials.Sort((a, b) => a.CredentialGranted.CompareTo(b.CredentialGranted));
            }

            return result;
        }

        private static IQueryable<VLearnerActivity> ApplyPaging(IQueryable<VLearnerActivity> query, VLearnerActivityFilter filter)
        {
            return filter.Paging == null || filter.Paging.Take == null
                ? query
                : query.Where(x => x.RowNumber > filter.Paging.Skip && x.RowNumber <= filter.Paging.Skip + filter.Paging.Take);
        }

        private static IQueryable<VLearnerActivity> Filter(VLearnerActivityFilter filter, InternalDbContext db)
        {
            var query = db.VLearnerActivities.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.CredentialGrantedSince.HasValue)
                query = query.Where(x => x.CredentialGranted >= filter.CredentialGrantedSince);

            if (filter.CredentialGrantedBefore.HasValue)
                query = query.Where(x => x.CredentialGranted < filter.CredentialGrantedBefore);

            if (filter.EnrollmentCreatedSince.HasValue)
                query = query.Where(x => x.EnrollmentCreated >= filter.EnrollmentCreatedSince);

            if (filter.EnrollmentCreatedBefore.HasValue)
                query = query.Where(x => x.EnrollmentCreated < filter.EnrollmentCreatedBefore);

            if (!string.IsNullOrEmpty(filter.UserEmail))
                query = query.Where(x => x.UserEmail.StartsWith(filter.UserEmail));

            if (!string.IsNullOrEmpty(filter.UserFirstName))
                query = query.Where(x => x.UserFirstName.StartsWith(filter.UserFirstName));

            if (!string.IsNullOrEmpty(filter.PersonCode))
                query = query.Where(x => x.PersonCode.Contains(filter.PersonCode));

            if (!string.IsNullOrEmpty(filter.UserLastName))
                query = query.Where(x => x.UserLastName.StartsWith(filter.UserLastName));

            if (filter.IsLearner.HasValue)
                query = query.Where(x => x.IsLearner == filter.IsLearner);

            if (filter.IsAdministrator.HasValue)
                query = query.Where(x => x.IsAdministrator == filter.IsAdministrator);

            if (filter.UserGenders != null && filter.UserGenders.Length > 0)
                query = query.Where(x => filter.UserGenders.Contains(x.UserGender));

            if (filter.UserCitizenships != null && filter.UserCitizenships.Length > 0)
                query = query.Where(x => filter.UserCitizenships.Contains(x.UserCitizenship));

            if (filter.GradebookIdentifiers != null && filter.GradebookIdentifiers.Length > 0)
                query = query.Where(x => filter.GradebookIdentifiers.Contains(x.GradebookIdentifier.Value));

            if (filter.MembershipStatusItemIdentifiers != null && filter.MembershipStatusItemIdentifiers.Length > 0)
                query = query.Where(x => filter.MembershipStatusItemIdentifiers.Contains(x.MembershipStatusItemIdentifier.Value));

            if (filter.EmployerGroupIdentifiers != null && filter.EmployerGroupIdentifiers.Length > 0)
                query = query.Where(x => filter.EmployerGroupIdentifiers.Contains(x.EmployerGroupIdentifier.Value));

            if (filter.ProgramIdentifiers != null && filter.ProgramIdentifiers.Length > 0)
            {
                query = query.Where(x => filter.ProgramIdentifiers.Contains(x.ProgramIdentifier));
            }

            return query;
        }

        private static InternalDbContext CreateContext() => new InternalDbContext(false, false);
    }
}

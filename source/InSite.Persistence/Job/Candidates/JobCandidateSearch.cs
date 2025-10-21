using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class JobCandidateSearch
    {
        #region Constants

        public static readonly string[] KeywordConjunctions = new string[] { " this ", " is ", " no ", " it ", " and ", " a ", " to ", " for ", " the ", " at ", " in ", " my " };

        public static readonly char[] KeywordGarbageChars = new char[] { ';', ':', '\'', '"', '-', ',', '&', '/', '\\', '(', ')', '[', ']' };

        #endregion

        #region SELECT (CandidateFilter)

        public static int CountByCandidateFilter(JobCandidateFilter filter)
        {
            using (var db = new InternalDbContext())
                return CountByCandidateFilter(db, filter);
        }

        private static int CountByCandidateFilter(InternalDbContext context, JobCandidateFilter filter) =>
            CreateQueryByCandidateFilter(filter, context).Count();

        public static List<Person> SelectList(JobCandidateFilter filter, string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "Updated DESC";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByCandidateFilter(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<Person> CreateQueryByCandidateFilter(JobCandidateFilter filter, InternalDbContext db) =>
            FilterQueryByCandidateFilter(filter, db.Persons.AsExpandable(), db);

        private static IQueryable<Person> FilterQueryByCandidateFilter(JobCandidateFilter filter, IQueryable<Person> query, InternalDbContext db)
        {
            db.Database.CommandTimeout = 2 * 60; // 2 minutes

            var predicate = PredicateBuilder.True<Person>();

            if (filter.Cities.IsNotEmpty())
                predicate = predicate.And(x => filter.Cities.Any(y => y == x.HomeAddress.City));

            if (filter.IsActivelySeeking)
                predicate = predicate.And(x => x.CandidateIsActivelySeeking == true);

            if (filter.IsApproved.HasValue)
                predicate = filter.IsApproved == true ? predicate.And(x => x.JobsApproved != null)
                    : predicate.And(x => x.JobsApproved == null);

            if (filter.HightSchoolDiploma)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "High School Diploma"));

            if (filter.CollegeUniversityCertificate)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "College / University Certificate"));

            if (filter.CollegeDiploma)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "College Diploma"));

            if (filter.TradesCertificate)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "Trades Certificate/Apprenticeship"));

            if (filter.BachelorsDegree)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "Bachelor’s Degree"));

            if (filter.MastersDegree)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "Master’s Degree"));

            if (filter.DoctoralDegree)
                predicate = predicate.And(x => x.User.CandidateEducations.Any(y => y.EducationQualification == "Doctoral Degree"));

            if (filter.Occupation.IsNotEmpty())
            {
                var builder = new StringBuilder(filter.Occupation.Trim());

                foreach (var garbageChar in KeywordGarbageChars) builder.Replace(garbageChar, ' ');
                foreach (var conjunction in KeywordConjunctions) builder.Replace(conjunction, " ");
                builder.Replace("  ", " ");

                var words = builder.ToString().Split(' ');
                predicate = predicate.And(
                    x =>
                        db.TPersonFields.Where(y =>
                            y.OrganizationIdentifier == x.OrganizationIdentifier
                            && y.UserIdentifier == x.UserIdentifier
                            && y.FieldName == "Industry Interest Area"
                            && words.Any(z => y.FieldValue.Contains(z))
                        ).Any()
                );
            }

            if (!string.IsNullOrEmpty(filter.Keywords))
            {
                var builder = new StringBuilder(filter.Keywords.Trim());

                foreach (var garbageChar in KeywordGarbageChars) builder.Replace(garbageChar, ' ');
                foreach (var conjunction in KeywordConjunctions) builder.Replace(conjunction, " ");
                builder.Replace("  ", " ");

                var keywords = builder.ToString().Split(new char[] { ' ', ',' });
                foreach (var keyword in keywords)
                {
                    predicate = predicate.And(x =>
                        x.User.FirstName == keyword
                        || x.User.LastName == keyword
                        || x.User.CandidateExperiences.Any(y =>
                               y.EmployerName.Contains(keyword)
                            || y.ExperienceJobTitle.Contains(keyword)
                            || y.ExperienceCountry.Contains(keyword)
                            || y.ExperienceCity.Contains(keyword)
                            )
                        || x.User.CandidateEducations.Any(y =>
                               y.EducationInstitution.Contains(keyword)
                            || y.EducationName.Contains(keyword)
                            || y.EducationCountry.Contains(keyword)
                            || y.EducationCity.Contains(keyword)
                            || y.EducationQualification.Contains(keyword)
                            )
                    );
                }
            }

            return query.Where(predicate);
        }

        #endregion

        #region SELECT

        public static string GetCompletionStatus(int percent)
        {
            if (percent <= 30) return "danger";
            if (percent < 70) return "warning";
            return "success";
        }

        #endregion
    }
}

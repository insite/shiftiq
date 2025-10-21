using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common;
using InSite.Domain.Records;
using InSite.UI.Portal.Records.Logbooks.Controls;
using InSite.UI.Portal.Records.Logbooks.Models;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;
using Shift.Toolbox.Progress;

using Comment = Shift.Toolbox.Progress.Comment;
using Experience = Shift.Toolbox.Progress.Experience;

namespace InSite.UI.Portal.Records.Logbooks.Utilities
{
    public static class LogbookResultHelper
    {
        private const string SkillRatingNullValue = "None";

        public static LogbookModel GetLogbookResultPdfModel(
            Guid journalSetupIdentifier,
            Guid userIdentifier,
            HttpServerUtility server,
            Guid organizationIdentifier,
            TimeZoneInfo timeZone,
            string language)
        {
            var person = ServiceLocator.ContactSearch.GetPerson(userIdentifier, organizationIdentifier);

            var logoImageUrl = QuestPDFImageHelper.GetOrganizationLogoUrl(CurrentSessionState.Identity.Organization.Code);
            if (logoImageUrl != null)
                logoImageUrl = server.MapPath(logoImageUrl);

            var journalSetup = ServiceLocator.JournalSearch
                .GetJournalSetup(journalSetupIdentifier);

            var experiences = ServiceLocator.JournalSearch
                .GetExperiences(new QExperienceFilter
                {
                    JournalSetupIdentifier = journalSetupIdentifier,
                    UserIdentifier = userIdentifier
                });

            var result = new LogbookModel()
            {
                LogbookTitle = journalSetup.JournalSetupName,
                OrganizationLogoPath = logoImageUrl,
                PersonFullName = person?.UserFullName,
                PersonEmail = person?.UserEmail,
                Experiences = MapExperiences(experiences, journalSetupIdentifier, timeZone, language),
                Areas = MapAreas(journalSetup, userIdentifier, language),
                Comments = MapComments(journalSetup, userIdentifier, experiences)
            };

            result.ShowSkillRating = result.Areas.SelectMany(x => x.Competencies).Any(x => x.SkillRating != SkillRatingNullValue);
            result.NumberOfHoursText = LabelHelper.GetLabelContentText("Number of Hours");

            return result;
        }

        private static List<Comment> MapComments(QJournalSetup journalSetup, Guid userIdentifier, List<QExperience> experiences)
        {
            var journal = ServiceLocator.JournalSearch.GetJournal(journalSetup.JournalSetupIdentifier, userIdentifier);
            if (journal == null)
                return new List<Comment>();

            return ServiceLocator.JournalSearch
                .GetJournalComments(journal.JournalIdentifier)
                .Where(x => !x.CommentIsPrivate || x.AuthorUserIdentifier == userIdentifier)
                .Select(x =>
                {
                    return new Comment()
                    {
                        AuthorName = x.AuthorUserName,
                        PostedOn = $"posted {x.CommentPosted.Humanize()}",
                        Text = x.CommentText,
                        Title = GetTitle((x.ContainerIdentifier != x.LogbookIdentifier), x.ContainerIdentifier)
                    };
                })
                .ToList();

            string GetTitle(bool isExperience, Guid containerIdentifier)
            {
                if (!isExperience)
                    return string.Empty;

                var experience = experiences.Find(x => x.ExperienceIdentifier == containerIdentifier);
                return $"Entry #{experience.Sequence} added on {experience.ExperienceCreated.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone)}";
            }
        }

        private static List<Area> MapAreas(QJournalSetup journalSetup, Guid userIdentifier, string language)
        {
            var areas = CompetencyHelper.GetAreas(journalSetup.JournalSetupIdentifier, language, true);

            if (areas == null)
                return null;

            var userCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(new QExperienceCompetencyFilter
            {
                JournalSetupIdentifier = journalSetup.JournalSetupIdentifier,
                UserIdentifier = userIdentifier
            })
            .GroupBy(x => x.CompetencyStandardIdentifier)
            .Select(x => new
            {
                Identifier = x.Key,
                JournalItems = x.Count(),
                Hours = x.Sum(y => y.CompetencyHours ?? 0)
            })
            .ToDictionary(x => x.Identifier);

            var areaRequirements = ServiceLocator.JournalSearch.GetAreaRequirements(journalSetup.JournalSetupIdentifier)
                .Where(x => x.AreaHours.HasValue && x.AreaHours.Value > 0)
                .ToDictionary(x => x.AreaStandardIdentifier, x => x);

            var results = areas
                .Select(x => new Area()
                {
                    Sequence = x.Sequence,
                    Name = x.Name,
                    RequiredHours = areaRequirements.GetOrDefault(x.Identifier)?.AreaHours,
                    Competencies = x.Competencies.Select(y =>
                    {
                        if (!userCompetencies.TryGetValue(y.Identifier, out var userCompetency))
                            userCompetency = null;

                        return new Competency()
                        {
                            Sequence = y.Sequence,
                            Name = y.Name,
                            Hours = ((y.Hours.HasValue && y.Hours.Value > 0)
                                    ? $"{(userCompetency?.Hours ?? 0):n2} of {y.Hours.Value:n2}"
                                    : $"{(userCompetency?.Hours ?? 0):n2}"),
                            JournalItems = (y.JournalItems.HasValue
                                        ? $"{userCompetency?.JournalItems ?? 0} of {y.JournalItems.Value}"
                                        : $"{userCompetency?.JournalItems ?? 0}"),
                            RequiredHours = y.Hours,
                            CompletedHours = userCompetency?.Hours ?? 0,
                            RequiredJournalItems = y.JournalItems,
                            CompletedJournalItems = userCompetency?.JournalItems ?? 0,
                            SkillRating = y.SkillRating.HasValue ? $"{(y?.SkillRating ?? 0):n2}" : SkillRatingNullValue,
                            IncludeHoursToArea = y.IncludeHoursToArea
                        };
                    })
                    .ToList()
                })
                .ToList();

            return results;
        }

        private static List<Experience> MapExperiences(List<QExperience> experiences, Guid journalSetupIdentifier, TimeZoneInfo timeZone, string language)
        {
            var results = new List<Experience>();

            if (experiences == null || experiences.Count == 0)
                return results;

            var experienceFields = ServiceLocator.JournalSearch.GetJournalSetupFields(journalSetupIdentifier);

            foreach (var experience in experiences)
            {
                var _experienceFields = experienceFields
                    .Select(entity =>
                    {
                        var content = ServiceLocator.ContentSearch.GetBlock(entity.JournalSetupFieldIdentifier);
                        var fieldType = entity.FieldType.ToEnum<JournalSetupFieldType>();
                        var description = ExperienceFieldDescription.Items[fieldType];
                        var fieldItem = new ExperienceField()
                        {
                            Title = content[JournalSetupField.ContentLabels.LabelText].Text.Get(language)
                                .IfNullOrEmpty(entity.FieldType),
                            Value = StringHelper.StripHtml(description.GetHtml(experience))
                        };

                        return fieldItem;
                    }).ToList();

                results.Add(new Experience()
                {
                    ExperienceCreated = TimeZones.Format(experience.ExperienceCreated, timeZone, false),
                    Sequence = experience.Sequence,
                    Status = experience.ExperienceValidated.HasValue
                            ? $"Validated by {experience?.Validator?.UserFullName ?? UserNames.Someone}"
                            : "Not Validated",
                    ExperienceFields = _experienceFields

                });
            }

            return results;
        }
    }
}
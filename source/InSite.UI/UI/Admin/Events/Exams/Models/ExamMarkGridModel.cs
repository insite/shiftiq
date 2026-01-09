using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Events.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;

namespace InSite.Admin.Events.Exams.Controls
{
    public class ExamMarkGridModel
    {
        private readonly TimeZoneInfo _zone;
        private readonly Dictionary<Guid, QAttempt> _attemptCache = new Dictionary<Guid, QAttempt>();

        public ExamMarkGridModel(TimeZoneInfo zone)
        {
            _zone = zone;
        }

        public bool AllowWithhold(object o)
        {
            var registration = (QRegistration)o;
            var score = registration.Score;
            var assigned = registration.GradeAssigned;
            var withheld = registration.GradeWithheld;
            var published = registration.GradePublished;
            return (score.HasValue || assigned.HasValue) && !withheld.HasValue && !published.HasValue;
        }

        public bool AllowRelease(object o)
        {
            var registration = (QRegistration)o;
            return registration.GradeWithheld.HasValue;
        }

        public bool AllowPublish(object o)
        {
            var registration = (QRegistration)o;
            return !registration.GradeWithheld.HasValue && registration.IsPresent;
        }

        public void ValidateGrades(Guid @event, QRegistration[] registrations)
        {
            var identifiers = registrations.Select(x => x.RegistrationIdentifier).ToArray();
            if (identifiers.Length > 0)
                ServiceLocator.SendCommand(new ValidateEventScores(@event, identifiers));
        }

        public void WithholdGrade(Guid registration)
        {
            ServiceLocator.SendCommand(new ChangeGrading(registration, "Withheld", null, null));
        }

        public void ReleaseGrade(QRegistration registration)
        {
            ReleaseGrades(new[] { registration });
        }

        public void ReleaseGrades(QRegistration[] registrations)
        {
            foreach (var registration in registrations)
                if (registration.GradeWithheld.HasValue)
                    ServiceLocator.SendCommand(new ChangeGrading(registration.RegistrationIdentifier, "Released", null, null));
        }

        public void PublishGrade(Guid @event, Guid registration)
        {
            PublishGrades(@event, new Guid[] { registration });
        }

        public void PublishGrades(Guid @event, Guid[] registrations)
        {
            if (registrations.Length > 0)
                ServiceLocator.SendCommand(new PublishEventScores(@event, registrations, true));
        }

        public string FormatScore(object o)
        {
            var registration = (QRegistration)o;
            var attempt = GetAttempt(registration.AttemptIdentifier);

            if (attempt == null || !attempt.AttemptGraded.HasValue)
                return string.Empty;

            var html = new StringBuilder();

            html.AppendFormat("<div>{0:p0}</div>", attempt.AttemptScore)
                .AppendFormat("<div class='form-text'>{0} / {1}</div>", attempt.AttemptPoints, attempt.FormPoints);

            if (attempt.AttemptIsPassing)
                html.Append("<span class='badge bg-success'>Pass</span>");
            else
                html.Append("<span class='badge bg-danger'>Fail</span>");

            return html.ToString();
        }

        public string FormatTime(object o)
        {
            var registration = (QRegistration)o;
            var attempt = GetAttempt(registration.AttemptIdentifier);

            if (attempt == null)
                return string.Empty;

            var html = new StringBuilder();

            if (attempt.AttemptStarted.HasValue)
                html.Append("<div>Started ").Append(attempt.AttemptStarted.Value.Format(_zone, true)).Append("</div>");

            if (attempt.AttemptGraded.HasValue)
                html.Append("<div>Completed ").Append(attempt.AttemptGraded.Value.Format(_zone, true)).Append("</div>");

            if (attempt.AttemptImported.HasValue)
                html.Append("<div class='form-text'>Imported ").Append(attempt.AttemptImported.Value.Format(_zone, true)).Append("</div>");

            if (attempt.AttemptDuration.HasValue)
                html.Append("<div class='form-text'>Time Taken = ")
                    .Append(((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second))
                    .Append("</div>");

            return html.ToString();
        }

        public string GetGradeStatus(object o)
        {
            var registration = (QRegistration)o;

            if (!registration.AttemptIdentifier.HasValue)
                return string.Empty;

            var html = new StringBuilder();

            html.Append("<div class='workflow-status float-end'>");

            if (registration.GradePublished.HasValue)
                html.Append("<span class='badge bg-success'>Published</span>");

            else if (registration.GradeReleased.HasValue)
                html.Append($"<span class='badge bg-primary'>Released</span>");

            else if (registration.GradeWithheld.HasValue)
                html.Append($"<span class='badge bg-danger'>Withheld</span>");

            else if (registration.GradeAssigned.HasValue)
                html.Append(string.Empty);

            else
                html.Append($"<span class='badge bg-custom-default'>Unassigned</span>");

            html.Append("</div>");

            return html.ToString();
        }

        public QRegistration[] GetRegistrations(QRegistrationFilter filter)
        {
            return ServiceLocator.RegistrationSearch.GetRegistrations(filter).ToArray();
        }

        public string GetRegistrationStatus(object o)
        {
            var registration = (QRegistration)o;

            var html = new StringBuilder();

            html.Append("<div class='workflow-status float-end'>");

            if (registration.IsPresent)
                html.Append("<span class='badge bg-success'>Attended</span>");

            else if (registration.AttendanceStatus == "Absent")
                html.Append("<span class='badge bg-danger'>No Show</span>");

            if (registration.GradePublished.HasValue)
                html.Append("<span class='badge bg-success'>Published</span>");

            html.Append("</div>");

            return html.ToString();
        }

        public string GetSebVersion(object o)
        {
            var registration = (QRegistration)o;
            var attempt = GetAttempt(registration.AttemptIdentifier);

            if (attempt == null)
                return string.Empty;

            var sebVersion = AttemptHelper.GetSebVersion(attempt.UserAgent);

            return sebVersion.IsEmpty() ? null : $"<span class='badge bg-info'>SEB v{sebVersion}</span>";
        }

        public QAttempt GetAttempt(Guid? id)
        {
            if (!id.HasValue)
                return null;

            if (!_attemptCache.ContainsKey(id.Value))
                _attemptCache.Add(id.Value, ServiceLocator.AttemptSearch.GetAttempt(id.Value));

            return _attemptCache[id.Value];
        }
    }
}
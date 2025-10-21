using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class ExamMarkGrid : SearchResultsGridViewController<QRegistrationFilter>
    {
        protected override bool IsFinder => false;

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        public int LoadData(Guid @event)
        {
            CanWrite = Identity.IsGranted(PermissionIdentifiers.Admin_Assessments_Attempts, PermissionOperation.Write);

            CommandPanel.Visible = CanWrite;

            Search(new QRegistrationFilter { EventIdentifier = @event });
            BindEvent(@event);
            BindStatus();
            return RowCount;
        }

        protected override int SelectCount(QRegistrationFilter filter)
            => ServiceLocator.RegistrationSearch.CountRegistrations(filter);

        protected override IListSource SelectData(QRegistrationFilter filter)
            => ServiceLocator.RegistrationSearch.GetRegistrations(filter, x => x.Form, x => x.Candidate).ToSearchResult();

        private readonly ExamMarkGridModel _logic = new ExamMarkGridModel(User.TimeZone);

        protected ExamMarkGridModel Logic => _logic;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ValidateButton.Click += ValidateButton_Click;
            ReleaseButton.Click += ReleaseButton_Click;
            PublishButton.Click += PublishButton_Click;

            Grid.RowCommand += Grid_RowCommand;
        }

        private void BindEvent(Guid @event)
        {
            GradeLink.NavigateUrl = $"/ui/admin/assessments/attempts/upload?event={@event}";
        }

        private void BindStatus()
        {
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(Filter);

            NoGradesPanel.Visible = registrations.Count == 0;

            ValidateButton.Visible = registrations.Any(x => x.GradingStatus == null || StringHelper.EqualsAny(x.GradingStatus, new[] { "Unassigned", "Assigned", "Released" }));
            ReleaseButton.Visible = registrations.Any(x => StringHelper.Equals(x.GradingStatus, "Withheld"));
            PublishButton.Visible = registrations.Any(x => x.GradingStatus == null || StringHelper.EqualsAny(x.GradingStatus, new[] { "Unassigned", "Assigned", "Released" }));
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            _logic.ValidateGrades(Filter.EventIdentifier.Value, _logic.GetRegistrations(Filter));
            Search(Filter);
            BindStatus();
        }

        private void ReleaseButton_Click(object sender, EventArgs e)
        {
            _logic.ReleaseGrades(_logic.GetRegistrations(Filter));
            Search(Filter);
            BindStatus();
        }

        private void PublishButton_Click(object sender, EventArgs e)
        {
            try
            {
                var registrations = _logic.GetRegistrations(Filter);
                if (registrations.Length == 0)
                {
                    OnPublished(-1, null);
                }
                else
                {
                    var identifiers = registrations.Select(x => x.RegistrationIdentifier).ToArray();
                    _logic.PublishGrades(Filter.EventIdentifier.Value, identifiers);
                    Search(Filter);
                    BindStatus();

                    var count = 0;
                    foreach (var identifier in identifiers)
                    {
                        var registration = ServiceLocator.RegistrationSearch.GetRegistration(identifier);
                        if (registration?.GradingStatus == "Published")
                            count++;
                    }

                    OnPublished(count, null);
                }
            }
            catch (Exception ex)
            {
                var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                ErrorPanel.Visible = true;
                ErrorPanel.AddMessage(AlertType.Error, "An unexpected error occurred sending exam results to Direct Access: " + errors);
            }
        }

        #region OnPublished

        public class PublishedEventArgs : EventArgs
        {
            public int Count { get; set; }
            public string CandidateName { get; set; }
        }

        public delegate void PublishedEventHandler(object sender, PublishedEventArgs e);

        public event PublishedEventHandler Published;

        private void OnPublished(int count, string candidateName)
        {
            Published?.Invoke(this, new PublishedEventArgs { Count = count, CandidateName = candidateName });
        }

        #endregion

        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var registrationId = Grid.GetDataKey<Guid>(e);

            if (e.CommandName == "Publish")
            {
                try
                {
                    _logic.PublishGrade(Filter.EventIdentifier.Value, registrationId);

                    var registration = ServiceLocator.RegistrationSearch.GetRegistration(registrationId, x => x.Candidate);
                    var count = registration?.GradingStatus == "Published" ? 1 : 0;

                    OnPublished(count, registration.Candidate?.UserFullName);
                }
                catch (Exception ex)
                {
                    var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                    ErrorPanel.Visible = true;
                    ErrorPanel.AddMessage(AlertType.Error, "An unexpected error occurred sending exam results to Direct Access: " + errors);
                }
            }
            else if (e.CommandName == "Release")
            {
                var registration = ServiceLocator.RegistrationSearch.GetRegistration(registrationId);
                _logic.ReleaseGrade(registration);
            }
            else if (e.CommandName == "Withhold")
            {
                _logic.WithholdGrade(registrationId);
            }

            Search(Filter);
        }
    }
}
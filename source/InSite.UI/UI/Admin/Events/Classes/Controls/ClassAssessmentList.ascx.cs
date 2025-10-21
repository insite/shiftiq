using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Attempts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassAssessmentList : BaseUserControl
    {
        private Guid EventIdentifier
        {
            get => (Guid)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        public event EventHandler FormsShown;
        public event EventHandler FormsHidden;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddFormButton.Click += AddExamFormButton_Click;
            FormRepeater.ItemDataBound += FormRepeater_ItemDataBound;
            FormRepeater.ItemCommand += FormRepeater_ItemCommand;
        }

        public bool Bind(Guid eventId, bool canWrite)
        {
            EventIdentifier = eventId;

            CanWrite = canWrite;

            FormPopupSelectorWindow.Filter.IsPublished = true;

            return BindFormRepeater();
        }

        private void AddExamFormButton_Click(object sender, EventArgs e)
        {
            var formId = ValidateForm();
            if (formId == null)
                return;

            ServiceLocator.SendCommand(new AddEventAssessment(EventIdentifier, formId.Value));

            AddFormIdentifier.Value = null;

            if (BindFormRepeater())
                FormsShown?.Invoke(this, new EventArgs());
        }

        private bool BindFormRepeater()
        {
            var forms = ServiceLocator.EventSearch.GetEventAssessmentForms(EventIdentifier);
            
            FormRepeater.DataSource = forms;
            FormRepeater.DataBind();

            FormRepeater.Visible = forms.Count > 0;

            return forms.Count > 0;
        }

        private void FormRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var formId = Guid.Parse(e.CommandArgument.ToString());
            var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter
            {
                EventIdentifier = EventIdentifier,
                FormIdentifier = formId
            });

            if (attempts.Count > 0)
            {
                var message = $"This exam form cannot be detached from this event because it is selected for {"exam candidate".ToQuantity(attempts.Count)}.";

                ScriptManager.RegisterStartupScript(Page, GetType(), "DetachExamForm", $"alert('{message}');", true);
            }
            else
            {
                ServiceLocator.SendCommand(new RemoveEventAssessment(EventIdentifier, formId));

                if (!BindFormRepeater())
                    FormsHidden?.Invoke(this, new EventArgs());
            }
        }

        private void FormRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var form = (QEventAssessmentForm)e.Item.DataItem;

            ShowFormMaterials(form, e.Item);
        }

        private Guid? ValidateForm()
        {
            if (!Guid.TryParse(AddFormIdentifier.Value, out Guid formId))
                return null;

            var form = ServiceLocator.BankSearch.GetForm(formId);
            if (form == null)
            {
                AddFormAlert.AddMessage(AlertType.Error, "Form is not found");
                return null;
            }

            if (!string.IsNullOrEmpty(form.FormName))
                return formId;

            AddFormAlert.AddMessage(AlertType.Error, "Form Name field is required.");

            return null;
        }

        private void ShowFormMaterials(QEventAssessmentForm form, RepeaterItem item)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(form.BankIdentifier);
            var f = bank.FindForm(form.FormIdentifier);

            var materialsForDistribution = f.Content?.MaterialsForDistribution?.Default;
            var materialsForParticipation = f.Content?.MaterialsForParticipation?.Default;

            if (materialsForDistribution.HasValue() || materialsForParticipation.HasValue())
            {
                var materials = (System.Web.UI.WebControls.Literal)item.FindControl("FormMaterials");
                materials.Visible = true;

                var html = new StringBuilder();
                if (materialsForDistribution.HasValue())
                {
                    html.Append("<div class='form-group mb-3'>");
                    html.Append("<label class='form-label'>Materials for Distribution</label>");
                    html.Append("<div>" + Markdown.ToHtml(materialsForDistribution) + "</div>");
                    html.Append("</div>");
                }
                if (materialsForParticipation.HasValue())
                {
                    html.Append("<div class='form-group mb-3'>");
                    html.Append("<label class='form-label'>Materials for Participation/Candidates</label>");
                    html.Append("<div>" + Markdown.ToHtml(materialsForParticipation) + "</div>");
                    html.Append("</div>");
                }
                materials.Text = html.ToString();
            }
        }
    }
}
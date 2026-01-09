using System;

using InSite.Common.Web;

namespace InSite.Admin.Workflow.Forms.Questions.Models
{
    public class DefineLikertScalesNavigator
    {
        private Guid _survey { get; set; }
        private Guid _question { get; set; }

        public DefineLikertScalesNavigator(Guid survey, Guid question)
        {
            _survey = survey;
            _question = question;
        }

        public void RedirectToOutline()
        {
            var url = GetOutlineUrl();
            HttpResponseHelper.Redirect(url, true);
        }

        public string GetOutlineUrl()
        {
            return $"/ui/admin/workflow/forms/outline?form={_survey}&question={_question}&panel=questions";
        }

        public string GetParentLinkParameters(string parentName)
        {
            return parentName.EndsWith("/outline") ? $"form={_survey}&question={_question}&panel=questions" : null;
        }
    }
}
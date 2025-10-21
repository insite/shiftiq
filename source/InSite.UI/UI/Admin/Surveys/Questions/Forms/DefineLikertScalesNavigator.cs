using System;

using InSite.Common.Web;

namespace InSite.Admin.Surveys.Questions.Forms
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
            return $"/ui/admin/surveys/forms/outline?survey={_survey}&question={_question}&panel=questions";
        }

        public string GetParentLinkParameters(string parentName)
        {
            return parentName.EndsWith("/outline") ? $"survey={_survey}&question={_question}&panel=questions" : null;
        }
    }
}
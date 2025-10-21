using System;

namespace InSite.Portal.Surveys.Responses
{
    public class SurveyComboBox : Common.Web.UI.ComboBox
    {
        public Guid? ListIdentifier
        {
            get { return (Guid?)ViewState[nameof(ListIdentifier)]; }
            set { ViewState[nameof(ListIdentifier)] = value; }
        }
    }
}
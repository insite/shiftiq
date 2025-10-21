using System;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CompareValidator : System.Web.UI.WebControls.CompareValidator
    {
        #region Properties

        public string ImageUrl { get; set; }

        public String FieldName { get; set; }

        #endregion

        #region Construction

        public CompareValidator() 
        { 
            Display = ValidatorDisplay.Static; 
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var imageAlt = "Do not match";

            if (!string.IsNullOrEmpty(ErrorMessage))
                imageAlt = StringHelper.StripHtml(ErrorMessage);

            if (string.IsNullOrEmpty(Text))
				Text = string.Format("<span class='text-danger' title='{0}'><i class='fas fa-circle'></i> {0}</span>", imageAlt);
        }

        #endregion
    }
}
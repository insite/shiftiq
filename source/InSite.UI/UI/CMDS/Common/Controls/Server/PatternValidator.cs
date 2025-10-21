using System;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class PatternValidator : RegularExpressionValidator
    {
        #region Fields

        private String _imageUrl = Constants.ValidatorErrorImageUrl;

        #endregion

        #region Properties

        public String ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public String FieldName
        {
            get; set;
        }

        #endregion

        #region Rendering

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (String.IsNullOrEmpty(Text))
                Text = string.Format("<img src='{0}' alt='Incorrect Format' border='0' align='absmiddle' />", ResolveUrl(_imageUrl));

            if (String.IsNullOrEmpty(ErrorMessage) && !String.IsNullOrEmpty(FieldName))
				ErrorMessage = string.Format("{0} is not in the correct format", FieldName);
        }

        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FileExtensionValidator : CustomValidator
    {
        #region Properties

        [TypeConverter(typeof(StringArrayConverter))]
        public String[] FileExtensions
        {
            get { return (String[])ViewState[nameof(FileExtensions)]; }
            set { ViewState[nameof(FileExtensions)] = value; }
        }

        public String ImageUrl
        {
            get { return (String)ViewState[nameof(ImageUrl)]; }
            set { ViewState[nameof(ImageUrl)] = value; }
        }

        #endregion

        #region Construction

        public FileExtensionValidator()
        {
            ImageUrl = string.Empty;
            ClientValidationFunction = "inSite.common.validators.fileExtensionValidator.validate";
        }

        #endregion

        #region Validation

        protected override Boolean OnServerValidate(String value)
        {
            Boolean valid = true;

            if (!String.IsNullOrEmpty(value) && FileExtensions.IsNotEmpty())
                valid = Regex.IsMatch(value, String.Format(@".+(?i)({0})$", GetExtensions(FileExtensions, "|").Replace(".", "\\.")));

            return valid;
        }

        #endregion

        #region Rendering

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (base.RenderUplevel)
            {
                if (FileExtensions != null)
                {
                    String ext = GetExtensions(FileExtensions, "|");
                    if (!String.IsNullOrEmpty(ext))
                        Page.ClientScript.RegisterExpandoAttribute(ClientID, "fileExtensions", ext);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.IsPostBack && FileExtensions.IsNotEmpty())
            {
                String extensions = GetExtensions(FileExtensions, ", *");

                if (String.IsNullOrEmpty(ErrorMessage))
                    ErrorMessage = String.Format("Allowed extensions:{0}", extensions);

                if (String.IsNullOrEmpty(Text))
                    Text = string.Format(
                        "<span class='text-danger' title='Invalid Extension'><i class='fas fa-circle'></i> {0}</span>"
                      , ErrorMessage
                    );
            }
        }

        #endregion

        #region Helpers

        internal static String GetExtensions(String[] data, String separator)
        {
            StringBuilder result = new StringBuilder();

            foreach (String ext in data)
                if (!String.IsNullOrEmpty(ext))
                    result.AppendFormat("{1}.{0}", ext, separator);

            return result.Length > 0 ? result.Remove(0, 1).ToString() : null;
        }

        #endregion
    }
}

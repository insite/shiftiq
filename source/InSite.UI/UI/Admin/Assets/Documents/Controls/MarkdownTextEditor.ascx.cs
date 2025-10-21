using System;
using System.IO;

using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assets.Documents.Controls
{
    public partial class MarkdownTextEditor : BaseUserControl
    {
        #region Classes

        [Serializable]
        protected class ControlData
        {
            #region Properties

            public Guid IssueIdentifier { get; private set; }

            #endregion

            #region Construction

            public ControlData()
            {

            }

            #endregion
        }

        #endregion

        #region Properties

        protected ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            private set => ViewState[nameof(CurrentData)] = value;
        }

        public string Text
        {
            get => MDText.Text;
            set
            {
                //if (CurrentData == null)
                //    throw new ApplicationError("The contol is not initialized.");

                MDText.Text = value;
            }
        }

        protected string TextEditorObject => $"markdownTextEditor_{ClientID}";

        #endregion

        #region Initialization

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            TextDropZone.Attributes["class"] = $"form-group {TextDropZone.ClientID}";
        }

        #endregion

        #region Methods

        public void LoadData(string filePath)
        {
            CurrentData = new ControlData();
            using (StreamReader sr = new StreamReader(filePath))
            {
                MDText.Text = sr.ReadToEnd();
            }
        }

        #endregion
    }
}
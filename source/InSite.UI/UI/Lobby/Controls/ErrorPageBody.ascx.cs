using System;
using System.Web.UI;

namespace InSite.UI.Lobby.Controls
{
    public partial class ErrorPageBody : UserControl
    {
        public class ContentEventArgs : EventArgs
        {
            public Control Container { get; }

            public ContentEventArgs(Control container)
            {
                Container = container;
            }
        }

        public delegate void AnswerContainerEventHandler(object sender, ContentEventArgs e);

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ContentTemplate { get; set; }

        public event AnswerContainerEventHandler ContentCreated;

        protected override void CreateChildControls()
        {
            ContentTemplate?.InstantiateIn(ContentPlaceholder);
            ContentCreated?.Invoke(this, new ContentEventArgs(ContentPlaceholder));

            base.CreateChildControls();
        }
    }
}
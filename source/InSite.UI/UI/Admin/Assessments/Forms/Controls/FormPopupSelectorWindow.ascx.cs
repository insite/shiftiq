using System;
using System.ComponentModel;
using System.Web.UI;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormPopupSelectorWindow : UserControl
    {
        [Serializable]
        public class FilterSettings 
        { 
            public bool IsPublished { get; set; }
        }

        [PersistenceMode(PersistenceMode.Attribute), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FilterSettings Filter 
            => (FilterSettings)(ViewState[nameof(Filter)] ?? (ViewState[nameof(Filter)] = new FilterSettings()));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StyleLiteral.ContentKey = GetType().FullName;
        }
    }
}
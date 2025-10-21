using System;

using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Pages;

using Shift.Common;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentBlocksCreator : ContentBlocksEditorBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateButton.Click += CreateButton_Click;
        }

        private void CreateButton_Click(object sender, EventArgs e) => OnInsert();

        protected override void OnPreRender(EventArgs e)
        {
            var validationGroup = ValidationGroup + "Creator";

            ValidationSummary.ValidationGroup = validationGroup;
            ControlRequiredValidator.ValidationGroup = validationGroup;
            CreateButton.ValidationGroup = validationGroup;

            base.OnPreRender(e);
        }

        public void SetDefaultInputValues()
        {
            BindControlSelector(ControlSelector);
        }

        public void GetInputValues(PageState page)
        {
            page.ContentControl = ControlSelector.Value;
            page.Title = StringHelper.FirstValue(Title.Text, ControlSelector.GetSelectedOption().Text);
            page.Slug = StringHelper.Sanitize(page.Title, '-', true, new[] { '_' });
        }

        public void GetInputValues(QPage page)
        {
            page.ContentControl = ControlSelector.Value;
            page.Title = StringHelper.FirstValue(Title.Text, ControlSelector.GetSelectedOption().Text);
            page.PageSlug = StringHelper.Sanitize(page.Title, '-', true, new[] { '_' });
        }
    }
}
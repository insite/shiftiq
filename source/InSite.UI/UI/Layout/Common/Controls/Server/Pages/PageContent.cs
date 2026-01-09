using System;
using System.Web.UI;

using ContextType = Shift.Constant.ContextType;
using IPageContent = InSite.Common.Web.UI.PageContentRenderer.IPageContent;

namespace InSite.Common.Web.UI
{
    public class PageContent : Control, IPageContent
    {
        #region IPageContent

        string IPageContent.Key => InnerContentKey;

        Action<HtmlTextWriter> IPageContent.Render => InnerRenderControls;

        #endregion

        #region Properties

        public virtual ContextType RenderContext
        {
            get => (ContextType)ViewState[nameof(RenderContext)];
            set => ViewState[nameof(RenderContext)] = value;
        }

        public virtual string ContextKey
        {
            get => (string)ViewState[nameof(ContextKey)];
            set => ViewState[nameof(ContextKey)] = value;
        }

        public string ContentKey
        {
            get;
            set;
        }

        private string InnerContentKey
        {
            get => (string)ViewState[nameof(InnerContentKey)];
            set => ViewState[nameof(InnerContentKey)] = value;
        }

        public bool RenderRequired
        {
            get => (bool?)ViewState[nameof(RenderRequired)] ?? false;
            set => ViewState[nameof(RenderRequired)] = value;
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            var renderer = PageContentRenderer.Get(RenderContext, ContextKey);

            if (InnerContentKey == null)
                InnerContentKey = string.IsNullOrEmpty(ContentKey)
                    ? renderer.ContextKeyGenerator.Next()
                    : ContentKey;

            renderer.Register(this);

            base.OnLoad(e);
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {

        }

        private void InnerRenderControls(HtmlTextWriter writer)
        {
            RenderChildren(writer);
        }

        #endregion
    }
}
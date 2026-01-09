using System;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class PageHeadContentRenderer : PageContentRenderer
    {
        public override ContextType RenderContext
        {
            get => ContextType.Header;
            set { }
        }

        public override string ContextKey
        {
            get => null;
            set => throw new NotImplementedException();
        }
    }
}
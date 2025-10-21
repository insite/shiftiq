using System;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class PageFooterContent : PageContent
    {
        public override ContextType RenderContext 
        { 
            get => ContextType.Footer;
            set { }
        }

        public override string ContextKey 
        { 
            get => null; 
            set => throw new NotImplementedException(); 
        }
    }
}
using System;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class NavContent : Control
    {
        #region Fields

        private Nav _nav;

        #endregion

        #region Methods

        internal void Register(Nav nav)
        {
            if (_nav != null)
                throw new ApplicationError("The content renderer control is already assigned to Nav: " + ClientID);

            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            if (_nav == null)
                throw new ApplicationError("The content renderer control is not assigned to Nav: " + ClientID);

            _nav.RenderTabContent(writer);
        }

        #endregion
    }
}
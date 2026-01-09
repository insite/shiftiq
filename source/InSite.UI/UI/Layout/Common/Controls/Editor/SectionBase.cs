using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public abstract class SectionBase : UserControl
    {
        #region Events

        public event RedirectUrlHandler Redirect;

        protected void OnRedirect(string url) =>
            Redirect?.Invoke(this, new RedirectUrlArgs(url));

        #endregion

        public abstract void SetOptions(LayoutContentSection options);

        public abstract void SetValidationGroup(string groupName);

        public abstract MultilingualString GetValue();

        public abstract MultilingualString GetValue(string id);

        public abstract IEnumerable<MultilingualString> GetValues();

        public abstract void GetValues(MultilingualDictionary dictionary);

        public abstract void OpenTab(string id);

        public abstract void SetLanguage(string lang);
    }
}
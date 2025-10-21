using System;
using System.Collections.Generic;

using InSite.Admin.Messages.Messages.Forms;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Messages
{
    public partial class Send : AdminBasePage, ICmdsUserControl
    {
        private const string SelfUrl = "/ui/cmds/admin/messages/send";
        private const string ParentUrl = "/ui/cmds/admin/messages/assign-subscribers";

        private TriggerHelper _helper;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _helper = new TriggerHelper(ServiceLocator.MessageSearch, this, ViewState);
            _helper.SetTitle += TriggerHelper_SetTitle;

            TestTab.Visible = Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.Programmers);

            ChangeTriggerButton.Click += ChangeTriggerButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_helper.TriggerModel == null)
                HttpResponseHelper.Redirect(ParentUrl);
            else
                _helper.Load();

            CancelButton.NavigateUrl = ParentUrl;
        }

        private void TriggerHelper_SetTitle(object sender, TriggerHelper.SetTitleArgs args)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: args.Title + $" <span class=\"form-text\">{args.Subtitle}</span>");
        }

        private void ChangeTriggerButton_Click(object sender, EventArgs e)
        {
            if (!ChangeTriggerDate.Value.HasValue)
                return;

            var url = $"{SelfUrl}?message={_helper.MessageGuid}&date={ChangeTriggerDate.Value:yyyy-MM-dd}";

            HttpResponseHelper.Redirect(url, true);
        }

        private Dictionary<Guid, string> _cache = null;

        private Dictionary<Guid, string> EmailCache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new Dictionary<Guid, string>();


                    var contacts = PersonCriteria.Bind(x => new { x.User.Email, x.UserIdentifier }, new PersonFilter
                    {
                        OrganizationOrParentIdentifier = OrganizationIdentifiers.CMDS,
                        IsCmds = true,
                        EmailEnabled = true,
                        IsApproved = true
                    });

                    foreach (var contact in contacts)
                        if (!_cache.ContainsKey(contact.UserIdentifier))
                            _cache.Add(contact.UserIdentifier, contact.Email.ToLower());
                }
                return _cache;
            }
        }

        protected string GetEmailAddress(object o)
        {
            Guid? item = o as Guid?;

            if (item != null)
                if (EmailCache.ContainsKey(item.Value))
                    return $"<a href='mailto:{EmailCache[item.Value]}'>{EmailCache[item.Value]}</a>";

            return string.Empty;
        }

        protected string GetEmailAddressList(object o)
        {
            var items = o as List<Guid>;
            if (items == null)
                return string.Empty;

            var csv = string.Empty;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (EmailCache.ContainsKey(item))
                {
                    if (i == 0)
                        csv += "cc: ";

                    csv += $"<a href='mailto:{EmailCache[item]}'>{EmailCache[item]}</a>";

                    if (i < items.Count - 1)
                        csv += ", ";
                }
            }
            return csv;
        }

    }
}

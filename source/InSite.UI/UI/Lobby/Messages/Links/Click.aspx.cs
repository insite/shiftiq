using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

using Shift.Common.Timeline.Exceptions;

using InSite.Common.Web;

using Shift.Sdk.UI;

namespace InSite.UI.Lobby.Messages.Links
{
    public partial class Click : Page
    {
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _clickLocks = new ConcurrentDictionary<Guid, SemaphoreSlim>();

        private ClickModel _model;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (CreateModel())
                HttpResponseHelper.Redirect(_model.LinkUrl.Replace("&amp;", "&"));
            else
                HttpResponseHelper.SendHttp400("The URL for this page requires a link parameter and a user parameter. Please contact your administrator with the steps to reproduce this error message.");
        }

        private bool CreateModel()
        {
            _model = new ClickModel(Request.QueryString["link"], Request.QueryString["user"]);

            if (_model.IsValid)
                _model.LinkUrl = GetLinkUrl();

            return _model.IsValid && !string.IsNullOrEmpty(_model.LinkUrl);
        }

        public string GetLinkUrl(bool insertMissingLink = false)
        {
            var @lock = _clickLocks.GetOrAdd(_model.Link, x => new SemaphoreSlim(1, 1));

            @lock.Wait();

            try
            {
                var search = ServiceLocator.MessageSearch;
                var store = ServiceLocator.MessageStore;

                var link = search.FindLink(_model.Link);
                if (link == null)
                {
                    if (insertMissingLink)
                        link = store.InsertLink(_model.Link);
                    else
                        return null;
                }

                var browser = $"{Request.Browser.Browser} {Request.Browser.Version}";

                for (var i = 9; i >= 0; i--)
                {
                    try
                    {
                        store.InsertClickthrough(_model.Link, _model.User, Request.UserHostAddress, browser);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 0 || !(ex.InnerException is ConcurrencyException))
                            throw;

                        Task.Delay(200);
                    }
                }

                return link.LinkUrl;
            }
            finally
            {
                @lock.Release();
            }
        }
    }
}
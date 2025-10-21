﻿using System;

using InSite.Application.Responses.Write;
using InSite.Common.Web;

using Shift.Constant;

namespace InSite.Portal.Surveys.Responses
{
    public partial class Delete : ResponseSessionControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += (sender, args) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DeleteButton.Visible = Current.IsValid;
            CancelButton.NavigateUrl = GetReturnUrl()
                ?? (Current.IsValid
                    ? Navigator.GetLaunchPageUrl(Current.SessionIdentifier).ToString()
                    : RelativeUrl.PortalHomeUrl);
        }

        private void Continue()
        {
            var redirectUrl = RelativeUrl.PortalHomeUrl;

            if (Current.IsValid)
            {
                ServiceLocator.SendCommand(new DeleteResponseSession(Current.SessionIdentifier));
                redirectUrl = ResponseSessionNavigator.GetLaunchPageUrl(Current.Survey.Asset, Current.UserIdentifier).ToString();
            }

            HttpResponseHelper.Redirect(GetReturnUrl() ?? redirectUrl, true);
        }

        private string GetReturnUrl() => new ReturnUrl().GetReturnUrl();
    }
}
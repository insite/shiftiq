using System;

using InSite.Common.Web;
using InSite.UI.Admin.Integrations.Tests.Utilities;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

namespace InSite.UI.Admin.Integrations.Tests
{
    public partial class D365 : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MethodSelector.AutoPostBack = true;
            MethodSelector.ValueChanged += (x, y) => OnMethodChanged();

            SendButton.Click += SendButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect("/ui/admin/home");

            PageHelper.AutoBindHeader(this);

            OnMethodChanged();
        }

        private void OnMethodChanged()
        {
            var name = MethodSelector.Value;
            var path = $"~/UI/Admin/Integrations/Tests/Controls/{name}.ascx";
            var control = (ID365Method)MethodContainer.LoadControl(path);

            control.InitMethod();

            RequestPanel.Visible = false;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var methodCtrl = (ID365Method)MethodContainer.GetControl();

            RequestPanel.Visible = true;
            RequestUrl.Text = methodCtrl.GetUrl();
            RequestBody.Text = methodCtrl.GetBody(Formatting.Indented);

            var response = methodCtrl.SendRequest();

            ResponseStatus.Text = response.Status;
            ResponseBody.Text = response.Body;
        }
    }
}
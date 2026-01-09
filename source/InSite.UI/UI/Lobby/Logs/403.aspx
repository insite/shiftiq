<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Register TagPrefix="uc" TagName="ErrorPageHeader" Src="~/UI/Lobby/Controls/ErrorPageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="ErrorPageBody" Src="~/UI/Lobby/Controls/ErrorPageBody.ascx" %>

<%@ Import Namespace="InSite.UI.Lobby.Controls"  %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, viewport-fit=cover">

    <title>HTTP 403 - Forbidden</title>

    <uc:ErrorPageHeader runat="server" />

    <script runat="server">
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ErrorBody.ContentCreated += ErrorBody_ContentCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.StatusCode = 403;

            base.OnLoad(e);
        }

        private void ErrorBody_ContentCreated(object sender, ErrorPageBody.ContentEventArgs e)
        {
            try
            {
                var path = Request.QueryString["path"];
                if (path.IsNotEmpty())
                {
                    var href = (ITextControl)e.Container.FindControl("ErrorHref");
                    href.Text = "<p class='pb-1 text-danger'><i class='fas fa-lock me-1'></i> " + HttpUtility.HtmlEncode(path) + "</p>";
                }

                var message = Request.QueryString["message"]?.Trim();
                if (message.IsNotEmpty())
                {
                    var body = (HtmlGenericControl)e.Container.FindControl("ErrorBody");
                    body.InnerText = StringHelper.DecodeBase64Url(message);
                }
            }
            catch 
            { 

            }
        }
    </script>

</head>
<body>

    <uc:ErrorPageBody runat="server" ID="ErrorBody">
        <ContentTemplate>

            <h1 class="display-5">Access Denied</h1>
            <p runat="server" id="ErrorBody" class="fs-3 pb-1">It seems your account is not granted permission to access this page.</p>
            <asp:Literal runat="server" ID="ErrorHref" />

            <hr class="my-4" />

            <p class="text-body-secondary fs-6 mb-4">
                Contact your administrator if your permissions need to be updated, or click the back button on your browser.
            </p>

            <p>
                <a href="/" class="btn btn-primary"><i class="fas fa-home me-2"></i> Home</a>
                <a href="/ui/lobby/signout" class="btn btn-default ms-1"><i class="fas fa-sign-out me-2"></i> Sign Out</a>
            </p>

        </ContentTemplate>
    </uc:ErrorPageBody>

</body>
</html>

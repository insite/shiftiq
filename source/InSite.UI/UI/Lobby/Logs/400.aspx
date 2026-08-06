<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Register TagPrefix="uc" TagName="ErrorPageHeader" Src="~/UI/Lobby/Controls/ErrorPageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="ErrorPageBody" Src="~/UI/Lobby/Controls/ErrorPageBody.ascx" %>

<%@ Import Namespace="InSite.UI.Lobby.Controls"  %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, viewport-fit=cover">

    <title>HTTP 400 - Bad Request</title>

    <uc:ErrorPageHeader runat="server" />

    <script runat="server">
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ErrorBody.ContentCreated += ErrorBody_ContentCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.StatusCode = 400;

            base.OnLoad(e);
        }

        private void ErrorBody_ContentCreated(object sender, ErrorPageBody.ContentEventArgs e)
        {
            try
            {
                var message = Request.QueryString["message"]?.Trim();
                if (message.IsNotEmpty())
                {
                    var body = (HtmlGenericControl)e.Container.FindControl("ErrorMessage");
                    body.InnerText = StringHelper.DecodeBase64Url(message);
                    body.Visible = true;
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

            <h1 class="display-5">Bad Request</h1>
            <p class="fs-3 pb-1">It seems your browser sent a request that this site could not process.</p>
            <p runat="server" id="ErrorMessage" class="fs-5 text-muted" visible="false"></p>

        </ContentTemplate>
    </uc:ErrorPageBody>

</body>
</html>

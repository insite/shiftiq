<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" CodeBehind="404.aspx.cs" Inherits="InSite.UI.Lobby.Logs._404" %>

<%@ Register TagPrefix="uc" TagName="ErrorPageHeader" Src="~/UI/Lobby/Controls/ErrorPageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="ErrorPageBody" Src="~/UI/Lobby/Controls/ErrorPageBody.ascx" %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, viewport-fit=cover">

    <title>HTTP 404 - Not Found</title>

    <uc:ErrorPageHeader runat="server" />

</head>
<body>

    <uc:ErrorPageBody runat="server" ID="ErrorBody">
        <ContentTemplate>

            <h1 runat="server" id="ErrorHeading" class="display-5"></h1>
            <p runat="server" id="ErrorBody" class="fs-3 pb-1"></p>
            <asp:Literal runat="server" ID="ErrorHref" />

            <hr class="my-4" />

            <p class="text-body-secondary fs-6 mb-4">
                Return to the Home page 
                or click the back button on your browser.
            </p>

            <p>
                <a href="/" class="btn btn-primary"><i class="fas fa-home me-2"></i> Home</a>
            </p>

        </ContentTemplate>
    </uc:ErrorPageBody>

</body>
</html>

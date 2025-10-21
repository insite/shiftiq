<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" CodeBehind="500.aspx.cs" Inherits="InSite.UI.Lobby.Logs._500" %>

<%@ Register TagPrefix="uc" TagName="ErrorPageHeader" Src="~/UI/Lobby/Controls/ErrorPageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="ErrorPageBody" Src="~/UI/Lobby/Controls/ErrorPageBody.ascx" %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, viewport-fit=cover">

    <title>HTTP 500 - Unexpected Error</title>

    <uc:ErrorPageHeader runat="server" />

    <style type="text/css">
        .debug-body {
            margin-top: 30px;
            padding: 10px;
            max-height: 150px;
            overflow: auto;
            border: dotted 1px #888888;
            font-size: 14px;
            color: #888888;
        }

        .debug-body strong { display: block; font-size: 16px; margin-bottom: 5px; }
    </style>

</head>
<body>

    <uc:ErrorPageBody runat="server" ID="ErrorBody">
        <ContentTemplate>

            <h1 runat="server" id="ErrorHeading" class="display-5">Unexpected Error</h1>
            <p runat="server" id="ErrorBody" class="fs-3 pb-1">Something unexpected happened. We're looking into it for you now.</p>
            <asp:Literal runat="server" ID="ErrorHref" />

            <hr class="my-4" />

            <p class="text-body-secondary fs-6 mb-4">
                Contact your administrator with the steps to reproduce this error, or click the back button on your browser.
            </p>

            <p>
                <a href="/" class="btn btn-primary"><i class="fas fa-home me-2"></i> Home</a>
                <a runat="server" id="SignOutLink" href="/ui/lobby/signout" class="btn btn-default ms-1"><i class="fas fa-sign-out me-2"></i> Sign Out</a>
            </p>

            <div runat="server" id="SystemInformation" class="debug-body" visible="false">
                <strong>System Information:</strong>
                <asp:Literal runat="server" ID="ErrorMessage"></asp:Literal>
            </div>

        </ContentTemplate>
    </uc:ErrorPageBody>

</body>
</html>

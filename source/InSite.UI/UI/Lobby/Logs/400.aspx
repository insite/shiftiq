<%@ Register TagPrefix="uc" TagName="ErrorPageHeader" Src="~/UI/Lobby/Controls/ErrorPageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="ErrorPageBody" Src="~/UI/Lobby/Controls/ErrorPageBody.ascx" %>

<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, viewport-fit=cover">

    <title>HTTP 400 - Bad Request</title>

    <uc:ErrorPageHeader runat="server" />

</head>
<body>

    <uc:ErrorPageBody runat="server">
        <ContentTemplate>

            <h1 class="display-5">Bad Request</h1>
            <p class="fs-3 pb-1">It seems your browser sent a request that this site could not process.</p>

        </ContentTemplate>
    </uc:ErrorPageBody>

</body>
</html>

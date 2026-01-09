<%@ Page Language="C#" CodeBehind="Launch.aspx.cs" Inherits="InSite.UI.Portal.Integrations.Scorm.Launch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shift iQ - Course</title>
    <link rel="stylesheet" media="screen" href="/library/fonts/font-awesome/7.1.0/css/all.min.css" />
    <style type="text/css">
        html, body, form {
            height: 100%;
            margin: 0;
            font-family: "Open Sans", Calibri;
            font-size: 17px;
        }
        iframe {
            display: block;
            background: #ffffff;
            border: none;
            width: 100%;
            height: 100%;
        }
        .header { position: absolute; top: 0px; color: #313537; background-color: #ffffff; height: 22px; padding: 10px 20px 10px 20px; border-bottom: #efefef solid 1px; border-right: #efefef solid 1px; }
        .header a { color: #457897; text-decoration: none; }
        .header a:hover { text-decoration: underline; }
    </style>
</head>
    <body style="margin:0px;padding:0px;">
        <form runat="server">
            <div runat="server" id="NavigationBar" class="header">
                <asp:HyperLink runat="server" ID="ScormCloseLink" Text="&laquo; Return to Portal" />
                <span style='color:#efefef; padding:0 10px 0 10px;'>|</span>
                <asp:Literal runat="server" ID="ScormHeader" />
            </div>
            <div style="height:100%">
                <iframe runat="server" id="ScormContent" src="#" frameborder="0" style="height:100%;width:100%"></iframe>
            </div>
        </form>
    </body>
</html>

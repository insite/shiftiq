<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AuthToken.aspx.cs" Inherits="InSite.UI.Lobby.AuthToken" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body { font-family: 'Courier New'; font-size: 14px; }
        table { border-collapse: collapse; width: 800px; }
        table tr td { padding: 20px; border: solid 1px silver; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <h2>Current Cookie</h2>

            <div>
                <asp:Label runat="server" ID="CookieName" /> Serialized:
                <div>
                    <asp:TextBox runat="server" ID="SerializedCookieValue" TextMode="MultiLine" Rows="7" Width="600px" ReadOnly="true" />
                </div>
            </div>

            <div>
                <div>
                    <span runat="server" id="DeserializedDetail" style="color:dodgerblue; font-weight:bold; "></span>
                </div>
                Deserialized:
                <span runat="server" id="DeserializedSuccess" visible="false" style="color:forestgreen; font-weight:bold;">Success</span>
                <span runat="server" id="DeserializedFailure" visible="false" style="color:red; font-weight:bold;">Failure</span>
                <div>
            <pre><asp:Literal runat="server" ID="DeserializedCookieValue" /></pre>
                </div>
            </div>

            <hr />

            <h2>Validation Test</h2>

            <div>
                Input:
                <div>
                    <asp:TextBox runat="server" ID="TestInput" TextMode="MultiLine" Rows="7" Width="600px" />
                </div>
            </div>

            <div>
                API:
                <div>
                    <asp:TextBox runat="server" ID="TestUrl" Width="600px" />
                </div>
            </div>

            <div style="padding-top:10px; padding-bottom:10px;">
                <asp:Button runat="server" ID="TestCase1" Text="Submit" />
            </div>

            <div>
                <div>
                    <span runat="server" id="TestDetail" style="color:dodgerblue; font-weight:bold; "></span>
                </div>
                Validation:
                <span runat="server" id="TestSuccess" visible="false" style="color:forestgreen; font-weight:bold;">Success</span>
                <span runat="server" id="TestFailure" visible="false" style="color:red; font-weight:bold;">Failure</span>
                <div>
<pre><asp:Literal runat="server" ID="TestOutput" /></pre>
                </div>
            </div>

        </div>
    </form>
</body>
</html>

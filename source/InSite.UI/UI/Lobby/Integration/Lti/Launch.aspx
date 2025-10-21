<%@ Page Language="C#" CodeBehind="Launch.aspx.cs" Inherits="InSite.UI.Lobby.Integration.Lti.Launch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LTI Launch</title>
    <style type="text/css">
    * {
        margin: 0;
        padding: 0;
    }

    body {
        font-family: Calibri, sans-serif;
        color: #444;
    }

    #content {
        padding: 30px;
    }

    p { margin-bottom: 5px; }

    table.parameters {
        border-collapse: collapse;
    }

    table.parameters tr th {
        text-align: left;
    }

    table.parameters tr th, table.parameters tr td {
        padding: 5px;
    }

    table.parameters tr:nth-child(even) {
        background: #eee;
    }

    input[type="text"] {
        width: 700px;
        height: 26px;
        padding-left: 5px;
    }
    input[type="submit"] {
        height: 32px;
        padding: 5px;
    }

    #keys {
        margin-top: 20px;
        padding-top: 20px;
        padding-bottom: 20px;
        border-top: solid 1px #aaa;
    }

    #errors {
        color: red;
    }

    .label-debug {
        font-size: 0.5em;
        color: #d43f3a;
    }

    .button {
        display: block;
        width: 100px;
        height: 22px;
        background: #444;
        padding: 10px;
        text-align: center;
        border-radius: 5px;
        color: white;
        font-weight: bold;
        text-decoration: none;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="content">

            <h1>LTI Launch Request</h1>

            <p>This form implements a simple LTI Provider.</p>

            <table class="parameters">
                <asp:Repeater runat="server" ID="ParameterRepeater">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align:right;"><%# Eval("Key") %> = </td>
                            <td><input type="text" name='<%# Eval("Key") %>' value="<%# Eval("Value") %>" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>

            <div id="errors">
                <asp:Repeater runat="server" ID="ErrorRepeater">
                    <ItemTemplate>
                        <span><%# Container.DataItem %></span>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </div>
    </form>
</body>
</html>

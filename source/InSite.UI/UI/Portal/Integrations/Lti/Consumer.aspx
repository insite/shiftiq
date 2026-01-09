<%@ Page Language="C#" CodeBehind="Consumer.aspx.cs" Inherits="InSite.UI.Portal.Integrations.Lti.Consumer" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<style>

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

    <asp:Panel runat="server" ID="DebugPanel" Visible="false">
        <div id="submitForm">
            <table class="parameters">
                <tr>
                    <th><insite:Literal runat="server" Text="Name" /></th>
                    <th><insite:Literal runat="server" Text="Value" /></th>
                </tr>
                <asp:Repeater runat="server" ID="ParameterRepeater2">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Key") %></td>
                            <td><input type="text" name='<%# Eval("Key") %>' value="<%# Eval("Value") %>" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td colspan="2" align="right">
                        <script type="text/javascript">
                            function submitform() {
                                var formId = "submitForm";
                                var formHtml = "<form id='" + formId + "' action='<%= FormUrl %>' method='post'>" + $("#" + formId)[0].innerHTML + "</form>";

                                $("#" + formId).replaceWith(formHtml);
                                $("#" + formId).submit();
                            }
                        </script>
                        <a class="btn btn-default" href="javascript: submitform()"><i class="fas fa-cloud-upload me-2"></i>POST</a>
                    </td>
                </tr>
            </table>
        </div>

        <div id="keys">
            <asp:Repeater runat="server" ID="ParameterRepeater3">
                <ItemTemplate>
                    <span><%# Eval("Key") %></span>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </asp:Panel>

    <asp:Panel runat="server" ID="NoneDebugPanel" Visible="false">
        <div>
            <p>Redirecting...</p>
            <div id="redirectForm">
                <asp:Repeater runat="server" ID="ParameterRepeater">
                    <ItemTemplate>
                        <input type="hidden" name='<%# Eval("Key") %>' value="<%# Eval("Value") %>" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <script type="text/javascript">
            $(function () {
                var formId = "redirectForm";
                var formHtml = "<form id='" + formId + "' action='<%= FormUrl %>' method='post'>" + $("#" + formId)[0].innerHTML + "</form>";

                $("#" + formId).replaceWith(formHtml);
                $("#" + formId).submit();
            });
        </script>
    </asp:Panel>

</asp:Content>

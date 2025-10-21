<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonRecords.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonRecords" %>

<div class="card">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoGradebooks" Text="No gradebooks to display." />

        <insite:GradebookComboBox runat="server" ID="GradebookComboBox" CssClass="w-50" AllowBlank="false" />

        <asp:Repeater runat="server" ID="ScoreRepeater">
            <HeaderTemplate>
                <table class="table table-striped mt-3">
            </HeaderTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td style='<%# Eval("GradeItemNameStyle") %>'>
                        <span><%# Eval("GradeItemName") %></span>
                    </td>
                    <td class="text-end">
                        <span class='<%# Eval("ProgressStatusClass") %>'>
                            <%# Eval("ProgressStatus") %>
                        </span>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>
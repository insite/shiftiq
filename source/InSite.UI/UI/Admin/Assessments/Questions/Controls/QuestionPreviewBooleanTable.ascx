<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewBooleanTable.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewBooleanTable" %>

<div class="form-group boolean-list">
    <table class="table table-hover table-boolean">
        <asp:Repeater runat="server" ID="HeaderRepeater">
            <HeaderTemplate>
                <thead><tr>
            </HeaderTemplate>
            <FooterTemplate>
                <th></th></tr></thead>
            </FooterTemplate>
            <ItemTemplate>
                <th class='<%# Eval("CssClass") %>' style='<%# Eval("Style") %>'>
                    <div><%# Eval("Html") %></div>
                </th>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater runat="server" ID="RowRepeater">
            <HeaderTemplate><tbody></HeaderTemplate>
            <FooterTemplate></tbody></FooterTemplate>
            <ItemTemplate>
                <tr>
                    <asp:Repeater runat="server" ID="CellRepeater">
                        <ItemTemplate>
                            <td class='<%# Eval("CssClass") %>' style='<%# Eval("Style") %>'><%# Eval("Html") %></td>
                        </ItemTemplate>
                    </asp:Repeater>
                    <td class="input">
                        <insite:RadioButton runat="server" Text="True" Checked='<%# (bool)Eval("IsTrue") %>' GroupName='<%# Eval("GroupName") %>' />
                        <insite:RadioButton runat="server" Text="False" Checked='<%# !(bool)Eval("IsTrue") %>' GroupName='<%# Eval("GroupName") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>

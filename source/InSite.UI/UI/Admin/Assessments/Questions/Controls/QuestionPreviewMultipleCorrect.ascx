<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewMultipleCorrect.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewMultipleCorrect" %>

<div class="form-group checkbox-list">
    <table class="table table-option">
        <asp:Repeater runat="server" ID="HeaderRepeater">
            <HeaderTemplate>
                <thead><tr><th></th>
            </HeaderTemplate>
            <FooterTemplate>
                </tr></thead>
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
                    <td class="input">
                        <insite:CheckBox runat="server" Checked='<%# (bool)Eval("IsTrue") %>' />
                    </td>
                    <asp:Repeater runat="server" ID="CellRepeater">
                        <ItemTemplate>
                            <td class='<%# Eval("CssClass") %>' style='<%# Eval("Style") %>'><%# Eval("Html") %></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>

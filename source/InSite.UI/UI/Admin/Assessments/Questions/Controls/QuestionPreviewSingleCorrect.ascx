<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewSingleCorrect.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewSingleCorrect" %>

<div class="form-group radio-list">
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
                        <insite:RadioButton runat="server" Checked='<%# (bool)Eval("HasPoints") %>' GroupName='<%# Eval("GroupName") %>' />
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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewLikert.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionPreviewLikert" %>

<div class="form-group likert-matrix">
    <table class="table table-striped table-likert" style="visibility:hidden;">
        <thead>
            <asp:Repeater runat="server" ID="LikertColumnRepeater">
                <HeaderTemplate>
                    <tr>
                        <td></td>
                </HeaderTemplate>
                <ItemTemplate>
                    <td class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                </ItemTemplate>
                <FooterTemplate>
                    </tr>
                </FooterTemplate>
            </asp:Repeater>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="LikertRowRepeater">
                <ItemTemplate>
                    <tr>
                        <td class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                        <asp:Repeater runat="server" ID="LikertOptionRepeater">
                            <ItemTemplate>
                                <td class="input">
                                    <insite:RadioButton runat="server" CssClass="d-inline-block" Checked='<%# IsOptionChecked() %>' GroupName='<%# GetOptionGroup() %>' />
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewMatching.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewMatching" %>

<div class="form-group match-list">
    <table class="table table-hover table-match">
        <tbody>
            <asp:Repeater runat="server" ID="RowRepeater">
                <ItemTemplate>
                    <tr>
                        <td class="left"><%# Eval("Left") %></td>
                        <td class="right">
                            <insite:ComboBox runat="server" ID="Selector" Width="100%" AllowBlank="false" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>

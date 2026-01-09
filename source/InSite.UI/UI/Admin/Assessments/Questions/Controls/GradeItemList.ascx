<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeItemList.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.GradeItemList" %>

<asp:Repeater runat="server" ID="List">
    <HeaderTemplate>
        <table class="table table-striped">
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:30%;">
                <%# Eval("FormName") %>
            </td>
            <td style="width:70%;">
                <asp:Literal runat="server" ID="FormID" Visible="false" Text='<%# Eval("FormId") %>' />
                <insite:GradebookItemComboBox runat="server" ID="GradeItem" />

                <asp:Repeater runat="server" ID="LikertRowRepeater">
                    <HeaderTemplate>
                        <table class="table table-striped mt-2">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="w-50">
                                <%# Container.ItemIndex + 1 %>.
                                <%# Eval("QuestionText") %>
                            </td>
                            <td class="w-50">
                                <%# Eval("GradeItemCode") %>
                                <%# Eval("GradeItemName") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
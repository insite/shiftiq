<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentChecklist.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.DepartmentChecklist" %>

<asp:Panel runat="server" ID="MainPanel">
    <div runat="server" id="OwnerNotice" visible="false" class="alert alert-info mb-3">
        This achievement is owned by another organization, so it cannot be added to departments in
        this organization. Only the departments that already hold it are listed, so you can remove
        it from them.
    </div>
    <asp:DataList runat="server" ID="DepartmentsRepeater" RepeatLayout="Table" RepeatDirection="Vertical" RepeatColumns="3">
        <HeaderTemplate>
            <table>
        </HeaderTemplate>
        <ItemTemplate>
            <div style="padding-right:20px;">
                <asp:Literal runat="server" ID="DepartmentIdentifier" Text='<%# Eval("DepartmentIdentifier") %>' Visible="false" />
                <insite:CheckBox runat="server" ID="IsDepartmentSelected" AutoPostBack="true" Checked='<%# Eval("IsChecked") %>' Text='<%# Eval("DepartmentName") %>' />
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:DataList>
</asp:Panel>

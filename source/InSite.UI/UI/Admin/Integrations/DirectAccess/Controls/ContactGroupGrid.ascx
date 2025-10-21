<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactGroupGrid.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls.ContactGroupGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="Thumbprint">
    <Columns>

        <asp:BoundField HeaderText="Group Type" DataField="GroupType" />
        <asp:HyperLinkField HeaderText="Group Name" DataTextField="GroupName" 
            DataNavigateUrlFields="Thumbprint" DataNavigateUrlFormatString="/ui/admin/contacts/groups/edit?contact={0}" />
        <asp:BoundField HeaderText="Group Size" DataField="GroupSize" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>

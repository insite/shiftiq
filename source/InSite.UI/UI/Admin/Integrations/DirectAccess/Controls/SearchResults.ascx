<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="IndividualKey">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40">
            <ItemTemplate>
                <insite:IconLink Name="search" ToolTip="View Individual" runat="server" NavigateUrl='<%# string.Format("/ui/admin/integrations/directaccess/view?individual={0}", Eval("IndividualKey")) %>' />
                <insite:IconButton runat="server" Name="plus-circle" ToolTip="Add New Person" Visible='<%# Eval("RefreshedBy") == null %>' OnClientClick="return confirm('Are you sure to add new person')" CommandName="NewPerson" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Individual Key" DataField="IndividualKey" />
        <asp:BoundField HeaderText="First Name" DataField="FirstName" />
        <asp:BoundField HeaderText="Last Name" DataField="LastName" />
        <asp:BoundField HeaderText="Email" DataField="Email" />
        <asp:BoundField HeaderText="City" DataField="AddressCity" />

        <asp:TemplateField HeaderText="Refreshed">
            <ItemTemplate>
                <%# TimeZones.Format((DateTimeOffset)Eval("Refreshed"), TimeZones.Utc) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:HyperLinkField HeaderText="Number" DataTextField="ProfileNumber" DataNavigateUrlFields="ProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" />
        <asp:HyperLinkField HeaderText="Name" DataTextField="ProfileTitle" DataNavigateUrlFields="ProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" />
        <asp:HyperLinkField HeaderText="Parent" DataTextField="ParentProfileLabel" DataNavigateUrlFields="ParentProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" />
        <asp:BoundField HeaderText="Children" DataField="ChildCount" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
        <asp:TemplateField HeaderText="Diverged">
            <ItemTemplate>
                <%# Convert.ToBoolean(Eval("IsDiverged")) ? "<span class=\"text-info\">Diverged</span>" : "" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Competencies" DataField="CompetencyCount" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
        <asp:BoundField HeaderText="People" DataField="AcquiredCount" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
    </Columns>
</insite:Grid>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileSearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:HyperLinkField HeaderText="Number" DataTextField="ProfileNumber" DataNavigateUrlFields="ProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:HyperLinkField HeaderText="Name" DataTextField="ProfileTitle" DataNavigateUrlFields="ProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" />
        <asp:BoundField HeaderText="# of Competencies" DataField="CompetencyCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="# Acquired" DataField="AcquiredCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
    </Columns>
</insite:Grid>
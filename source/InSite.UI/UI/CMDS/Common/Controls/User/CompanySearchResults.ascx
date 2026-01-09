<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanySearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.CompanySearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:HyperLinkField HeaderText="Name" DataTextField="CompanyTitle" DataNavigateUrlFields="OrganizationIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/organizations/edit?id={0}" />
        <asp:BoundField HeaderText="Departments" DataField="DepartmentCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="People" DataField="PersonCount" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Profiles" DataField="ProfileCount" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Competencies" DataField="CompetencyCount" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Achievements" DataField="AchievementCount" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
    </Columns>
</insite:Grid>
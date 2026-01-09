<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="DepartmentIdentifier,ProfileStandardIdentifier,UserIdentifier">
    <Columns>
        <asp:BoundField HeaderText="Organization" DataField="OrganizationName" />
        <asp:BoundField HeaderText="Department" DataField="DepartmentName" />
        <asp:BoundField HeaderText="Profile" DataField="ProfileName" />
        <asp:BoundField HeaderText="Person" DataField="UserFullname" />

        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit Department Profile User'
                    NavigateUrl='<%# string.Format("/ui/cmds/admin/departments/profile-users/edit?department={0}&profile={1}&user={2}", Eval("DepartmentIdentifier"), Eval("ProfileStandardIdentifier"), Eval("UserIdentifier")) %>' />

                <insite:IconButton runat="server" Name="trash-alt" ToolTip="Remove Department Profile User" 
                    CommandName="DeleteRecord"
                    ConfirmText="Are you sure you want to remove this department profile user?" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
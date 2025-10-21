<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencySearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Talents.Competencies.CompetencySearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:HyperLinkField HeaderText="Number" DataTextField="Number" DataNavigateUrlFields="CompetencyStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/competencies/edit?id={0}"/>
            
        <asp:BoundField HeaderText="Summary" DataField="Summary" />

        <asp:TemplateField HeaderText="Category">
            <ItemTemplate>
                <%# GetCategories((Guid)Eval("StandardIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Organizations" DataField="StandardOrganizationCount" DataFormatString="{0:n0}" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />
        <asp:BoundField HeaderText="Users" DataField="StandardValidationCount" DataFormatString="{0:n0}" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>

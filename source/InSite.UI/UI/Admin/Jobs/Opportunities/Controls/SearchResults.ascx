<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Jobs.Opportunities.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="OpportunityIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/admin/jobs/opportunities/edit?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:BoundField HeaderText="Employer Name" DataField="EmployerGroupName" />
        <asp:BoundField HeaderText="Job Position" DataField="JobTitle" />
        <asp:BoundField HeaderText="Occupation Area" DataField="OccupationAreaName" />
        <asp:BoundField HeaderText="Job Location" DataField="LocationName" />
        <asp:BoundField HeaderText="Job Type" DataField="LocationType" />

        <asp:TemplateField HeaderText="Published">
            <ItemTemplate>
                <%# LocalizeTime((DateTimeOffset?)Eval("WhenPublished")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# LocalizeTime((DateTimeOffset)Eval("WhenCreated")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Modified">
            <ItemTemplate>
                <%# LocalizeTime((DateTimeOffset?)Eval("WhenModified")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

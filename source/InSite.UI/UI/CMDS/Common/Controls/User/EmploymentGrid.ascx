<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmploymentGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Employees.Employments.EmploymentGrid" %>

<insite:Container runat="server" ID="NoPrimaryProfilePanel">
    <p>
        This person has not yet acquired a primary profile.
    </p>
    
    <insite:Button runat="server" ID="EmployeeProfileEditorLink1" Text="Assign a primary profile" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</insite:Container>

<insite:Container runat="server" ID="PrimaryProfilePanel">
    <asp:Panel runat="server" ID="EmployeeProfileEditorLink2Panel" CssClass="mb-3">
        <insite:Button runat="server" ID="EmployeeProfileEditorLink2" Text="Edit profiles acquired by this person" Icon="fas fa-pencil" ButtonStyle="Default" />
    </asp:Panel>
    
    <insite:Grid runat="server" ID="Grid" DataKeyNames="OrganizationIdentifier">
        <Columns>
                        
            <asp:BoundField HeaderText="Organization" DataField="CompanyName" />
            <asp:BoundField HeaderText="Department" DataField="DepartmentName" />

            <asp:TemplateField HeaderText="Profile" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <a runat="server" href='<%# "/ui/cmds/portal/validations/competencies/search?profile=" + Eval("ProfileStandardIdentifier") + "&userID=" + Eval("PersonID") + "&department=" + Eval("DepartmentIdentifier") %>'>
                        <%# Eval("ProfileNumber") %>: <%# Eval("ProfileTitle") %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="15px" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <insite:IconButton runat="server" Name="trash-alt" ToolTip="Remove" CommandName="Delete" OnClientClick="return confirm('Delete this employment?')" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</insite:Container>
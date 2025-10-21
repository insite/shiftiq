<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileCompetencyList.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileCompetencyList" %>

<insite:Nav runat="server">

    <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies">
        <asp:Repeater ID="AddedCompetencies" runat="server">
            <HeaderTemplate>
                <table id='<%# AddedCompetencies.ClientID %>'><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td class="p-1">
                        <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                        <asp:CheckBox runat="server" ID="Competency" />
                    </td>
                    <td class="p-1 text-nowrap">
                        <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                    </td>
                    <td class="p-1">
                        <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <div runat="server" id="CompetencyButtons" class="mt-3">
            <insite:Button runat="server" ID="SelectAllButton" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
            <insite:Button runat="server" ID="UnselectAllButton" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
            <insite:DeleteButton ID="DeleteCompetencyButton" runat="server" />
        </div>

        <div class="form-text mt-3">
            Don't forget to assign priorities and levels to the competencies you add to this profile.
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="NewCompetencyTab" Title="Add Competencies">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NewCompetencyUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="NewCompetencyUpdatePanel">
            <Triggers>
                <asp:PostBackTrigger ControlID="AddCompetencyButton" />
            </Triggers>
            <ContentTemplate>

                <div class="row">
                    <div class="col-lg-6">
                        <div class="mb-3">
                            Search for the competencies to add to this profile, 
                            then check the box next to each one and click 'Add'.
                        </div>

                        <div class="mb-3">
                            <cmds:FindProfile ID="SearchProfile" runat="server" EmptyMessage="Profile" />
                        </div>

                        <div class="mb-3">
                            <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Competency Number or Summary" />
                        </div>

                        <div class="mt-3">
                            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="OutlinePrimary" />
                            <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="OutlinePrimary" />
                        </div>

                    </div>
                </div>
    
                <div runat="server" id="FoundCompetency" visible="false" class="my-3">
                </div>
        
                <insite:Container ID="CompetencyList" runat="server" Visible="false">
                    <asp:Repeater ID="NewCompetencies" runat="server">
                        <HeaderTemplate>
                            <table id='<%# NewCompetencies.ClientID %>'><tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="p-1">
                                    <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                    <asp:CheckBox ID="Competency" runat="server" />
                                </td>
                                <td class="p-1 text-nowrap">
                                    <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                </td>
                                <td class="p-1">
                                    <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
        
                    <div class="mt-3">
                        <insite:Button runat="server" ID="SelectAllButton2" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="UnselectAllButton2" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="AddCompetencyButton" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" />
                    </div>
                </insite:Container>

            </ContentTemplate>
        </insite:UpdatePanel>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="AddMultipleQualificationsTab" Title="Add Multiple Competencies">
        <div class="row">
            <div class="col-lg-6">

                <div class="mb-3">
                    Enter the competency numbers you wish to add to this profile, then click 'Add'
                </div>
    
                <div>
                    <insite:TextBox runat="server" ID="MultipleCompetencyNumbers" TextMode="MultiLine" Rows="10" />
                </div>

                <div class="mt-3">
                    <insite:Button runat="server" ID="AddMultipleButton" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="OutlineSuccess" />
                </div>
            </div>
        </div>
    </insite:NavItem>

</insite:Nav>

<script type="text/javascript">

    function setCheckboxes(containerId, checked)
    {
        var checkboxes = document.getElementById(containerId).getElementsByTagName("input");

        for (var i = 0; i < checkboxes.length; i++)
        {
            if (checkboxes[i].type.toLowerCase() == "checkbox")
                checkboxes[i].checked = checked;
        }

        return false;
    }

</script>
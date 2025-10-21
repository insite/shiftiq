<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactProfileEditor.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.ListEditors.ContactProfileEditor" %>

<insite:Nav runat="server">

    <insite:NavItem runat="server" ID="ProfileTab" Title="Profiles">

        <asp:Repeater ID="AddedProfiles" runat="server">
            <HeaderTemplate>
                <table id='<%# AddedProfiles.ClientID %>'><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</tr>": "" %>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                    <td class="p-1">
                        <asp:Literal ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
                        <asp:CheckBox runat="server" ID="ProfileSelected" />
                    </td>
                    <td class="p-1 text-nowrap">
                        <a href='<%# "/ui/cmds/admin/standards/profiles/edit?id=" + Eval("ProfileStandardIdentifier") + "&organization=" + OrganizationIdentifier + "&department=" + DepartmentIdentifier %>'><%# Eval("ProfileNumber") %></a>
                    </td>
                    <td class="p-1 pe-4">
                        <asp:Label runat="server" ID="Title" AssociatedControlID="ProfileSelected" Text='<%# Eval("ProfileTitle") %>' />
                    </td>
                <%# (Container.ItemIndex % 2) != 0 ? "</tr>": "" %>
            </ItemTemplate>
        </asp:Repeater>
               
    
        <div class="mt-3">
            <insite:Button runat="server" ID="SelectAllButton" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
            <insite:Button runat="server" ID="UnselectAllButton" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
            <insite:DeleteButton ID="DeleteProfileButton" runat="server" />
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="NewProfileTab" Title="Add Profiles">

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NewProfileUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="NewProfileUpdatePanel">
            <Triggers>
                <asp:PostBackTrigger ControlID="AddProfileButton" />
                <asp:PostBackTrigger ControlID="AddProfileCopyButton" />
            </Triggers>
            <ContentTemplate>

                <div class="row">
                    <div class="col-lg-6">
                        <div class="mb-3">
                            Search for the profiles you want to add to this <asp:Literal ID="EntityName1" runat="server" />. 
                            Check the box next to each one and click the Add button. This will assign the profile to the 
                            <asp:Literal ID="EntityName2" runat="server" />.
    
                            <asp:PlaceHolder ID="CopyMessage" runat="server">
                            You can click the Add Copy button to create a copy of the profile (including the competencies 
                            assigned to it) and assign the copied profile to this <asp:Literal ID="EntityName3" runat="server" />.
                            </asp:PlaceHolder>
                        </div>

                        <div>
                            <insite:TextBox ID="SearchText" runat="server" />
                        </div>

                        <div class="mt-3">
                            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="OutlinePrimary" />
                            <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="OutlinePrimary" />
                        </div>

                    </div>
                </div>
    
                <div runat="server" id="FoundProfile" visible="false" class="my-3">
                </div>
        
                <insite:Container ID="ProfileList" runat="server" Visible="false">
                    <asp:Repeater ID="NewProfiles" runat="server">
                        <HeaderTemplate>
                            <table id='<%# NewProfiles.ClientID %>'><tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                                <td class="p-1">
                                    <asp:Literal ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
                                    <asp:CheckBox ID="ProfileSelected" runat="server" />
                                </td>
                                <td class="p-1 text-nowrap">
                                    <a href='<%# "/ui/cmds/admin/standards/profiles/edit?id=" + Eval("ProfileStandardIdentifier") + "&department=" + DepartmentIdentifier + "&organization=" + OrganizationIdentifier %>'><%# Eval("ProfileNumber") %></a>
                                </td>
                                <td class="p-1 pe-4">
                                    <asp:Label runat="server" ID="Title" AssociatedControlID="ProfileSelected" Text='<%# Eval("ProfileTitle") %>' />
                                </td>
                            <%# (Container.ItemIndex % 2) != 0 ? "<tr>": "" %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</tr>": "" %>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
        
                    <div class="mt-3">
                        <insite:Button runat="server" ID="SelectAllButton2" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="UnselectAllButton2" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="AddProfileButton" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" />
                        <insite:Button runat="server" ID="AddProfileCopyButton" Icon="fas fa-plus-circle" Text="Add Copy" ButtonStyle="Success" 
                            ConfirmText="Are you sure you want to add copies of selected profiles?" />
                    </div>
                </insite:Container>

            </ContentTemplate>
        </insite:UpdatePanel>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="AddMultipleProfilesTab" Title="Add Multiple Profiles">
        <div class="row">
            <div class="col-lg-6">
                <div class="mb-3">
                    Enter the list of profile numbers you want to add to this
                    <asp:Literal ID="EntityName4" runat="server" />
                    then click the Add button.
                </div>
    
                <div>
                    <insite:TextBox runat="server" ID="MultipleProfileNumbers" TextMode="MultiLine" Rows="10" />
                </div>

                <div class="mt-3">
                    <insite:Button runat="server" ID="AddMultipleButton" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="OutlineSuccess" />
                </div>
            </div>
        </div>
    </insite:NavItem>

</insite:Nav>
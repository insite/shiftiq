<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupOccupations.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.GroupOccupations" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <insite:Nav runat="server">

            <insite:NavItem runat="server" ID="ProfileTab" Title="Profiles">
                <div runat="server" id="ProfileTable" class="mb-3">
                    <asp:Repeater ID="AddedProfiles" runat="server">
                        <HeaderTemplate><table class="table-profiles"><tbody></HeaderTemplate>
                        <ItemTemplate>
                            <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                                <td class="std-chk">
                                    <asp:CheckBox runat="server" ID="OccupationSelected" />
                                </td>
                                <td class="std-code text-nowrap">
                                    <a href='<%# "/ui/admin/standards/edit?id=" + Eval("StandardIdentifier") + "&group=" + DepartmentIdentifier %>'><%# Eval("Code") %></a>
                                </td>
                                <td class="std-title ps-1 pe-3">
                                    <div class="text-truncate">
                                        <%# Eval("ContentTitle") %>
                                    </div>
                                </td>
                            <%# ((Container.ItemIndex + 1) % 2) == 0 ? "</tr>": "" %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</tr>": "" %></tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div>
                    <insite:Button runat="server" ID="AddedProfilesSelectAllButton" 
                        ButtonStyle="OutlineSecondary" Icon="far fa-check-square" Text="Select All" />
                    <insite:Button runat="server" ID="AddedProfilesUnselectAllButton" 
                        ButtonStyle="OutlineSecondary" Icon="far fa-square" Text="Deselect All" />
                    &nbsp;
                    <insite:DeleteButton runat="server" ID="AddedProfilesDeleteButton" ButtonStyle="OutlineDanger" />
                </div>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="NewProfileTab" Title="Search and Add">
    
                <div class="row">
                    <div class="col-lg-6 mb-3">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Occupation Title or Code
                            </label>
                            <insite:TextBox runat="server" ID="NewProfileSearchText" />
                        </div>
                        
                        <div class="mt-3">
                            <insite:FilterButton runat="server" ID="NewProfileFilterButton" ButtonStyle="OutlinePrimary" />
                            <insite:ClearButton runat="server" ID="NewProfileClearButton" ButtonStyle="OutlineSecondary" />
                        </div>
                    </div>

                    <div runat="server" id="NewProfileResultColumn" class="col-lg-12">
                        <p runat="server" id="NewProfileResultMessage" visible="false"></p>

                        <div runat="server" id="NewProfileTable" class="mb-3">
                            <asp:Repeater ID="NewProfiles" runat="server">
                                <HeaderTemplate>
                                    <table class="table-profiles">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                                        <td class="std-chk">
                                            <asp:CheckBox ID="OccupationSelected" runat="server" />
                                        </td>
                                        <td class="std-code text-nowrap">
                                            <a href='<%# "/ui/cmds/admin/standards/profiles/edit?id=" + Eval("StandardIdentifier") + "&department=" + DepartmentIdentifier %>'><%# Eval("Code") %></a>
                                        </td>
                                        <td class="std-title ps-1 pe-3">
                                            <div class="text-truncate">
                                                <%# Eval("ContentTitle") %>
                                            </div>
                                        </td>
                                    <%# ((Container.ItemIndex + 1) % 2) == 0 ? "</tr>": "" %>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</tr>": "" %>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <div>
                            <insite:Button runat="server" ID="NewProfileSelectAllButton" 
                                ButtonStyle="OutlineSecondary" Icon="far fa-check-square" Text="Select All" />
                            <insite:Button runat="server" ID="NewProfileUnselectAllButton" 
                                ButtonStyle="OutlineSecondary" Icon="far fa-square" Text="Deselect All" />
                            &nbsp;
                            <insite:Button runat="server" ID="NewProfileAddButton" 
                                ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Add" />
                            <insite:Button runat="server" ID="NewProfileAddCopyButton" 
                                ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Add Copy" 
                                OnClientClick="return confirm('Are you sure you want to add copies of selected profiles?')" />
                        </div>
                    </div>
                </div>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="NewBulkProfilesTab" Title="Bulk Add">
    
                <div class="row">
                    <div class="col-lg-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Occupation Code(s)
                            </label>
                            <insite:TextBox runat="server" ID="NewBulkProfileNumbers" TextMode="MultiLine" Rows="4" />
                        </div>
        
                        <div>
                            <insite:Button runat="server" ID="NewBulkAddButton" 
                                ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Add" />
                        </div>
                    </div>
                </div>

            </insite:NavItem>

        </insite:Nav>
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style>
        table.table-profiles {
            width: 100%;
        }

            table.table-profiles td.std-chk {
                width: 27px;
            }

            table.table-profiles td.std-code {
                width: 2%;
            }

            table.table-profiles td.std-title {
                max-width: 0;
                cursor: default;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('table.table-profiles td.std-title')
                    .off('click', onTitleClick)
                    .on('click', onTitleClick);
            });

            function onTitleClick(e) {
                e.preventDefault();
                $(this).closest('td').prevAll('.std-chk:first').find('input[type="checkbox"]').click();
            }
        })();
    </script>
</insite:PageFooterContent>
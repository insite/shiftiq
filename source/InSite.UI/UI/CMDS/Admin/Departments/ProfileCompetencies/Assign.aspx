<%@ Page Language="C#" CodeBehind="AssignDepartment.ascx.cs" Inherits="InSite.Cmds.Admin.Workflows.Profiles.Forms.AssignDepartment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="AssignProfiles" />

    <asp:CustomValidator runat="server" ID="ProfilesValidator" ErrorMessage="There are no selected profiles" ValidationGroup="AssignProfiles" Display="None" />
    <asp:CustomValidator runat="server" ID="DepartmentsValidator" ErrorMessage="There are no selected departments" ValidationGroup="AssignProfiles" Display="None" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Profile &amp; Status
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">
                    <div class="col-md-6 mb-3 mb-md-0">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ProfileUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ProfileUpdatePanel">
                            <ContentTemplate>

                                <h3>Profiles</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Profile Number
                                    </label>
                                    <insite:TextBox ID="ProfileNumber" runat="server" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Profile Name
                                    </label>
                                    <insite:TextBox ID="ProfileTitle" runat="server" />
                                </div>

                                <div class="my-3">
                                    <insite:FilterButton ID="FilterButton" runat="server" />
                                    <insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                    <insite:Button ID="UnselectAllButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                                </div>

                                <asp:Repeater ID="Profiles" runat="server">
                                    <HeaderTemplate>
                                        <table id='<%# Profiles.ClientID %>' class="mt-3"><tbody>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:Label ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
                                                <asp:CheckBox ID="Selected" runat="server" Text='<%# Eval("ProfileNumber") + ": " + Eval("ProfileTitle") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                    <div class="col-md-6">

                        <h3>Departments</h3>
                        
                        <asp:Repeater ID="Departments" runat="server">
                            <HeaderTemplate>
                                <table><tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="DepartmentIdentifier" runat="server" Text='<%# Eval("DepartmentIdentifier") %>' Visible="false" />
                                        <asp:CheckBox ID="Selected" runat="server" Text='<%# Eval("DepartmentName") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="AssignProfiles" 
            ConfirmText="Are you sure you want to assign selected profiles to selected departments?" />
        <insite:CancelButton runat="server" NavigateUrl="/ui/admin/tools" />
    </div>

</asp:Content>

<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.View" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/ProfileHierarchy.ascx" TagName="ProfileHierarchy" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <div class="float-end">
        <insite:Button runat="server" ID="PrintButton" ButtonStyle="Default" Icon="fas fa-print" Text="Print" />
    </div>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profile" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row">
                        <div class="col-lg-6 mb-3 mb-lg-0">

                            <div class="card h-100">
                                <div class="card-body">

                                    <h3>Details</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Number
                                        </label>
                                        <insite:TextBox ID="Number" runat="server" ReadOnly="true" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Name
                                        </label>
                                        <insite:TextBox ID="TitleInput" runat="server" ReadOnly="true" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Description
                                        </label>
                                        <insite:TextBox ID="Description" runat="server" TextMode="MultiLine" ReadOnly="true" />
                                    </div>

                                </div>
                            </div>

                        </div>
                        <div class="col-lg-6 mb-3 mb-lg-0">

                            <div class="card h-100">
                                <div class="card-body">

                                    <h3>Profile Visibility</h3>

                                    <uc:ProfileHierarchy ID="ProfileHierarchy" runat="server" />

                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <asp:Repeater ID="Competencies" runat="server">
                        <HeaderTemplate>
                            <table class="competencies"><tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="p-2">
                                    <%# Eval("Number") %>
                                </td>
                                <td class="p-2">
                                    <asp:Label runat="server" Text='<%# Eval("Summary") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PeopleTab" Title="People" Icon="far fa-user" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div id="NoPersonPanel" runat="server">
                        There are no employees who acquired this profile.
                    </div>

                    <div id="PersonPanel" runat="server">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PersonUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="PersonUpdatePanel">
                            <ContentTemplate>
                                <uc:PersonGrid ID="PersonGrid" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

</asp:Content>
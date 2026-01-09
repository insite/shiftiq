<%@ Page Language="C#" CodeBehind="Compare.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.Compare" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.table-differences > tbody > tr:not(.no-accent):hover > td {
            background-color: rgba(0, 0, 0, 0.0375);
            color: #737491;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section runat="server" ID="CompareSection" class="mb-3">
        
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Compare
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>
                        <table class="w-100 table-differences">
                            <tbody>

                                <tr class="no-accent">
                                    <td class="w-50 align-top pe-2">

                                        <div class="d-inline-block align-middle me-2" style="width:calc(100% - 160px);">
                                            <cmds:FindProfile ID="Profile1" runat="server" /> 
                                        </div>
                                    
                                        <insite:Button runat="server" ID="EditProfile1" Text="Edit Profile" Icon="fas fa-pencil" Enabled="false" ButtonStyle="Default" Size="Default" />

                                        <div runat="server" id="ProfileDetails1" class="my-3">
                                            <asp:Literal ID="CompetencyCount1" runat="server" />
                                        </div>

                                    </td>
                                    <td class="w-50 align-top ps-2" colspan="2">

                                        <div class="d-inline-block align-middle me-2" style="width:calc(100% - 160px);">
                                            <cmds:FindProfile ID="Profile2" runat="server"  /> 
                                        </div>

                                        <insite:Button runat="server" ID="EditProfile2" Text="Edit Profile" Icon="fas fa-pencil" Enabled="false" ButtonStyle="Default" Size="Default" />

                                        <div runat="server" id="ProfileDetails2" class="my-3">
                                            <asp:Literal ID="CompetencyCount2" runat="server" />
                                        </div>

                                    </td>
                                </tr>

                                <asp:Repeater ID="Differences" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="p-2">
                                                <%# Eval("LeftText") %>
                                            </td>
                                            <td class="text-nowrap align-top p-2" style="width:16px;">
                                                <cmds:IconButton ID="DeleteCompetencyButton" runat="server"
                                                    Visible='<%# !(bool)Eval("IsRightEmpty") && !IsRightProfileLocked %>'
                                                    IsFontIcon="true" CssClass="trash-alt"
                                                    CommandName="DeleteCompetency"
                                                    CommandArgument='<%# Eval("CompetencyStandardIdentifier") %>'
                                                    ToolTip="Delete competency"
                                                    ConfirmText="Are you sure you want to delete competency from this profile?"
                                                />
                                                <cmds:IconButton ID="AddCompetencyButton" runat="server"
                                                    Visible='<%# (bool)Eval("IsRightEmpty") && Eval("RightText") != null && !IsLeftProfileLocked %>'
                                                    IsFontIcon="true" CssClass="plus-circle"
                                                    CommandName="AddCompetency"
                                                    CommandArgument='<%# Eval("CompetencyStandardIdentifier") %>'
                                                    ToolTip="Add competency"
                                                />
                                            </td>
                                            <td class="align-top p-2" >
                                                <%# Eval("RightText") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Repeater ID="Similarities" runat="server">
                                    <HeaderTemplate>
                                        <tr class="no-accent">
                                            <td colspan="3">
                                                <hr />
                                            </td>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="p-2">
                                                <%# Eval("LeftText") %>
                                            </td>
                                            <td class="text-nowrap align-top p-2" style="width:16px;">
                                                <cmds:IconButton ID="DeleteCompetencyButton" runat="server"
                                                    Visible='<%# !IsLeftProfileLocked && !IsRightProfileLocked %>'
                                                    IsFontIcon="true" CssClass="trash-alt"
                                                    CommandName="DeleteCompetency"
                                                    CommandArgument='<%# Eval("CompetencyStandardIdentifier") %>'
                                                    ToolTip="Delete competency"
                                                    ConfirmText="Are you sure you want to delete competency from this profile?"
                                                />
                                            </td>
                                            <td class="align-top p-2" >
                                                <%# Eval("RightText") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </tbody>
                        </table>

                        <div class="mt-3">
                            <asp:Literal ID="CountText" runat="server" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

</asp:Content>

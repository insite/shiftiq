<%@ Page Language="C#" CodeBehind="Contacts.aspx.cs" Inherits="InSite.UI.Portal.Home.Contacts" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <dl runat="server" id="LeadersPanel" class="row">
        <dt class="col-sm-3">Leaders</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="LeaderRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-4"><%# Eval("Phone") %></dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

    <dl runat="server" ID="ManagersPanel" class="row">
        <dt class="col-sm-3">Managers</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="ManagerRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-4"><%# Eval("Phone") %></dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

    <dl runat="server" ID="SupervisorsPanel" class="row">
        <dt class="col-sm-3">Supervisors</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="SupervisorRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-4"><%# Eval("Phone") %></dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

    <dl runat="server" ID="ValidatorsPanel" class="row">
        <dt class="col-sm-3">Validators</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="ValidatorRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-4"><%# Eval("Phone") %></dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

    <dl runat="server" ID="SubordinatesPanel" class="row">
        <dt class="col-sm-3">Direct Reports</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="SubordinateRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-3"><%# Eval("Phone") %></dd>
                        <dd class="col-sm-1">
                            <a runat="server" id="HomeAnchor" target="_blank" href='#'><i class="far fa-home"></i></a>
                        </dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

    <dl runat="server" ID="LearnersPanel" class="row">
        <dt class="col-sm-3">Learners</dt>
        <dd class="col-sm-9">
            <dl class="row">
                <asp:Repeater ID="LearnerRepeater" runat="server">
                    <ItemTemplate>
                        <dt class="col-sm-4"><%# Eval("FullName") %></dt>
                        <dd class="col-sm-4"><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></dd>
                        <dd class="col-sm-4"><%# Eval("Phone") %></dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </dd>
    </dl>

</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="CurrentUserSessions.aspx.cs" Inherits="InSite.UI.Admin.Reporting.CurrentUserSessions" %>

<%@ Register Src="~/UI/Admin/Reporting/Controls/DashboardUsers.ascx" TagName="DashboardUsers" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section runat="server" id="CurrentUserSessions" class="pb-5 mb-md-2">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:DashboardUsers runat="server" ID="DashboardUsers" />
            </div>
        </div>
    </section>

</asp:Content>
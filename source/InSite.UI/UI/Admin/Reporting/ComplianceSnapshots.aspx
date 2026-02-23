<%@ Page Language="C#" CodeBehind="ComplianceSnapshots.aspx.cs" Inherits="InSite.UI.Admin.Reporting.ComplianceSnapshots" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/HistorySnapshotGrid.ascx" TagName="HistorySnapshotGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="SnapshotStatus" />

    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:HistorySnapshotGrid runat="server" ID="HistorySnapshotGrid" />
            </div>
        </div>

    </section>

</asp:Content>

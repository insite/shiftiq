<%@ Page Language="C#" CodeBehind="Snapshot.ascx.cs" Inherits="InSite.Cmds.Actions.Reporting.Compliance.Snapshot" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/HistorySnapshotGrid.ascx" TagName="HistorySnapshotGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="SnapshotStatus" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-chart-bar me-1"></i>
            Worker Compliance History
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:HistorySnapshotGrid runat="server" ID="HistorySnapshotGrid" />
            </div>
        </div>
    </section>

</asp:Content>

<%@ Page Language="C#" CodeBehind="Verify.aspx.cs" Inherits="InSite.Admin.Events.Candidates.Forms.Verify" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:ProgressPanel runat="server" ID="VerifyProgress" HeaderText="Verifying Candidates" Cancel="Disabled" PostBackAsTrigger="false" AjaxRequestAsTrigger="false">
        <ClientEvents OnPollingStopped="verifyCandidates.onProgressStopped" />
        <Items>
            <insite:ProgressIndicator Name="Progress" Caption="Progress: {percent}%" />
            <insite:ProgressStatus>
                Elapsed time: <span style='display:inline-block;width:80px;'>{time_elapsed}s</span>
                Remaining time: <span style='display:inline-block;width:80px;'>{time_remaining}</span>
            </insite:ProgressStatus>
        </Items>
    </insite:ProgressPanel>

    <asp:Button runat="server" ID="VerifyCompleted" CssClass="d-none" />

    <insite:PageFooterContent runat="server">

        <script type="text/javascript">
            (function () {
                var instance = window.verifyCandidates = window.verifyCandidates || {};

                instance.onProgressStopped = function onUpgradeStopped() {
                    document.getElementById('<%= VerifyCompleted.ClientID %>').click();
                };
            })();
        </script>

    </insite:PageFooterContent>
</asp:Content>

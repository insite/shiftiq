<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountStatus.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.AccountStatus" %>

<div class="alert alert-success btn-sm margin-top-general" style="padding: 8px;" runat="server" id="AccountStatusApproved" visible="false">
    <i class="fa fa-check-circle"></i> IEC-BC Verified
</div>
<div class="alert alert-warning btn-sm margin-top-general" style="padding: 8px;" runat="server" id="AccountStatusNotApproved" visible="false">
    <i class="fa fa-user-plus"></i> Account is pending IEC-BC Admin approval
</div>

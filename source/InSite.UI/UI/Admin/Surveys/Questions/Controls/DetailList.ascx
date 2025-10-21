<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailList.ascx.cs" Inherits="InSite.Admin.Surveys.Questions.Controls.DetailList" %>

<%@ Register Src="OptionGrid.ascx" TagName="OptionGrid" TagPrefix="uc" %>

<div>
    <uc:OptionGrid runat="server" ID="OptionGrid" />
</div>

<div class="mt-3">

    <div runat="server" id="ListEnableRandomizationField">
        <insite:CheckBox runat="server" ID="ListEnableRandomization" Text="Randomize Options" />
    </div>
    <div runat="server" id="ListEnableOtherTextField">
        <insite:CheckBox runat="server" ID="ListEnableOtherText" Text="Display <strong>Other</strong> Text Box" />
    </div>
    <div runat="server" id="ListEnableBranchField">
        <insite:CheckBox runat="server" ID="ListEnableBranch" Text="Enable Branches" />
    </div>
    <div runat="server" id="ListEnableMembershipField">
        <insite:CheckBox runat="server" ID="ListEnableGroupMembership" Text="Enable Group Memberships" />
    </div>

</div>
<%@ Control Language="C#" CodeBehind="ProgramAchievementsCriteria.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.ProgramAchievementsCriteria" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementCriteriaSelector.ascx" TagName="AchievementCriteriaSelector" TagPrefix="uc" %>

<insite:CustomValidator runat="server" ID="AchievementSelectorValidator" ErrorMessage="At least one achievement must be selected." Display="None" />

<div class="row">
    <div class="col-lg-6">

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Group Type
                    </label>
                    <insite:GroupTypeComboBox runat="server" ID="GroupType" EmptyMessage="All group types" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Group
                    </label>
                    <insite:FindGroup runat="server" ID="FindGroup" MaxSelectionCount="0" EmptyMessage="All groups" CurrentOrganizationOnly="true" />
                </div>

            </ContentTemplate>
        </insite:UpdatePanel>

        <div class="form-group mb-3">
            <label class="form-label">
                Program
            </label>
            <insite:FindProgram runat="server" ID="FindProgram" MaxSelectionCount="0" EmptyMessage="All programs" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Credential Status
            </label>
            <insite:ComboBox runat="server" ID="CredentialStatus">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="Pending" Text="Pending" />
                    <insite:ComboBoxOption Value="Valid" Text="Valid" />
                    <insite:ComboBoxOption Value="Expired" Text="Expired" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Learner
            </label>
            <insite:FindPerson runat="server" ID="FindLearner" MaxSelectionCount="0" EmptyMessage="All learners" />
        </div>

    </div>
    <div class="col-lg-6">

        <uc:AchievementCriteriaSelector runat="server" ID="AchievementSelector" EnableSingleSelection="true" />

    </div>
</div>

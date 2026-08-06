<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrainingReportCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Reporting.Report.TrainingReportCriteria" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementCriteriaSelector.ascx" TagName="AchievementCriteriaSelector" TagPrefix="uc" %>

<insite:CustomValidator runat="server" ID="AchievementSelectorValidator" ErrorMessage="Set at least one achievement type to All, or pick specific achievements." Display="None" />

<insite:Alert runat="server" Indicator="Information">
    <strong>Please note:</strong> to prevent timeout errors, the system does not allow reporting on all achievements for all departments at the same time.
    If all achievements are selected, then a selection of departments must be made.
</insite:Alert>

<div class="row">
    <div class="col-lg-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Department
            </label>
            <cmds:FindDepartment runat="server" ID="FindDepartment" MaxSelectionCount="0" CausesValidation="true" ValidationGroup="Other" EmptyMessage="All departments" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Learner
            </label>
            <insite:FindPerson runat="server" ID="FindLearner" MaxSelectionCount="0" Enabled="false" EmptyMessage="All learners" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Program
            </label>
            <insite:FindProgram runat="server" ID="FindProgram" MaxSelectionCount="0" CausesValidation="true" ValidationGroup="Other" EmptyMessage="All programs" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Settings
            </label>
            <div>
                <insite:RadioButtonList ID="IsRequired" runat="server">
                    <asp:ListItem Text="Required and Optional Achievements" Selected="True" />
                    <asp:ListItem Value="True" Text="Required Achievements Only" />
                </insite:RadioButtonList>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Membership
            </label>
            <div>
                <insite:CheckBox runat="server" ID="MembershipOrganization" Text="Organization" CssClass="me-3" />
                <insite:CheckBox runat="server" ID="MembershipDepartment" Text="Department" Checked="true" />
            </div>
        </div>

        <asp:PlaceHolder runat="server" ID="JobDivisionSection">
            <div class="form-group mb-3">
                <label class="form-label">
                    Job Division
                </label>
                <insite:ComboBox runat="server" ID="JobDivisionPresence" AllowBlank="false" AutoPostBack="true">
                    <Items>
                        <insite:ComboBoxOption Value="" Text="With or without a job division" />
                        <insite:ComboBoxOption Value="With" Text="With a job division" />
                        <insite:ComboBoxOption Value="Without" Text="Without a job division" />
                    </Items>
                </insite:ComboBox>
                <asp:PlaceHolder runat="server" ID="JobDivisionValuePanel">
                    <div class="mt-2">
                        <insite:JobDivisionComboBox runat="server" ID="JobDivisionValue" EmptyMessage="All job divisions" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:PlaceHolder>

        <div class="form-group mb-3">
            <label class="form-label">
                Credential Status
            </label>
            <insite:ComboBox runat="server" ID="CredentialStatus">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="Valid" Text="Valid" />
                    <insite:ComboBoxOption Value="Pending" Text="Pending" />
                    <insite:ComboBoxOption Value="Expired" Text="Expired" />
                </Items>
            </insite:ComboBox>
            <div class="form-group mt-2">
                <insite:CheckBox runat="server" ID="ExcludeSelfDeclaredCredentials" Text="Exclude self-declared achievements" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Completed Since
            </label>
            <insite:DateSelector ID="CompletedSince" runat="server" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Completed Before
            </label>
            <insite:DateSelector ID="CompletedBefore" runat="server" />
        </div>

    </div>
    <div class="col-lg-6">

        <uc:AchievementCriteriaSelector runat="server" ID="AchievementSelector" EnableSelectionModes="true" />

    </div>
</div>

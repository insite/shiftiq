<%@ Control Language="C#" CodeBehind="TabConfigurationDetails.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Sections.Controls.TabConfigurationDetails" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ConfigurationUpdatePanel" />

<insite:UpdatePanel runat="server" ID="ConfigurationUpdatePanel">
    <ContentTemplate>

        <div class="form-group mb-3">
            <label class="form-label">
                Warning on Next Tab
            </label>
            <div>
                <insite:BooleanComboBox runat="server" ID="WarningOnNextTab" TrueText="Show" FalseText="Disabled" AllowBlank="false" />
            </div>
            <div class="form-text">
                Whether the candidate is warned before moving on to the next tab.
            </div>
        </div>

        <div runat="server" id="BreakTimerField" class="form-group mb-3">
            <label class="form-label">
                Break Timer
            </label>
            <div>
                <insite:BooleanComboBox runat="server" ID="BreakTimer" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" />
            </div>
            <div class="form-text">
                Whether the time spent on this tab is excluded from the attempt's total time.
            </div>
        </div>

        <div runat="server" id="TimeLimitField" class="form-group mb-3">
            <label class="form-label">
                Time Limit (minutes)
            </label>
            <div>
                <insite:NumericBox runat="server" ID="TimeLimit" Width="100%" MinValue="0" MaxValue="1440" NumericMode="Integer" />
            </div>
            <div class="form-text">
                The maximum number of minutes the candidate can stay on this tab.
            </div>
        </div>

        <div runat="server" id="TimerTypeField" class="form-group mb-3">
            <label class="form-label">
                Timer Type
            </label>
            <div>
                <insite:ComboBox runat="server" ID="TimerType" AllowBlank="false" />
            </div>
            <div class="form-text">
                Whether the candidate can leave the tab before the time limit is reached (Optional) or must wait for it to elapse (Enforced).
            </div>
        </div>

    </ContentTemplate>
</insite:UpdatePanel>

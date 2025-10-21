<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricDetail.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.RubricDetail" %>

<div class="row">
    <div class="col-md-6">
        <h3>Rubric</h3>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="RubricPanel" />

        <insite:UpdatePanel runat="server" ID="RubricPanel">
            <ContentTemplate>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Connect an Existing Rubric
                        <span class="float-right">
                            <insite:IconLink runat="server" id="CreateLink" ToolTip="Add a new rubric" Name="plus-square" />
                            <insite:IconLink runat="server" id="OutlineLink" ToolTip="View rubric details" Name="external-link-square" Target="_blank" />
                        </span>
                    </label>
                    <div>
                        <insite:FindRubric runat="server" ID="Rubric" />
                    </div>
                    <div class="form-text" runat="server" id="RubricHint">
                        If a rubric is not connected to this question, it will not be graded *
                    </div>
                    <div runat="server" id="ChangeWarning" class="alert alert-warning mt-2" visible="false">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Important:</strong>
                        Do not modify a rubric if there are existing assessment attempts.
                        Changes to a rubric will affect all previous attempts.<br />
                        If necessary, create a new version of the question with the updated rubric.
                    </div>
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const instanceName = '<%= ClientID %>';
            if (typeof window[instanceName] !== 'undefined')
                return;

            const instance = window[instanceName] = {};
            const hasAttempts = <%= HasAttempts.ToString().ToLower() %>;
            const rubricValue = '<%= Rubric.Value.HasValue ? Rubric.Value.Value.ToString() : string.Empty %>'
            const confirmText = `ARE YOU SURE?
Do not modify a rubric if there are existing assessment attempts. Changes to a rubric will affect all previous attempts.
If needed, create a new question version with the updated rubric.`;

            instance.allowSave = function () {
                if (hasAttempts === false || rubricValue.length == 0)
                    return true;

                const selection = <%= Rubric.ClientID %>.getItem();
                if (!selection) {
                    alert('The rubric cannot be disconnected if the question has attempts!');
                    return false;
                }

                if (selection.value.toLowerCase() === rubricValue.toLowerCase())
                    return true;

                return confirm(confirmText);
            };
        })();
    </script>
</insite:PageFooterContent>
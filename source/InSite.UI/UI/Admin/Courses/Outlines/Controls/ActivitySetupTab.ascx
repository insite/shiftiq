<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivitySetupTab.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivitySetupTab" %>

<%@ Register Src="./PrerequisiteList.ascx" TagName="PrerequisiteList" TagPrefix="uc" %>

<uc:PrerequisiteList runat="server" ID="PrerequisiteList" />

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">Requirement</label>
            <div>
                <div><asp:RadioButton runat="server" ID="RequirementConditionView" GroupName="RequirementCondition" Text="Learners must view the activity" /></div>
                <div><asp:RadioButton runat="server" ID="RequirementConditionMarkAsDone" GroupName="RequirementCondition" Text="Learners must mark the activity as done" /></div>
                <div><asp:RadioButton runat="server" ID="RequirementConditionScoreAtLeast" GroupName="RequirementCondition" Text="Learners must achieve a passing score" /></div>
                <div><asp:RadioButton runat="server" ID="RequirementConditionCompleteSurvey" GroupName="RequirementCondition" Text="Learners must complete a submission to the form" /></div>
                <div><asp:RadioButton runat="server" ID="RequirementConditionCompleteScorm" GroupName="RequirementCondition" Text="Learners must complete the SCORM package" /></div>
            </div>
            <div class="form-text">
                Learners must satisfy this requirement before the activity is considered complete.
            </div>
            <asp:Button runat="server" ID="RequirementConditionChanged" CssClass="d-none" />
        </div>

        <insite:Container runat="server" ID="DoneSettings" Visible="false">
            <div class="form-group mb-3">
                <label class="form-label">
                    Done Button Text
                    <insite:RequiredValidator runat="server" ID="DoneButtonTextValidator" ControlToValidate="DoneButtonText" FieldName="Done Button Text" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="DoneButtonText" MaxLength="24" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Done Button Instructions
                    <insite:RequiredValidator runat="server" ID="DoneButtonInstructionsValidator" ControlToValidate="DoneButtonInstructions" FieldName="Done Button Instructions" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="DoneButtonInstructions" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Activity Done Instructions
                    <insite:RequiredValidator runat="server" ID="DoneMarkedInstructionsValidator" ControlToValidate="DoneMarkedInstructions" FieldName="Activity Done Instructions" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="DoneMarkedInstructions" />
                </div>
            </div>
        </insite:Container>

        <div class="form-group mb-3" runat="server" id="ActivityTypeField">
            <label class="form-label">Activity Type</label>
            <div>
                <asp:RadioButtonList runat="server" ID="ActivityType" RepeatDirection="Horizontal" RepeatColumns="4">
                    <asp:ListItem Value="Lesson" />
                    <asp:ListItem Value="Assessment" />
                    <asp:ListItem Value="Survey" />
                    <asp:ListItem Value="Document" />
                    <asp:ListItem Value="Link" />
                    <asp:ListItem Value="Video" />
                    <asp:ListItem Value="Interaction" />
                    <asp:ListItem Value="Quiz" />
                </asp:RadioButtonList>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Activity Name</label>
            <div>
                <insite:TextBox runat="server" ID="ActivityName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Activity Code</label>
            <div>
                <insite:TextBox runat="server" ID="ActivityCode" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Expected Time Required (in minutes)
            </label>
            <div>
                <insite:NumericBox runat="server" ID="ActivityMinutes" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Activity Settings
            </label>
            <div>
                <asp:CheckBox runat="server" ID="ActivityIsAdaptive" Text="Adaptive" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Author Name</label>
            <div>
                <insite:TextBox runat="server" ID="AuthorName" MaxLength="100" />
            </div>
            <div class="form-text">
                Name of the individual who authored this content.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Date Authored</label>
            <div>
                <insite:DateSelector runat="server" ID="AuthorDate" />
            </div>
            <div class="form-text">
                Date the content was initially written.
            </div>
        </div>
    
        <div class="form-group mb-3">
            <div class="float-end">
                <span class="badge bg-custom-default">Asset # <asp:Literal runat="server" ID="ActivityAsset" /></span>
            </div>
            <label class="form-label">Activity Identifier</label>
            <div>
                <asp:Literal runat="server" ID="ActivityIdentifier" />
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            let button = null;

            Sys.Application.add_load(onLoad);

            function onLoad() {
                if (button && document.contains(button))
                    return;

                button = document.getElementById('<%= RequirementConditionChanged.ClientID %>');
                button.closest('.form-group').querySelectorAll('input[type="radio"]').forEach(radio => {
                    radio.addEventListener('change', onRadioChanged);
                });
            }

            function onRadioChanged() {
                button.click();
            }
        })();
    </script>
</insite:PageFooterContent>

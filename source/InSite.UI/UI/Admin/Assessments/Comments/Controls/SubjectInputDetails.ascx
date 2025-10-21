<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubjectInputDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Comments.Controls.SubjectInputDetails" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionUpdatePanel" />
<insite:UpdatePanel runat="server" ID="QuestionUpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">Specification</label>
            <div>
                <insite:ComboBox runat="server" ID="SpecSelector" />
            </div>
            <div class="form-text">
                If the comment pertains to a specific specification then select it here.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Form</label>
            <div>
                <insite:ComboBox runat="server" ID="FormSelector" />
            </div>
            <div class="form-text">
                If the comment pertains to a specific form then select it here.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Question</label>
            <div>
                <insite:FindEntity runat="server" ID="QuestionSelector" />
            </div>
            <div class="form-text">
                If the comment pertains to a specific question then select it here.
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>


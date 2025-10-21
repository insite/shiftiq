<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrerequisiteList.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.PrerequisiteList" %>

<div runat="server" id="PrerequisiteRepeaterField" class="form-group mb-3">
    <label class="form-label">
        Prerequisite(s)
    </label>
    <asp:Repeater runat="server" ID="PrerequisiteRepeater">
        <HeaderTemplate>
            <table class="table table-striped">
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width:100%">
                    <a runat="server"
                        visible='<%# Eval("TriggerUrl") != null %>'
                        target='<%# Eval("TriggerUrlTarget")%>'
                        href='<%# Eval("TriggerUrl") %>'
                    >
                        <%# Eval("TriggerChange") %>:
                        <%# Eval("TriggerDescription") %>
                    </a>
                    <asp:Literal runat="server" visible='<%# Eval("TriggerUrl") == null %>' Text='<%# Eval("TriggerChange") + ": " + Eval("TriggerDescription") %>' />
                </td>
                <td>
                    <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete Prerequisite" CommandName="PrerequisiteDelete" CommandArgument='<%# Eval("PrerequisiteIdentifier") %>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<div runat="server" id="PrerequisiteDeterminer" class="form-group mb-3">
    <label class="form-label">
        Prerequisite Determiner
    </label>
    <div>
        <div><asp:RadioButton runat="server" ID="PrerequisiteDeterminerAny" GroupName="PrerequisiteDeterminer" Text="Any" />
             <asp:RadioButton runat="server" ID="PrerequisiteDeterminerAll" GroupName="PrerequisiteDeterminer" Text="All" /></div>
    </div>
    <div class="form-text">
        How many of the prerequisites listed above must be satisfied?
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        New Prerequisite
    </label>
    <div class="row">
        <div class="col-md-12">
            <insite:ComboBox runat="server" ID="TriggerChange" Width="100%">
                <Items>
                    <insite:ComboBoxOption Value="None" Text="" Selected="true" />
                    <insite:ComboBoxOption Value="ActivityCompleted" Text="Activity Completed" />
                    <insite:ComboBoxOption Value="AssessmentPassed" Text="Assessment Passed" />
                    <insite:ComboBoxOption Value="AssessmentFailed" Text="Assessment Failed" />
                    <insite:ComboBoxOption Value="AssessmentScored" Text="Assessment Scored" />
                    <insite:ComboBoxOption Value="QuestionAnsweredCorrectly" Text="Question Answered Correctly" />
                    <insite:ComboBoxOption Value="QuestionAnsweredIncorrectly" Text="Question Answered Incorrectly" />
                    <insite:ComboBoxOption Value="GradeItemPassed" Text="Grade Item Passed" />
                    <insite:ComboBoxOption Value="GradeItemFailed" Text="Grade Item Failed" />
                </Items>
            </insite:ComboBox>
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerActivityField" Visible="false">
        <div class="col-md-12">
            <insite:ActivityComboBox runat="server" ID="TriggerActivityIdentifier" />
            <div class="d-none">
                <insite:RequiredValidator runat="server"
                    ID="TriggerActivityIdentifierValidator"
                    ControlToValidate="TriggerActivityIdentifier"
                    FieldName="Trigger"
                />
            </div>
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerFormField" Visible="false">
        <div class="col-md-12">
            <insite:FindBankForm runat="server" ID="TriggerAssessmentFormIdentifier" EmptyMessage="Select Assessment Form" />
            <div class="d-none">
                <insite:RequiredValidator runat="server"
                    ID="TriggerAssessmentFormIdentifierValidator"
                    ControlToValidate="TriggerAssessmentFormIdentifier"
                    FieldName="Trigger"
                />
            </div>
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerConditionField" Visible="false">
        <div class="col-md-12">
            between 
            <insite:NumericBox runat="server" ID="TriggerConditionScoreFrom" NumericMode="Integer" MinValue="0" MaxValue="100" Width="40" />
            and 
            <insite:NumericBox runat="server" ID="TriggerConditionScoreThru" NumericMode="Integer" MinValue="0" MaxValue="100" Width="40" />
            percent
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerBankField" Visible="false">
        <div class="col-md-12">
            <insite:FindBank runat="server" ID="TriggerAssessmentBankIdentifier" EmptyMessage="Select Question Bank" />
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerQuestionField" Visible="false">
        <div class="col-md-12">
            <insite:FindBankQuestion runat="server" ID="TriggerAssessmentQuestionIdentifier" EmptyMessage="Select Question" />
            <div class="d-none">
                <insite:RequiredValidator runat="server"
                    ID="TriggerAssessmentQuestionIdentifierValidator"
                    ControlToValidate="TriggerAssessmentQuestionIdentifier"
                    FieldName="Trigger"
                />
            </div>
        </div>
    </div>
    <div class="row mt-1" runat="server" id="TriggerGradeItemField" Visible="false">
        <div class="col-md-12">
            <insite:GradebookItemComboBox runat="server" ID="TriggerGradeItemIdentifier" EmptyMessage="Select Grade Item" DropDown-Size="10" />
            <div class="d-none">
                <insite:RequiredValidator runat="server"
                    ID="TriggerGradeItemIdentifierValidator"
                    ControlToValidate="TriggerGradeItemIdentifier"
                    FieldName="Trigger"
                />
            </div>
        </div>
    </div>
    <div class="form-text">
        Learners must satisfy these prerequisites before the activity is available to start.
    </div>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivitySetup.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivitySetup" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetupTab" TagPrefix="uc" %>
<%@ Register Src="../../Activities/Controls/CompetenciesSelector.ascx" TagName="CompetenciesSelector" TagPrefix="uc" %>
<%@ Register Src="./PrivacySettingsGroups.ascx" TagName="PrivacySettingsGroups" TagPrefix="uc" %>
    
<insite:Alert runat="server" ID="ActivitySetupAlert" />

<insite:Nav runat="server">
            
    <insite:NavItem runat="server" Title="Activity Details">
        
        <div class="row">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <uc:ActivitySetupTab runat="server" ID="ActivitySetupTab" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="alert alert-info">
                    
                    <i class="fas fa-info-circle"></i>
                    <strong>Note:</strong>
                    
                    The input fields you see here are exactly the same input fields that you see under the Activity Setup tab
                    in the Course Outline panel above. <br /><br />
                    
                    This is by design, so you can view and update the settings for your activity in either place. You'll likely 
                    prefer one or the other depending on the type of course-work you are configuring.

                </div>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Records">

        <div class="row">
            <div class="col-lg-6">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Grading</h3>

                        <div runat="server" id="GradeItemIdentifierField" class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconButton runat="server" id="GradebookCreateButton" ToolTip="Add a new grade item" Name="plus-square" />
                                <insite:IconLink runat="server" id="GradebookOutlineLink" ToolTip="View details for this grade item" Name="external-link-square" Target="_blank" />
                            </div>
                            <label class="form-label">
                                Grade Item
                                <insite:CustomValidator runat="server"
                                    ID="UniqueGradeItem"
                                    ControlToValidate="GradeItemIdentifier"
                                    Display="None"
                                    ErrorMessage="The specified grade item was already assigned to another activity"
                                    ValidationGroup="ActivitySetup"
                                />
                            </label>
                            <div>
                                <insite:GradebookItemComboBox runat="server" ID="GradeItemIdentifier" />
                            </div>
                        </div>

                        <div runat="server" id="GradeItemFields">

                            <div class="form-group mb-3">
                                <label class="form-label">Grade Item Name</label>
                                <div>
                                    <insite:TextBox runat="server" ID="GradeItemName" />
                                </div>
                                <div class="form-text">
                        
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Grade Item Code
                                    <insite:CustomValidator runat="server" ID="GradeItemCodeValidator" Display="None" ValidationGroup="ActivitySetup" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="GradeItemCode" MaxLength="100" />
                                </div>
                                <div class="form-text">The unique code for this score item.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Grade Item Format
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="GradeItemFormat" RepeatLayout="Table">
                                        <asp:ListItem Text="None" Value="None" />
                                        <asp:ListItem Text="Score (%)" Value="Percent" Selected="true" />
                                        <asp:ListItem Text="Points" Value="Point" />
                                        <asp:ListItem Text="Complete or Incomplete" Value="Boolean" />
                                        <asp:ListItem Text="Text" Value="Text" />
                                        <asp:ListItem Text="Number" Value="Number" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="form-text">The display format for the grade item.</div>
                            </div>

                            <div runat="server" id="GradeItemPassPercentField" class="form-group mb-3" style="display:none;">
                                <label class="form-label">Passing Score (%)</label>
                                <div>
                                    <insite:NumericBox runat="server" ID="GradeItemPassPercent" NumericMode="Integer" MinValue="0" MaxValue="100" Width="80px" style="text-align:right;" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

            <div class="col-lg-6" runat="server" id="AchievementPanel">

                <div class="card h-100">
                    <div class="card-body">

                        <h3>Recognition</h3>

                        <div runat="server" id="AchievementIdentifierField" class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconButton runat="server" id="AchievementCreateButton" ToolTip="Add a new certificate" Name="plus-square" />
                                <insite:IconLink runat="server" id="AchievementOutlineLink" ToolTip="View details for this certificate" Name="external-link-square" Target="_blank" />
                            </div>
                            <label class="form-label">Achievement</label>
                            <div>
                                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            </div>
                        </div>

                        <div runat="server" id="AchievementFields">

                            <div class="form-group mb-3">
                                <label class="form-label">Achievement Name</label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Achievement Type</label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementLabel" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Achievement Expiration</label>
                                <div>
                                    <asp:Literal runat="server" ID="AchievementExpiration" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Certificate Layout</label>
                                <div>
                                    <insite:CertificateLayoutComboBox runat="server" ID="AchievementLayout" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="CompetenciesTab" Title="Competencies">
        <div class="card">
            <div class="card-body">
                <uc:CompetenciesSelector runat="server" ID="CompetenciesSelector" />
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Privacy Settings">

        <div class="row">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <uc:PrivacySettingsGroups runat="server" ID="PrivacySettingsGroups" />
                    </div>
                </div>
            </div>
        </div>

    </insite:NavItem>

</insite:Nav>

<div class="mt-3">
    <insite:SaveButton runat="server" ID="ActivitySaveButton" ValidationGroup="ActivitySetup" />
    <insite:CancelButton runat="server" ID="ActivityCancelButton" />
</div>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">
        (function () {
            $('#<%= GradeItemFormat.ClientID %>').on('change', onGradeItemFormatChanged);

            onGradeItemFormatChanged();

            function onGradeItemFormatChanged() {
                var $field = $('#<%= GradeItemPassPercentField.ClientID %>');
                var value = $('#<%= GradeItemFormat.ClientID %> input[type="radio"]:checked:first').prop('value');
                if (value == 'Percent')
                    $field.show();
                else
                    $field.hide();
            }
        })();
    </script>

</insite:PageFooterContent>
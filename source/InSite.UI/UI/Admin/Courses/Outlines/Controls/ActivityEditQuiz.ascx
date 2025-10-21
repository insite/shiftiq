<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEditQuiz.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityEditQuiz" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetup" TagPrefix="uc" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Nav runat="server">

    <insite:NavItem runat="server" Title="Quiz">
        
        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink runat="server" id="QuizLink" ToolTip="View details for this quiz" Name="external-link-square" Target="_blank" />
            </div>
            <label class="form-label">
                Quiz
                <insite:RequiredValidator runat="server" ControlToValidate="QuizIdentifier" FieldName="Quiz" />
            </label>
            <div>
                <insite:QuizComboBox runat="server" ID="QuizIdentifier" />
            </div>
        </div>

        <insite:Container runat="server" ID="QuizFields" Visible="false">
            <div class="form-group mb-3">
                <label class="form-label">
                    Quiz Name
                    <insite:RequiredValidator runat="server" ID="QuizNameValidator" ControlToValidate="QuizName" FieldName="Quiz Name" ValidationGroup="CourseConfig" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="QuizName" MaxLength="100" />
                </div>
            </div>

            <div runat="server" id="PassingWpmField" class="form-group mb-3">
                <label class="form-label">Passing WPM</label>
                <div>
                    <insite:NumericBox runat="server" ID="PassingWpm" MinValue="0" NumericMode="Integer" Width="100px" />
                </div>
                <div class="form-text">
                    What is the minimum WPM required to pass the quiz?
                </div>
            </div>

            <div runat="server" id="PassingKphField" class="form-group mb-3">
                <label class="form-label">Passing KPH</label>
                <div>
                    <insite:NumericBox runat="server" ID="PassingKph" MinValue="0" NumericMode="Integer" Width="100px" />
                </div>
                <div class="form-text">
                    What is the minimum KPH required to pass the quiz?
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">Passing Accuracy (%)</label>
                <div>
                    <insite:NumericBox runat="server" ID="PassingAccuracy" Width="100px" MinValue="0" MaxValue="100" NumericMode="Integer" />
                </div>
                <div class="form-text">
                    What is the minimum accuracy required to pass the quiz?
                </div>
            </div>
        </insite:Container>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Optional Content">

        <div class="form-group mb-3">
            <label class="form-label">Language</label>
            <div>
                <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
            </div>
        </div>

        <div class="row row-translate">
            <div class="col-md-12">
                <asp:Repeater runat="server" ID="ContentRepeater">
                    <ItemTemplate>
                        <div class="form-group mb-3">
                            <insite:DynamicControl runat="server" ID="Container" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Activity Setup">
        <uc:ActivitySetup runat="server" ID="ActivitySetup" />
    </insite:NavItem>

</insite:Nav>

<div class="mt-5">
    <insite:SaveButton runat="server" ID="ActivitySaveButton" ValidationGroup="CourseConfig" />
    <insite:CancelButton runat="server" ID="ActivityCancelButton" />
</div>
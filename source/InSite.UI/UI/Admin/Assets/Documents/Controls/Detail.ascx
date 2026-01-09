<%@ Control Language="C#" CodeBehind="Detail.ascx.cs" Inherits="InSite.UI.Admin.Assets.Documents.Controls.Detail" %>

<%@ Register TagPrefix="uc" TagName="MarkdownTextEditor" Src="MarkdownTextEditor.ascx" %>

<insite:Alert runat="server" ID="StatusAlert" />

<insite:Accordion runat="server" ID="MainAccordion">

    <insite:AccordionPanel runat="server" ID="UploadDocumentPanel" Title="Microsoft Word Document" Icon="fas fa-file-word">
        <div class="row">
            <div class="col-lg-6">
                <insite:FileUploadV1 runat="server" ID="DocumentUpload" LabelText="Select Document" FileUploadType="Document" />
            </div>
            <div class="row">
                <div class="col-lg-6 mt-3">
                    <insite:Button runat="server" ID="ActivitySaveButton" Text="Convert" ButtonStyle="Primary" Icon="fas fa-cog" CausesValidation="false" />
                </div>
            </div>
        </div>
    </insite:AccordionPanel>

    <insite:AccordionPanel runat="server" ID="MarkdownPanel" Title="Markdown Conversion" Icon="fas fa-file-code">
        <div class="row">
            <div class="col-md-12">
                <uc:MarkdownTextEditor EnableViewState="true" runat="server" ID="MarkdownText" />
            </div>
        </div>
    </insite:AccordionPanel>

    <insite:AccordionPanel runat="server" ID="CoursesPanel" Title="Course" Icon="fas fa-chalkboard-teacher" Visible="true">
        <div class="row">
            <div class="col-lg-6">

                <div class="form-group mb-3">
                    <div>
                        <insite:RadioButton runat="server" ID="RadioExtend" Text="Add lesson to existing course" Value="True" GroupName="Radio" Checked="true" />
                        <insite:RadioButton runat="server" ID="RadioAdd" Text="Add new course" Value="False" GroupName="Radio" />
                    </div>
                </div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CoursePanel" />
                <insite:UpdatePanel runat="server" ID="CoursePanel" Visible="false">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <label class="form-label">Select Course</label>
                            <insite:CourseComboBox runat="server" ID="CoursesComboBox" />
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Select Unit</label>
                            <insite:UnitComboBox runat="server" ID="UnitsComboBox" />
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Select Module</label>
                            <insite:ModuleComboBox runat="server" ID="ModulesComboBox" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
                <insite:UpdatePanel runat="server" ID="NewCoursePanel" Visible="false">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Course Name
                                <insite:RequiredValidator runat="server" ID="CourseNameValidator" ControlToValidate="CourseName" ValidationGroup="Course" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CourseName" Text="New Course" MaxLength="200" />
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mt-3">
                <insite:Button runat="server" ID="AddLessonButton" Text="Add Lesson" ButtonStyle="Primary" Icon="fas fa-plus" CausesValidation="false" />
            </div>
        </div>
    </insite:AccordionPanel>
</insite:Accordion>




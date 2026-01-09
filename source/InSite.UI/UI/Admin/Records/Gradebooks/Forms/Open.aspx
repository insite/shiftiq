<%@ Page Language="C#" CodeBehind="Open.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Open" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Gradebook" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload1ValidationGroup" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <section runat="server" ID="EventSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Gradebook
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Gradebook" />
            </div>
        </div>

        <div runat="server" id="NewPanel">

            <div class="row mb-3">

                <div class="col-md-6">
                                    
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div runat="server" id="CopyGradebookField" class="form-group mb-3">
				                <label class="form-label">
                                    Gradebook
                                    <insite:RequiredValidator runat="server" ControlToValidate="CopyGradebookSelector" FieldName="Gradebook" ValidationGroup="Gradebook" />
				                </label>
				                <div>
                                    <insite:GradebookComboBox runat="server" ID="CopyGradebookSelector" />
                                </div>
                                <div class="form-text">Choose the existing gradebook you wish to duplicate.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="GradebookTitle" FieldName="Title" ValidationGroup="Gradebook" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="GradebookTitle" MaxLength="400" />
                                </div>
                                <div class="form-text">The descriptive title for this gradebook.</div>
                            </div>
            
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Class
                                </label>
                                <div>
                                    <insite:FindEvent runat="server" ID="EventIdentifier" ShowPrefix="false" />
                                </div>
                                <div class="form-text">The class that includes the list of registered students.</div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Period
                                </label>
                                <div>
                                    <insite:FindPeriod runat="server" ID="PeriodIdentifier" />
                                </div>
                            </div>

                            <insite:Alert runat="server" ID="AlertClassStatus" />

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement
                                </label>
                                <div>
                                    <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                                </div>
                                <div class="form-text">The course content for this class.</div>
                            </div>

                        </div>
                    </div>

                </div>
            
                <div class="col-md-6">
                            
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Include:
                                    <insite:CustomValidator runat="server" ID="IncludeValidator" ControlToValidate="GradebookTitle" ValidationGroup="Gradebook" ErrorMessage="Please enable Scores and/or Standards for your new gradebook." />
                                </label>
                                <div>
                                    <asp:CheckBox runat="server" ID="Scores" Text="Scores" Checked="true" Enabled="false" />
                                    <asp:CheckBox runat="server" ID="Standards" Text="Standards" />
                                </div>
                                <div class="form-text">Track scores, standards, or both.</div>
                            </div>

                            <div runat="server" id="StandardField" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    Framework
                                    <insite:RequiredValidator runat="server" ControlToValidate="StandardIdentifier" FieldName="Framework" ValidationGroup="Gradebook" />
                                </label>
                                <div>
                                    <insite:FindStandard runat="server" ID="StandardIdentifier" TextType="Title" />
                                </div>
                                <div class="form-text">The framework that contains the standards measured by this gradebook.</div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

            <div>
                <insite:Button runat="server" ID="AddStudentsButton" Text="Add Students" Icon="fas fa-plus-circle" ButtonStyle="Default" Visible="false" />
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Gradebook" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div runat="server" ID="UploadSection">
            <div class="row mb-3">
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Select and Upload Gradebook JSON File
                                </label>
                                <div>
                                    <insite:FileUploadV1 runat="server" ID="JsonFileUpload" LabelText="" AllowedExtensions=".json" FileUploadType="Unlimited" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Uploaded JSON
                                    <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Upload2ValidationGroup" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <div>
                <insite:SaveButton runat="server" ID="UploadSaveButton" ValidationGroup="Upload2ValidationGroup" />
                <insite:CancelButton runat="server" ID="UploadCancelButton" />
            </div>
        </div>

    </section>

    <section runat="server" ID="StudentSection" class="mb-3" visible="false">
        <h2 class="h4 mb-3">
            <i class="far fa-user me-1"></i>
            Students
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
            <asp:Repeater runat="server" ID="StudentRepeater">
                <HeaderTemplate>
                    <table id="Students" class="table table-striped">
                        <thead>
                            <tr>
                                <th>
                                    <input id="SelectAll" type="checkbox" onclick="onSelectAll()" checked="checked" />
                                </th>
                                <th>Student</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="width:50px;">
                            <asp:CheckBox runat="server" ID="Selected" Checked="true" />
                            <asp:Literal runat="server" ID="StudentIdentifier" Text='<%# Eval("UserIdentifier") %>' Visible="false" />
                        </td>
                        <td>
                            <%# Eval("UserFullName") %>
                            <div class="form-text"><%# Eval("UserEmail") %></div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        
            <div class="row">
                <div class="col-lg-4">
                    <insite:SaveButton runat="server" ID="SaveStudentButton" ValidationGroup="Gradebook" />
                    <insite:CancelButton runat="server" ID="CancelStudentButton" />
                </div>
            </div>
            </div>
        </div>
    </section>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            $('[data-upload]').each(function () {
                var $btn = $(this);
                var uploadSelector = $btn.data('upload');
                $(uploadSelector).on('change', function () {
                    var fileName = '';

                    if (this.files) {
                        if (this.files.length > 0) {
                            fileName = this.files[0].name;
                        }
                    } else if (this.value) {
                        fileName = this.value.split(/(\\|\/)/g).pop();
                    }

                    $btn.closest('.input-group').find('input[type="text"]').val(fileName);
                });
            }).on('click', function () {
                var uploadSelector = $(this).data('upload');
                $(uploadSelector).click();
            });
        })();

    </script>
</insite:PageFooterContent>
</asp:Content>

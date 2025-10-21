<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="InSite.UI.Admin.Assets.Scorm.Upload" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:ValidationSummary runat="server" ValidationGroup="Upload" />
    <insite:Alert runat="server" ID="UploadStatus" />

    <insite:ProgressPanel runat="server" ID="UploadProgress" HeaderText="Uploading..." Cancel="Custom">
        <Triggers>
            <insite:ProgressControlTrigger ControlID="UploadButton" />
        </Triggers>
        <Items>
            <insite:ProgressStatus Text="Status: {status}{running_ellipsis}" />
            <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
        </Items>
    </insite:ProgressPanel>

    <section runat="server" ID="SCORMForm" class="pb-5 mb-md-2">
        
        <div class="row">                       

            <div class="col-lg-6">
                
                <dl class="row">
                    <dt class="col-sm-3">
                        Slug:
                        <insite:RequiredValidator runat="server" ControlToValidate="CourseSlug" FieldName="Slug" ValidationGroup="Upload" />
                    </dt>
                    <dd class="col-sm-9">
                        <insite:TextBox runat="server" ID="CourseSlug" MaxLength="50" />
                    </dd>

                    <dt class="col-sm-3">
                        Version:
                        <insite:RequiredValidator runat="server" ControlToValidate="CourseVersion" FieldName="Version" ValidationGroup="Upload" />
                    </dt>
                    <dd class="col-sm-9">
                        <insite:NumericBox runat="server" ID="CourseVersion" NumericMode="Integer" />
                    </dd>

                    <dt class="col-sm-3">
                        Language:
                        <insite:RequiredValidator runat="server" ControlToValidate="CourseLanguage" FieldName="Language" ValidationGroup="Upload" />
                    </dt>
                    <dd class="col-sm-9">
                        <insite:TextBox runat="server" ID="CourseLanguage" MaxLength="2" />
                    </dd>

                    <dt class="col-sm-3">Package:</dt>
                    <dd class="col-sm-9">
                        <insite:FileUploadV1 runat="server" ID="CourseFile" AllowMultiple="false" LabelText=""
                            MaxFileSize="268435456"
                            AllowedExtensions=".zip,.pdf,.mp3,.mp4"
                            OnClientFileUploaded="uploadSCORM.onFileUploaded"
                            OnClientFileUploadFailed="uploadSCORM.onFileUploadFailed"
                        />
                    </dd>
                </dl>

                <div>
                    <insite:Button runat="server" ID="UploadButton" Text="Upload" ValidationGroup="Upload" Icon="fas fa-cloud-upload" CssClass="btn btn-success disabled" />
                </div>
                
            </div>

            <div class="col-lg-6">

                <h3>Previously Uploaded</h3>

                <asp:Repeater runat="server" ID="ScormCourseRepeater">
                    <ItemTemplate>
                        
                        <div class="mb-3">

                            <div>
                                <strong><%# Eval("Title") %></strong>
                            </div>

                            <span class="badge bg-success"><%# Eval("Id") %></span>
                            <span class="badge bg-primary ms-1">Version <%# Eval("Version") %></span>
                            <span class="badge bg-info ms-1"><%# Eval("CourseLearningStandard") %></span>
                            <span class="fs-sm ms-1"><%# Eval("RegistrationCount") %> Registrations</span>

                            <div class="text-muted fs-sm">
                                Created <%# Eval("Created", "{0:MMM d, yyyy}") %>
                                Updated <%# Eval("Updated", "{0:MMM d, yyyy}") %>
                            </div>
                        
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>

        </div>
    </section>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                var instance = window.uploadSCORM = window.uploadSCORM || {};

                instance.onFileUploaded = function () {
                    $("#<%= UploadButton.ClientID %>").removeClass("disabled");
                };

                instance.onFileUploadFailed = function () {
                    $("#<%= UploadButton.ClientID %>").addClass("disabled");
                };
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
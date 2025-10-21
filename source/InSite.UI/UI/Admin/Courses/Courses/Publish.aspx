<%@ Page Language="C#" CodeBehind="Publish.aspx.cs" Inherits="InSite.Admin.Courses.Courses.Publish" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register TagPrefix="uc" TagName="PagePopupSelector" Src="~/UI/Admin/Sites/Pages/Controls/PagePopupSelector.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section runat="server" ID="CoursePanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-chalkboard-teacher me-1"></i>
            Course
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6 settings">

                        <h3>Publication</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Course Name</label>
                            <div>
                                <asp:Literal runat="server" ID="CourseName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Asset #</label>
                            <div>
                                <asp:Literal runat="server" ID="CourseAsset" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Publish Method</label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="PublishMethod">
                                    <asp:ListItem Value="Insert" Text="Add a new web page" Selected="True" />
                                    <asp:ListItem Value="Update" Text="Select an existing page" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Web Site
                                <insite:RequiredValidator runat="server" ControlToValidate="WebSiteIdentifier" FieldName="Web Site" ValidationGroup="Assessment" />
                            </label>
                            <insite:WebSiteComboBox runat="server" ID="WebSiteIdentifier" AllowBlank="true" />
                            <div class="form-text">
                                Select the web site to host the course.
                            </div>
                        </div>

                        <div runat="server" id="WebPagePanel" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                <asp:Literal runat="server" ID="WebPageLabel" />
                                <insite:CustomValidator runat="server" ID="WebPageRequired" ValidationGroup="Assessment" />
                            </label>
                            <uc:PagePopupSelector runat="server" ID="WebPageIdentifier" />
                            <div runat="server" id="WebPageHelp" class="form-text"></div>
                            
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-md-12">

            <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-cloud-upload" ValidationGroup="Assessment" ButtonStyle="Success" />
            <insite:CancelButton runat="server" ID="CancelButton" />

        </div>
    </div>

</asp:Content>

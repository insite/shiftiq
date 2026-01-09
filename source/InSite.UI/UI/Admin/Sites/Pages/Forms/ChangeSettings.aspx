<%@ Page Language="C#" CodeBehind="ChangeSettings.aspx.cs" Inherits="InSite.Admin.Sites.Pages.ChangeSettings" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/PageInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="PageChange" />

    <section class="mb-3">

        <h2 class="h4 mb-3">
            <i class="far fa-cloud me-1"></i>
            Page
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Details</h3>

                        <uc:Details runat="server" ID="PageDetails" />

                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>Content</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Icon</label>
                            <div>
                                <insite:TextBox runat="server" ID="Icon" MaxLength="30" />
                            </div>
                            <div class="form-text">
                                Used to add a <a href="https://fontawesome.com/search" target="_blank">FontAwesome</a> icon to the portal tile. If a picture is desired instead, add ImageUrl to the list of Tabs below, then save, and go to the Content Editor.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Visible Tabs</label>
                            <div>
                                <insite:TextBox runat="server" ID="ContentLabels" MaxLength="100" />
                            </div>
                            <div class="form-text">
                                Controls which tabs appear on the Content Editor screen. 
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Layout</label>
                            <div>

                                <insite:WebPageTemplateComboBox runat="server" ID="ContentControl" AllowBlank="true" />

                                <div runat="server" id="CatalogPanel" class="mt-2" visible="false">
                                    <insite:CatalogComboBox runat="server" ID="CatalogIdentifier" />
                                </div>

                                <div runat="server" id="CoursePanel" class="mt-2" visible="false">
                                    <insite:FindCourse runat="server" ID="CourseIdentifier" />
                                </div>

                                <div runat="server" id="ProgramPanel" class="mt-2" visible="false">
                                    <insite:FindProgram runat="server" ID="ProgramIdentifier" />
                                </div>

                                <div runat="server" id="SurveyPanel" class="mt-2" visible="false">
                                    <insite:FindWorkflowForm runat="server" ID="SurveyIdentifier" />
                                </div>
                            </div>
                            <div class="form-text">
                                Used to define the behaviour of a portal page that is linked to a specific type of asset, like a course, program or form. 
                            </div>
                        </div>

                    </div>
                </div>

                <div class="card border-0 shadow-lg mt-2">
                    <div class="card-body">

                        <h3>Integration</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Hook / Integration Code
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Hook" MaxLength="100" Width="100%" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>


    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="PageChange" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

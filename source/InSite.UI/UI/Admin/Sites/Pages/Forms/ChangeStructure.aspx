<%@ Page Language="C#" CodeBehind="ChangeStructure.aspx.cs" Inherits="InSite.Admin.Sites.Pages.ChangeStructure" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/PagePopupSelector.ascx" TagName="PagePopupSelector" TagPrefix="uc" %>
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

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Update Structure</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Page Slug (URL Segment)
                                    <insite:RequiredValidator runat="server" ControlToValidate="PageSlug" FieldName="Page Slug" ValidationGroup="PageChange" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PageSlug" runat="server" MaxLength="100" Width="100%" />
                                </div>
                                <div class="form-text">
                                    The part of the URL that specifically refers to this page.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">External Link</label>
                                <div>
                                    <insite:TextBox runat="server" ID="NavigateUrl" MaxLength="500" />
                                </div>
                                <div style="margin-top:5px;">
                                    <asp:CheckBox runat="server" ID="IsNavigateUrlToNewTab" Text="Open in a new browser window" />
                                </div>
                                <div class="form-text">
                                    Fully-qualified URL for a web page that is external to this web site.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Portal or Web Site
                                </label>
                                <insite:QSiteComboBox runat="server" ID="WebSiteSelector" AllowBlank="true" />
                                <div class="form-text">
                                    The web site that contains this page.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Parent Page
                                </label>
                                <uc:PagePopupSelector runat="server" ID="ParentPageId" />
                                <div class="form-text">
                                    Folders, pages, and sections are organized into a hierarchy.
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

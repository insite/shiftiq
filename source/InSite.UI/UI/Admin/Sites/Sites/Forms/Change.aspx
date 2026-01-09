<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SiteInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="SiteSetupChange" />

        <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-cloud me-1"></i>
            Site
        </h2>

            <div class="row">

                <div class="col-lg-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Details</h3>
                            <uc:Details runat="server" ID="SiteDetails" />
                        </div>
                    </div>
                </div>

                 <div class="col-lg-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Update Site</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" Display="Dynamic" FieldName="Title" ValidationGroup="SiteSetupChange" />
                                </label>
                                <div>
                                    <insite:TextBox ID="TitleInput" runat="server" Width="100%" MaxLength="128" />
                                </div>
                                <div class="form-text">A descriptive title that uniquely identifies this web site.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Domain
                                     <insite:RequiredValidator runat="server" ControlToValidate="Domain" Display="Dynamic" FieldName="Domain" ValidationGroup="SiteSetupChange" />
                                </label>
                                <div>
                                    <insite:TextBox ID="Domain" runat="server" MaxLength="256" Width="100%" />
                                </div>
                            </div>

                        </div>
                     </div>

                </div>

            </div>

        </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="SiteSetupChange" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

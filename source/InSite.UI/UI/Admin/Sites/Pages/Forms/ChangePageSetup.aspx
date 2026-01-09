<%@ Page Language="C#" CodeBehind="ChangePageSetup.aspx.cs" Inherits="InSite.Admin.Sites.Pages.ChangePageSetup" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/PageInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="PageSetupChange" />

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

                        <h3>Update Page</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Type
                                 <insite:RequiredValidator runat="server" ControlToValidate="PageType" FieldName="Page Type" ValidationGroup="PageSetupChange" />
                            </label>
                            <div>
                                <insite:WebPageTypeComboBox ID="PageType" runat="server" AllowBlank="false" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Title
                                <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" Display="Dynamic" FieldName="Title" ValidationGroup="PageSetupChange" />
                            </label>
                            <div>
                                <insite:TextBox ID="TitleInput" runat="server" MaxLength="128" />
                            </div>
                            <div class="form-text">A descriptive title that uniquely identifies this web site.</div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>


    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="PageSetupChange" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

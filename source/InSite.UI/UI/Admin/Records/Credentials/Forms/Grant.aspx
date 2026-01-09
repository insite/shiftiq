<%@ Page Language="C#" CodeBehind="Grant.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Grant" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CredentialDetails.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Credential" />

    <section runat="server" ID="CredentialSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-award me-1"></i>
            Grant Credential
        </h2>

            <div class="row">
                <div class="col-md-7">
                    <uc:Details runat="server" ID="CredentialDetails" />
                </div>

                <div class="col-md-5">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Granted
                                    <insite:RequiredValidator runat="server" FieldName="Granted" ControlToValidate="GrantedDate" ValidationGroup="Credential" />
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector ID="GrantedDate" runat="server" />
                                </div>
                                <div class="form-text">The credential has been granted.</div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Credential" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

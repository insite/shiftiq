<%@ Page Language="C#" CodeBehind="Revoke.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Revoke" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CredentialDetails.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Credential" />

    <section runat="server" ID="CredentialSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-award me-1"></i>
            Revoke Credential
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
                                Revoked
                                <insite:RequiredValidator runat="server" FieldName="Revoked" ControlToValidate="RevokedDate" ValidationGroup="Credential" />
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector ID="RevokedDate" runat="server" />
                            </div>
                            <div class="form-text">When the credential was revoked.</div>
                        </div>

                        <div runat="server" id="ReasonField" class="form-group mb-3">
                            <label class="form-label">
                                Reason
                                <insite:RequiredValidator runat="server" FieldName="Reason" ControlToValidate="Reason" ValidationGroup="Credential" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Reason" TextMode="MultiLine" Width="100%" Height="150px" MaxLength="200" />
                            </div>
                            <div class="form-text">
                                Please provide a short comment to indicate the reason why this credential is revoked.
                            </div>
                        </div>

                        </div>
                    </div>
                </div>
            </div>

    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Credential" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>

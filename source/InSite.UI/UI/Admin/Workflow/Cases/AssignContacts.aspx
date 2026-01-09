<%@ Page Language="C#" CodeBehind="AssignContacts.aspx.cs" Inherits="InSite.Admin.Issues.Forms.AssignUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/CaseInfo.ascx" TagName="CaseInfo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Case" />

    <section class="mb-3">

        <h2 class="h4 mb-3"><i class="far fa-exclamation me-2"></i>Case</h2>

        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CaseInfo runat="server" ID="CaseInfo" />
                    </div>
                </div>
            </div>
            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Update Contacts</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Case Manager
                                <insite:RequiredValidator runat="server" ControlToValidate="ManagerIdentifier" FieldName="Manager" ValidationGroup="Case" />
                            </label>
                            <div>
                                <insite:FindPerson runat="server" ID="ManagerIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Member
                                <insite:RequiredValidator runat="server" ControlToValidate="AssigneeID" FieldName="Member" ValidationGroup="Case" />
                            </label>
                            <div>
                                <insite:FindPerson runat="server" ID="AssigneeID" />
                            </div>
                            <div runat="server" id="AssigneeHelp" class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Current Owner
                            </label>
                            <div>
                                <insite:FindPerson runat="server" ID="OwnerUserIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Lawyer
                            </label>
                            <div>
                                <insite:CaseLawyerComboBox runat="server" ID="LawyerID" Width="100%" />
                            </div>
                        </div>

                        <div>
                            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Case" />
                            <insite:CancelButton runat="server" ID="CancelButton" />
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>

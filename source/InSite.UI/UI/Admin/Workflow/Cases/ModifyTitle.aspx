<%@ Page Language="C#" CodeBehind="ModifyTitle.aspx.cs" Inherits="InSite.Admin.Issues.Issues.Forms.ChangeTitle" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
                        <h3>Update Summary</h3>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Case Summary
                                <insite:RequiredValidator runat="server" ControlToValidate="IssueTitle" ValidationGroup="Case" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="IssueTitle" MaxLength="200" />
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

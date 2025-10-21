<%@ Page Language="C#" CodeBehind="ModifyStatus.aspx.cs" Inherits="InSite.Admin.Issues.Forms.ChangeStatus" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagName="CaseInfo" TagPrefix="uc" Src="./Controls/CaseInfo.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Case" />

    <section class="mb-3">

        <h2 class="h4 mb-3"><i class="far fa-exclamation me-2"></i>Change Status</h2>

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

                        <h3>New Status</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Status
                                <insite:RequiredValidator runat="server" ControlToValidate="IssueStatus" FieldName="New Status" ValidationGroup="Case" />
                            </label>
                            <div>
                                <insite:IssueStatusComboBox runat="server" ID="IssueStatus" ValidationGroup="Case" />
                            </div>
                        </div>
                
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Effective
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector runat="server" ID="IssueStatusEffective" />
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

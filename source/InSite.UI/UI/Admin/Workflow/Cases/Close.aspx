<%@ Page Language="C#" CodeBehind="Close.aspx.cs" Inherits="InSite.Admin.Issues.Forms.Close" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagName="CaseInfo" TagPrefix="uc" Src="./Controls/CaseInfo.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <section class="mb-3">

        <h2 class="h4 mb-3"><i class="far fa-exclamation me-2"></i>Close Case</h2>

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

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Status
                            </label>
                            <div>
                                <insite:IssueStatusRadioList runat="server" ID="IssueStatus" />
                            </div>
                        </div>
                
                        <div>
                            <insite:Button runat="server" ID="CloseIssueButton" Text="Close Case" Icon="fas fa-folder" ButtonStyle="Danger" />
                            <insite:CancelButton runat="server" ID="CancelButton" />
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    

</asp:Content>

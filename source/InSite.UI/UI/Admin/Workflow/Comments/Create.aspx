<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Issues.Comments.Forms.Author" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CommentInfo" Src="Controls/CommentInfo.ascx" %>
<%@ Register TagName="CaseInfo" TagPrefix="uc" Src="../Cases/Controls/CaseInfo.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Comment" />

    <section class="mb-3">
        <div class="row">

            <div class="col-md-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:CaseInfo runat="server" ID="CaseInfo" />

                    </div>
                </div>

            </div>

            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CommentInfo runat="server" ID="CommentInfo" />
                        <div>
                            <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Comment" />
                            <insite:CancelButton runat="server" ID="CancelButton" />
                        </div>

                    </div>
                </div>

            </div>

        </div>

    </section>
</asp:Content>

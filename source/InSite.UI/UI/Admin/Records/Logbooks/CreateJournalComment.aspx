<%@ Page Language="C#" CodeBehind="CreateJournalComment.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.CreateJournalComment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CommentDetail" Src="Controls/CommentDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Comment" />

    <section runat="server" ID="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-comment me-1"></i>
            Comment
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <uc:CommentDetail runat="server" ID="Detail" />

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="Comment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

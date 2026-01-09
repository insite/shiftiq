<%@ Page Language="C#" CodeBehind="CreateComment.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.CreateComment" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="CommentDetail" Src="Controls/CommentDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="card">
        <div class="card-body">
            <h4 class="card-title"><span class="far fa-comment"></span> Comment</h4>
            <uc:CommentDetail runat="server" ID="Detail" />
        </div>
    </div>

    <div class="pt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Comment" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
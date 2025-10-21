<%@ Page Language="C#" CodeBehind="Scores.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Scores" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ScoreControl.ascx" TagName="ScoreControl" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:ScoreControl runat="server" ID="ScoreControl" />
</asp:Content>

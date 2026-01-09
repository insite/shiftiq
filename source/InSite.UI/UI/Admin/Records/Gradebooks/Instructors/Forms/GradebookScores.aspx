<%@ Page Language="C#" CodeBehind="GradebookScores.aspx.cs" Inherits="InSite.Records.Gradebooks.Forms.Scores" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/ScoreControl.ascx" TagName="ScoreControl" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:ScoreControl runat="server" ID="ScoreControl" />
</asp:Content>

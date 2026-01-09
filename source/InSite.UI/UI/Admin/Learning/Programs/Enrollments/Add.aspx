<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Records.Programs.AddUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Messages/Subscribers/Controls/AddControl.ascx" TagName="AddControl" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
   
    <uc:AddControl runat="server" ID="ScreenControl" />

</asp:Content>

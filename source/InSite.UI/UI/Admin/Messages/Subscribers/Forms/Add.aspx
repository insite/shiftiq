<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Messages.Subscribers.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AddControl.ascx" TagName="AddControl" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <uc:AddControl runat="server" ID="ScreenControl" />

</asp:Content>

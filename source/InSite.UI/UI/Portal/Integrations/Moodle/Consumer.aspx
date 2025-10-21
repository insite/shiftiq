<%@ Page Language="C#" CodeBehind="Consumer.aspx.cs" Inherits="InSite.UI.Portal.Integrations.Moodle.Consumer" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        Response.Redirect("https://mypower.panglobal.org/login/index.php");
    }
</script>
</asp:Content>

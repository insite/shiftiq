<%@ Page Language="C#" CodeBehind="ScormExit.aspx.cs" Inherits="InSite.UI.Lobby.ScormExit" MasterPageFile="~/UI/Layout/Lobby/LobbyEmpty.master" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        window.onload = function () {
            window.top.location.href = "<%= RedirectUrl %>";
        }
    </script>
</asp:Content>
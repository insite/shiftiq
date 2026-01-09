<%@ Page Language="C#" CodeBehind="License.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.AcceptLicense" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <section class="p-5">

        <insite:Alert runat="server" ID="ScreenStatus" />

        <div runat="server" id="LicenseContainer"></div>

        <div runat="server" id="ButtonPanel" class="mt-5">
            <insite:Button runat="server" id="AgreeButton" Text="I Agree" Icon="fas fa-check" ButtonStyle="Success" />
            <insite:Button runat="server" id="DisagreeButton" Text="I Do Not Agree" Icon="fas fa-times" ButtonStyle="Danger" />
        </div>

        <div runat="server" id="ImpersonatorPanel" class="m-t-md">
            <div class="alert alert-danger">
                <i class="fas fa-exclamation-triangle"></i>
                <span>
                    You are currently impersonating another user,
                    and you cannot accept the license agreement on their behalf.
                    Please <a href="/ui/portal/home">click here</a> to return to their portal home page.
                </span>
            </div>
        </div>

    </section>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        header.navbar {
            display: none;
        }

        body {
            margin-bottom: 0 !important;
        }
    </style>
    <script type="text/javascript">
        (function () {
            var header = document.querySelector('header.navbar');
            if (header)
                header.remove();
        })();
    </script>
</asp:Content>

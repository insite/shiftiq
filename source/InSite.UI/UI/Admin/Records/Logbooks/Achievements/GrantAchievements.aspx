<%@ Page Language="C#" CodeBehind="GrantAchievements.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Achievements.GrantAchievements" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/UserGrantGrid.ascx" TagName="UserGrantGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenAlert" />

    <section runat="server" id="MainPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-users me-1"></i>
            Learners
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <uc:UserGrantGrid runat="server" ID="Users" />

            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" Text="Case" />
    <insite:CancelButton runat="server" ID="CancelButton" />

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    (function () {
        $(".root-checkbox input[type=checkbox]").on("click", function () {
            var $this = $(this);
            var checked = this.checked;

            $this.closest("table").find(".field-checkbox input[type=checkbox]").prop("checked", checked);
        });
    })();
</script>
</insite:PageFooterContent>
</asp:Content>

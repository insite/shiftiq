<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalAchievementExporter.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.Files.GlobalAchievementExporter" %>

<asp:PlaceHolder ID="AchievementPanel" runat="server" Visible="false">

    <asp:PlaceHolder ID="NoResultPanel" runat="server">
        There are no global <strong><asp:Literal ID="AchievementTypeName1" runat="server" /></strong> achievements. 
        You must select an existing organization-specific achievement or create a new achievement.
    </asp:PlaceHolder>

    <asp:Panel ID="ResultPanel" runat="server">
        <strong class="mb-1">
            Global <asp:Literal ID="AchievementTypeName2" runat="server" />
        </strong>
        <div class="form-text mb-3">
            To create a organization-specific copy of a global achievement, select it from the list and click Duplicate.
        </div>

        <asp:Panel ID="OtherAchievementsWarning" runat="server" CssClass="mb-3">
            <insite:Icon runat="server" Type="Regular" Name="exclamation-triangle" CssClass="me-2" />
            Remember that Other Achievements are not displayed in the Training Plan.
        </asp:Panel>

        <asp:ListBox ID="Achievements" runat="server" Rows="10" CssClass="w-100 mb-3" />

        <insite:Button ID="CopyButton" runat="server" Icon="fas fa-copy" Text="Duplicate" ButtonStyle="Default" />
        <insite:Button ID="UnselectButton" runat="server"
            Icon="far fa-square"
            Text="Unselect"
            ButtonStyle="Default"
            Enabled="false"
            OnClientClick="return !this.getAttribute('disabled');"
        />
    </asp:Panel>

</asp:PlaceHolder>

<script type="text/javascript">
    function Achievements_onclick(sender, unselectButtonId, disableFunctionName)
    {
        if (sender.selectedIndex >= 0)
        {
            $get(unselectButtonId).removeAttribute('disabled');
            $get(unselectButtonId).classList.remove("disabled");
            inSite.common.execFuncByName(disableFunctionName);
        }
    }

    function UnselectButton_onclick(sender, achievementsId, enableFunctionName)
    {
        $get(achievementsId).selectedIndex = -1;
        sender.setAttribute('disabled', 'disabled');
        sender.classList.add("disabled");

        inSite.common.execFuncByName(enableFunctionName);
    }
</script>
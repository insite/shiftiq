<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementListEditor.ascx.cs" Inherits="InSite.UI.CMDS.Common.Controls.User.AchievementListEditor" %>

<%@ Register TagPrefix="uc" TagName="AchievementListGrid" Src="~/UI/CMDS/Common/Controls/User/AchievementListGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="AchievementListGroup" Src="~/UI/CMDS/Common/Controls/User/AchievementListGroup.ascx" %>





        <div class="input-group mb-3">
            <insite:TextBox runat="server" ID="SearchText" />
            <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="OutlineSecondary" Text="" Size="Default" />
            <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="OutlineSecondary" Text="" Size="Default" />
            <insite:ComboBox runat="server" ID="AccountScope" Width="250px">
                <Items>
                    <insite:ComboBoxOption Value="Organization" Text="Organization-Specific" />
                    <insite:ComboBoxOption Value="Enterprise" Text="Global" />
                    <insite:ComboBoxOption Value="Partition" Text="All" />
                </Items>
            </insite:ComboBox>
            <insite:ComboBox runat="server" ID="GroupByComboBox" Width="220px" ButtonStyle="Primary">
                <Items>
                    <insite:ComboBoxOption Value="Type" Text="Type" />
                    <insite:ComboBoxOption Value="TypeAndCategory" Text="Type and Category" />
                    <insite:ComboBoxOption Value="None" Text="No Grouping" />
                </Items>
            </insite:ComboBox>
        </div>

<insite:Nav runat="server">

    <insite:NavItem runat="server" ID="AchievementTab" Title="Achievements">
        
        <div runat="server" id="AchievementNoMatch" class="alert alert-info mb-0">
            There are no achievements matching your search criteria.
        </div>

        <div class="mb-3">
            <div runat="server" id="AchievementCount" class="my-3"></div>
        </div>

        <uc:AchievementListGroup runat="server" ID="AchievementGroups" Mode="Delete" />

        <uc:AchievementListGrid runat="server" ID="AchievementGrid" />

        <div runat="server" id="ButtonsPanel" class="mt-4">
            <div class="fw-bold mb-2">Commands for all checkboxes:</div>
            <div>
                <insite:Button runat="server" ID="SelectAllButton" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                <insite:Button runat="server" ID="UnselectAllButton" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                <insite:DeleteButton ID="DeleteAchievementButton" runat="server" />
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="NewAchievementTab" Title="Add Achievements">

        <div runat="server" id="NewAchievementNoMatch" class="alert alert-info mb-0">
            There are no achievements matching your search criteria.
        </div>

        <div class="mb-3">
            <span runat="server" id="NewAchievementCount" class="my-3"></span>
        </div>
        
        <uc:AchievementListGroup runat="server" ID="NewAchievementGroups" Mode="Add" />

        <uc:AchievementListGrid runat="server" ID="NewAchievementGrid" />

        <div runat="server" id="NewButtonsPanel" class="mt-4">
            <div class="fw-bold mb-2">Commands for all checkboxes:</div>
            <div>
                <insite:Button runat="server" ID="SelectAllButton2" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                <insite:Button runat="server" ID="UnselectAllButton2" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                <insite:Button runat="server" ID="AddAchievementButton" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" DisableAfterClick="true" />
            </div>
        </div>

    </insite:NavItem>

</insite:Nav>





<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= SearchText.ClientID %>')
                    .off('keydown', onKeyDown)
                    .on('keydown', onKeyDown);
            });

            function onKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= FilterButton.ClientID %>')[0].click();
                }
            }
        })();
    </script>
</insite:PageFooterContent>

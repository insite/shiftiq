<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementCriteriaSelector.ascx.cs" Inherits="InSite.Cmds.Controls.Reporting.Report.AchievementCriteriaSelector" %>

<asp:Repeater ID="Repeater" runat="server">
    <ItemTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                <%# Eval("Label") %>
            </label>
            <insite:ComboBox runat="server" ID="ModeSelector" AllowBlank="false" AutoPostBack="true" CssClass="mb-2">
                <Items>
                    <insite:ComboBoxOption Value="None" Text="None" />
                    <insite:ComboBoxOption Value="All" Text="All" />
                    <insite:ComboBoxOption Value="Specific" Text="Specific" />
                </Items>
            </insite:ComboBox>
            <insite:FindEntity runat="server" ID="AchievementSelector" MaxSelectionCount="0" EntityName="Achievement" PageSize="10" />
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
<insite:Button ID="DeselectAllButton" runat="server" Icon="far fa-square" Text="Clear All" ButtonStyle="OutlinePrimary" />

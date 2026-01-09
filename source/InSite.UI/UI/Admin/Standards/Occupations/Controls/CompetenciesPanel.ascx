<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesPanel.ascx.cs" Inherits="InSite.Admin.Standards.Occupations.Controls.CompetenciesPanel" %>

<%@ Register Src="~/UI/Admin/Standards/Occupations/Controls/CompetenciesNode.ascx" TagName="CompetenciesNode" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Occupations/Controls/CompetenciesSelector.ascx" TagName="CompetenciesSelector" TagPrefix="uc" %>

<insite:Alert runat="server" ID="Status" />

<div class="row">
    <div class="col-lg-12">
        <insite:Nav runat="server">
            <insite:NavItem runat="server" ID="CompetenciesTab" Title="Competencies">
                <div runat="server" id="NoCompetenciesAlert" class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This <span runat="server" id="AssetType" /> does not yet contain any competencies.
                </div>

                <div id="CommandButtons" runat="server" class="reorder-trigger reorder-hide pb-2">
                    <insite:Button runat="server" ID="SelectAllButton" ButtonStyle="Default" ToolTip="Select All" Icon="far fa-square" />
                    <insite:Button runat="server" ID="UnselectAllButton" ButtonStyle="Default" ToolTip="Deselect All" style="display:none;" Icon="far fa-check-square" />
                    <insite:Button runat="server" ID="PreDeleteButton" ButtonStyle="Default" ToolTip="Delete Selected Competencies" Icon="fas fa-trash-alt" />
                </div>

                <div runat="server" id="CompetencyPanel">
                    <asp:Repeater runat="server" ID="NodeRepeater">
                        <HeaderTemplate>
                            <div style="margin-left:-32px;">
                        </HeaderTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                        <ItemTemplate>
                            <uc:CompetenciesNode runat="server" ID="CompetenciesNode" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </insite:NavItem>

            <insite:NavItem runat="server" ID="AddCompetenciesTab" Title="Add Competencies">

                <uc:CompetenciesSelector runat="server" ID="CompetenciesSelector" />

                <div class="row" style="margin-top:15px;">
                    <div class="col-md-12">
                        <insite:SaveButton runat="server" ID="SaveCompetenciesButton" />
                    </div>
                </div>

            </insite:NavItem>

        </insite:Nav>
    </div>
</div>

<asp:Button runat="server" ID="DeleteCompetenciesButton" style="display:none;" />

<insite:PageFooterContent runat="server">
<script type="text/javascript">

    (function () {
        Sys.Application.add_load(onLoad);

        function onLoad() {
            $("#<%= PreDeleteButton.ClientID %>")
                .off('click', onDeleteCompetencies)
                .on('click', onDeleteCompetencies);

            $("#<%= SelectAllButton.ClientID %>")
                .off('click', onSelectAll)
                .on('click', onSelectAll);

            $("#<%= UnselectAllButton.ClientID %>")
                .off('click', onUnselectAll)
                .on('click', onUnselectAll);

            $("#<%= CompetencyPanel.ClientID %> input[id$='AllSelected']")
                .off('click', onSectionSelect)
                .on('click', onSectionSelect);
        }

        function onDeleteCompetencies() {
            if ($("#<%= CompetencyPanel.ClientID %> input[id$='IsSelected']:checked").length > 0) {
                if (confirm('Are you sure you want to delete selected competencies?'))
                    __doPostBack('<%= DeleteCompetenciesButton.UniqueID %>', '');
            }
            else {
                alert("Please select the competencies you want to delete.");
            }

            return false;
        }

        function onSelectAll() {
            $('#<%= UnselectAllButton.ClientID %>').css('display', '');
            $('#<%= SelectAllButton.ClientID %>').css('display', 'none');
            return setCheckboxes('<%= CompetencyPanel.ClientID %>', true);
        }

        function onUnselectAll() {
            $('#<%= UnselectAllButton.ClientID %>').css('display', 'none');
            $('#<%= SelectAllButton.ClientID %>').css('display', '');
            return setCheckboxes('<%= CompetencyPanel.ClientID %>', false);
        }

        function onSectionSelect() {
            $(this)
                .closest("table")
                .find("input[type=checkbox]")
                .not(this)
                .prop("checked", this.checked);
        }
    })();

</script>
</insite:PageFooterContent>
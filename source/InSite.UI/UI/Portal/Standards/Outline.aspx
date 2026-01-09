<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Standards.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/OutlineTree.ascx" TagName="OutlineTree" TagPrefix="uc" %>
<%@ Register Src="Controls/OutlineTreeScript.ascx" TagName="OutlineTreeScript" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Container runat="server" ID="ContentContainer">

        <div class="row mb-4">
            <div class="col-md-12">
                <div class="float-end">
                    <insite:Button runat="server" ID="ExpandAllButton" Text="Expand All" Icon="fas fa-chevron-down" PostBackEnabled="false" ButtonStyle="Default" />
                    <insite:Button runat="server" ID="CollapseAllButton" Text="Collapse All" Icon="fas fa-chevron-up" PostBackEnabled="false" ButtonStyle="Default" />
                </div>
                <div class="d-inline-block me-3">
                    <insite:CheckSwitch runat="server" ID="DisplayStatusSwitch" Text="Display Status" />
                </div>
            </div>
        </div>

        <insite:Accordion runat="server">
            <insite:AccordionPanel runat="server" ID="AccordionPanel" Title="Test" IsTitleLocalizable="false">
                <div runat="server" id="PageSubtitle" class="mb-4"></div>

                <p runat="server" id="SummaryHtml"></p>

                <uc:OutlineTreeScript runat="server" ID="OutlineTreeScript" />
                <uc:OutlineTree runat="server" ID="OutlineTree" />
            </insite:AccordionPanel>
        </insite:Accordion>

    </insite:Container>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                const instance = window.outlineTree.<%= OutlineTree.ClientID %>;

                const $switch = $('#<%= DisplayStatusSwitch.ClientID %>')
                    .css({ transition: 'none' })
                    .prop('checked', instance.isShowCompetencyStatus)
                    .on('change', (e) => instance.isShowCompetencyStatus = $(e.currentTarget).prop('checked'));

                setTimeout(function ($switch) { $switch.css({ transition: '', visibility: '' }); }, 10, $switch);

                var $expandTree = $('#<%= ExpandAllButton.ClientID %>');
                var $collapseTree = $('#<%= CollapseAllButton.ClientID %>');

                if (instance.allowExpandTree) {
                    $expandTree.on('click', (e) => instance.expandAll(e));
                    $collapseTree.on('click', (e) => instance.collapseAll(e));
                } else {
                    $expandTree.remove();
                    $collapseTree.remove();
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

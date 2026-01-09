<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCompetencySummary.ascx.cs" Inherits="InSite.UI.Portal.Home.Controls.UserCompetencySummary" %>

<%@ Register Src="~/UI/Portal/Standards/Controls/OutlineTree.ascx" TagName="OutlineTree" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Standards/Controls/OutlineTreeScript.ascx" TagName="OutlineTreeScript" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Records/Logbooks/Controls/CompetencyProgressGrid.ascx" TagName="CompetencyProgressGrid" TagPrefix="uc" %>

<uc:OutlineTreeScript runat="server" ID="OutlineTreeScript" />

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <div class="accordion position-relative" id='<%= Repeater.ClientID %>'>
    </HeaderTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
    <ItemTemplate>
        <div class="accordion-item">
            <h2 class="accordion-header" id='<%# Repeater.ClientID + "_H" + Container.ItemIndex %>'>
                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target='<%# "#" + Repeater.ClientID + "_C" + Container.ItemIndex %>' aria-expanded="false" aria-controls="<%# Repeater.ClientID + "_C" + Container.ItemIndex %>">
                    <%# Eval("FrameworkTitle") %>
                </button>
            </h2>
            <div class="accordion-collapse collapse" id="<%# Repeater.ClientID + "_C" + Container.ItemIndex %>" aria-labelledby="<%# Repeater.ClientID + "_H" + Container.ItemIndex %>" data-bs-parent='<%# "#" + Repeater.ClientID %>' data-index='<%# Container.ItemIndex %>'>
                <div class="accordion-body">
                    <p>
                        <%# Eval("FrameworkSummary") %>
                    </p>

                    <uc:CompetencyProgressGrid runat="server" ID="ProgressGrid" />

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OutlineTreeUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="OutlineTreeUpdatePanel">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="OutlineTreePanel" Visible="false">
                                <div class="my-3 d-flex align-items-center">
                                    <div>
                                        <insite:CheckSwitch runat="server" ID="DisplayStatusSwitch" Text="Display Status" CssClass="mb-0" />
                                    </div>
                                    <div class="ms-auto">
                                        <insite:Button runat="server" ID="ExpandAllButton" Text="Expand All" Icon="fas fa-chevron-down" PostBackEnabled="false" ButtonStyle="Default" />
                                        <insite:Button runat="server" ID="CollapseAllButton" Text="Collapse All" Icon="fas fa-chevron-up" PostBackEnabled="false" ButtonStyle="Default" />
                                    </div>
                                </div>

                                <uc:OutlineTree runat="server" ID="OutlineTree" AllowSaveState="false" Visible="false" />
                            </asp:Panel>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            var data = <%= Newtonsoft.Json.JsonConvert.SerializeObject(TreeData) %>;
            if (!data)
                return;

            var instance = window.userCompetencySummary = window.userCompetencySummary || { outlineTreeLoaded: outlineTreeLoaded };

            $('#<%= Repeater.ClientID %> > .accordion-item > .accordion-collapse').on('show.bs.collapse', onCollapseShow);

            function onCollapseShow() {
                var index = $(this).data('index');
                var item = data[index];

                if (!item || item.loaded === true)
                    return;

                if (!document.getElementById(item.panel))
                    document.getElementById(item.update).ajaxRequest('load');

                item.loaded = true;
            }

            function outlineTreeLoaded(i) {
                const item = data[i];

                if (!item || item.loaded !== true || item.inited === true || !document.getElementById(item.panel))
                    return;

                const instance = outlineTree[item.tree];

                const $switch = $(document.getElementById(item.displayStatus))
                if (instance) {
                    instance.isShowCompetencyStatus = true;

                    $switch
                        .css({ transition: 'none' })
                        .prop('checked', instance.isShowCompetencyStatus)
                        .on('change', (e) => instance.isShowCompetencyStatus = $(e.currentTarget).prop('checked'));

                    setTimeout(function ($switch) { $switch.css({ transition: '', visibility: '' }); }, 10, $switch);
                } else {
                    $switch.remove();
                }

                const $expandTree = $(document.getElementById(item.expand));
                const $collapseTree = $(document.getElementById(item.collapse));

                if (instance && instance.allowExpandTree) {
                    $expandTree.on('click', (e) => instance.expandAll(e));
                    $collapseTree.on('click', (e) => instance.collapseAll(e));
                } else {
                    $expandTree.remove();
                    $collapseTree.remove();
                }

                item.inited = true;
            }

            $(document).ready(() => {
                for (var i = 0; i < data.length; i++)
                    outlineTreeLoaded(i);
            });
        })();
    </script>
</insite:PageFooterContent>

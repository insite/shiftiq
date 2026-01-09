<%@ Page Language="C#" CodeBehind="Manage.aspx.cs" Inherits="InSite.UI.Admin.Standards.Manage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="OutlineTreeNodeRepeater" Src="~/UI/Admin/Standards/Standards/Controls/OutlineTreeNodeRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="OutlineEditor" />
    <insite:ValidationSummary runat="server" ValidationGroup="OutlineCreator" />

    <section runat="server" ID="OutlineSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-sitemap me-1"></i>
            Manage Standards
        </h2>

        <insite:Button runat="server" ID="PrintButton" ButtonStyle="Default" Icon="far fa-print" Text="Print" />

        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-lg-12">
                        <div runat="server" id="ViewModeSelector" class="d-inline-block me-3">
                            <insite:RadioButton runat="server" CssClass="d-inline-block me-2" GroupName="view-mode" Checked="true" Text="View" data-mode="view"/>

                            <insite:Container runat="server" ID="EditViewButtons">
                                <insite:RadioButton runat="server" CssClass="d-inline-block me-2" GroupName="view-mode" Text="Edit" data-mode="edit"/>
                                <insite:RadioButton runat="server" CssClass="d-inline-block me-2" GroupName="view-mode" Text="Translate" data-mode="translate"/>
                            </insite:Container>
                        </div>

                        <insite:Button runat="server" ID="ExpandAllButton" ButtonStyle="Default" Icon="fas fa-chevron-down" Text="Expand All" />
                        <insite:ComboBox runat="server" ID="ExpandLevelSelector" Width="120px" ButtonSize="Small" />
                        <insite:Button runat="server" ID="CollapseAllButton" ButtonStyle="Default" Icon="fas fa-chevron-up" Text="Collapse All" />
                        <insite:ComboBox runat="server" ID="ShowSummarySelector" Width="175px" ButtonSize="Small" />
                    </div>
                </div>

            <div class="row row-content view-tree">
                <div class="col-lg-12 tree-view-container">
                    <uc:OutlineTreeNodeRepeater runat="server" ID="NodeRepeater" />
                </div>

                <div class="col-lg-6 editor-container">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="EditorUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="EditorUpdatePanel">
                        <ContentTemplate>
                            <insite:DynamicControl runat="server" ID="EditorContainer" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>

            <div runat="server" id="FooterRow" class="row" visible="false">
                <div class="col-lg-12">
                    <p>
                        <small class='text-body-secondary'>
                            <i class='fa fa-check text-success'></i> <asp:Literal runat="server" ID="FooterText" />
                        </small>
                    </p>
                </div>
            </div>

            </div>
        </div>
    </section>

    <div class="d-none">
        <asp:Button runat="server" ID="UpdateTreeButton" />
    </div>

    <insite:Modal runat="server" ID="InfoWindow" />

    <insite:PageHeadContent runat="server">
        <link type="text/css" rel="stylesheet" href="/UI/Admin/standards/standards/content/styles/outline.css?v=1">
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server" ID="CommonScript">
        <script type="text/javascript">
            (function () {
                var outline = window.outline = window.outline || {};

                outline.settings = {
                    stateKey: 'standards.outline.state.<%= CurrentData.RootNumber %>',

                    expandAllButtonSelector: '#<%= ExpandAllButton.ClientID %>',
                    collapseAllButtonSelector: '#<%= CollapseAllButton.ClientID %>',
                    expandLevelComboBoxSelector: '#<%= ExpandLevelSelector.ClientID %>',
                    showSummarySelector: '#<%= ShowSummarySelector.ClientID %>',
                    viewModeSelector: '#<%= ViewModeSelector.ClientID %>',
                    editorUpdatePanelId: '<%= EditorUpdatePanel.ClientID %>',
                    infoWindowId: '<%= InfoWindow.ClientID %>',

                    reloadTree: function () {
                        document.getElementById('<%= UpdateTreeButton.ClientID %>').click();
                    },
                };
            })();
        </script>

        <script src="/UI/Admin/standards/standards/content/scripts/outline.js?v=1"></script>
    </insite:PageFooterContent>
</asp:Content>

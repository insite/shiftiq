<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Assessments.Outlines.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="BankSection" Src="../Controls/BankSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionsSection" Src="../Controls/QuestionsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="SpecificationsSection" Src="../Controls/SpecificationsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="FormsSection" Src="../Controls/FormsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentsSection" Src="../Controls/CommentsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="AttachmentsSection" Src="../Controls/AttachmentsSection.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="BankTab" Title="Bank" Icon="far fa-balance-scale" IconPosition="BeforeText">

            <div class="my-3">
                <insite:Button runat="server" ID="NewBankLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/assessments/banks/create" />

                <insite:ButtonSpacer runat="server" ID="NewBankSpacer" />

                <insite:Button runat="server" ID="PrintBankButton" Text="Print" Icon="fas fa-print" ButtonStyle="Default" />
                <insite:Button runat="server" ID="DuplicateBankLink" Visible="false" Text="Duplicate" Icon="fas fa-copy" ButtonStyle="Default" />
                <insite:Button runat="server" ID="MigrateBankLink" visible="false" Icon="fas fa-file-export" Text="Migrate" ButtonStyle="Default" />
                <insite:Button runat="server" ID="TranslateLink" Visible="false" Text="Translate" Icon="fas fa-globe" ButtonStyle="Default" />
                    
                <insite:ButtonSpacer runat="server" />

                <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                <insite:DownloadButton runat="server" ID="DownloadBankLink" Text="Download JSON" />
                <insite:DownloadButton runat="server" ID="DownloadSummariesButton" Text="Download Summaries" />
                
            </div>

            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:BankSection runat="server" ID="BankSection" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="QuestionsTab" Title="Questions" Icon="far fa-question" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:QuestionsSection runat="server" ID="QuestionsSection" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SpecificationsTab" Title="Specifications" Icon="far fa-clipboard-list" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:SpecificationsSection runat="server" ID="SpecificationsSection" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="FormsTab" Title="Forms" Icon="far fa-window" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:FormsSection runat="server" ID="FormsSection" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentsTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:CommentsSection runat="server" ID="CommentsSection" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AttachmentsTab" Title="Attachments" Icon="far fa-paperclip" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:AttachmentsSection runat="server" ID="AttachmentsSection" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                const instance = window.bankRead = window.bankRead || {};

                let isTabSync = false;

                instance.syncTabs = function (tab, navs) {
                    if (isTabSync)
                        return;

                    isTabSync = true;

                    try {
                        const $tab = $(tab);
                        const index = $tab.parent().index();

                        for (let i = 0; i < navs.length; i++) {
                            const nav = navs[i];

                            if (nav.length == 0 || index >= nav.length)
                                continue;

                            const $navTab = nav.eq(index);
                            if ($navTab.is($tab))
                                continue;

                            $navTab.click();
                        }

                    } finally {
                        isTabSync = false;
                    }
                };

                instance.scrollToQuestion = function (id) {
                    const $questionRow = $('table.question-grid:visible > tbody > tr[data-question="' + String(id) + '"]');
                    if ($questionRow.length !== 1)
                        return;

                    const headerHeight = $('header.navbar:first').outerHeight();
                    let scrollTo = $questionRow.offset().top - headerHeight;

                    if (scrollTo < 0)
                        scrollTo = 0;

                    $('html, body').animate({ scrollTop: scrollTo }, 250);
                };
            })();

            (function () {
                const tabs = [];

                function init() {
                    let $activeTab = null;

                    $('#sets-nav-content > .tab-content > .tab-pane > div > ul.nav-tabs').each(function () {
                        const $this = $(this);

                        tabs.push($this.find('> li > button').on('show.bs.tab', onTabShow));

                        if ($this.is(':visible')) {
                            const $tab = $this.find('> li > button.active');
                            if ($tab.length === 1)
                                $activeTab = $tab;
                        }
                    });

                    if ($activeTab != null)
                        bankRead.syncTabs($activeTab, tabs);
                }

                init();

                function onTabShow() {
                    bankRead.syncTabs(this, tabs);
                }
            })();

            (function () {
                const tabs = [];

                function init() {
                    let $activeTab = null;
                    
                    $('#spec-nav-content > .tab-content > .tab-pane > div > ul.nav-tabs').each(function () {
                        const $this = $(this);

                        tabs.push($this.find('> li > button').on('show.bs.tab', onTabShow));

                        if ($this.is(':visible')) {
                            const $tab = $this.find('> li > button.active');
                            if ($tab.length === 1)
                                $activeTab = $tab;
                        }
                    });

                    if ($activeTab != null)
                        bankRead.syncTabs($activeTab, tabs);
                }

                init();

                function onTabShow() {
                    bankRead.syncTabs(this, tabs);
                }
            })();

            (function () {
                let tabs = [];

                function initTabs() {
                    tabs = [];

                    $('#forms-nav-content > .tab-content > .tab-pane > div > div > ul.nav-tabs').each(function () {
                        tabs.push($(this).find('> li > button').on('show.bs.tab', onTabShow));
                    });
                }

                function onTabShow() {
                    bankRead.syncTabs(this, tabs);
                }

                Sys.Application.add_load(function () {
                    initTabs();
                });
            })();
        </script>
    </insite:PageFooterContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (typeof URLSearchParams !== 'undefined') {
                    const location = window.location;
                    const queryParams = new URLSearchParams(location.search);
                    const scroll = queryParams.get('scroll');

                    if (scroll) {
                        try {
                            const scrollTop = inSite.common.base64.toInt(scroll);

                            $(document).ready(function () {
                                $(window).scrollTop(scrollTop);
                            });

                            queryParams.delete('scroll');

                            const path = location.pathname + '?' + queryParams.toString();
                            const url = location.origin + path;

                            window.history.replaceState({}, '', url);

                            $('form#aspnetForm').attr('action', path);
                        } catch (e) {

                        }
                    }
                }

                $('a.scroll-send').on('click', function () {
                    let scroll = $(window).scrollTop();

                    if (scroll > 0)
                        scroll = inSite.common.base64.fromInt(Math.floor(scroll));
                    else
                        scroll = null;

                    const url = $(this).attr('href');

                    $(this).attr('href', inSite.common.updateQueryString('scroll', scroll, url));
                });
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
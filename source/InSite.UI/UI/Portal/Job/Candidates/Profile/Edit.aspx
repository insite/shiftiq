<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Edit" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/JobInterestSection.ascx" TagName="JobInterestSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/LanguageAbilitySection.ascx" TagName="LanguageAbilitySection" TagPrefix="uc" %>
<%@ Register Src="./Controls/ExperienceSection.ascx" TagName="ExperienceSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/EducationSection.ascx" TagName="EducationSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/DocumentSection.ascx" TagName="DocumentSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/AchievementSection.ascx" TagName="AchievementSection" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <insite:Nav runat="server">
        <insite:NavItem ID="ProfileTab" runat="server" Title="Profile" Icon="far fa-fw fa-id-card">

            <uc:JobInterestSection runat="server" ID="JobInterest" />

            <uc:LanguageAbilitySection runat="server" ID="LanguageAbility" />

        </insite:NavItem>
        <insite:NavItem ID="EducationTab" runat="server" Title="Experience and Education" Icon="far fa-fw fa-user-graduate">

            <uc:ExperienceSection runat="server" ID="Experience" />

            <uc:EducationSection runat="server" ID="Education" />

            <uc:DocumentSection runat="server" ID="Document" />

        </insite:NavItem>
        <insite:NavItem ID="CredentialsTab" runat="server" Title="Credentials" Icon="far fa-fw fa-trophy">

            <uc:AchievementSection runat="server" ID="Achievement" />

        </insite:NavItem>
    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Profile" />
        <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/job/candidates/profile/view" />
    </div>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .card.card-locked {
            }

                .card.card-locked > .card-body {
                    pointer-events: none;
                    opacity: 0.2;
                }

                .card.card-locked > .loading-panel {
                    display: block;
                    cursor: not-allowed;
                }

                    .card.card-locked > .loading-panel > div > div {
                        background-color: transparent;
                    }

                        .card.card-locked > .loading-panel > div > div > .alert {
                            margin-left: 1.5rem;
                            margin-right: 1.5rem;
                        }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                const instance = window.profileDetail = window.profileDetail || {};

                // File Upload

                $('a[data-upload]').on('click', onUploadClick);

                $('a[data-view]').each(function () {
                    const $this = $(this);

                    const targetUrl = $(String($this.data('view'))).val();
                    if (isUrlString(targetUrl)) {
                        $this.data('url', targetUrl).on('click', onViewClick);
                    } else {
                        $this.hide();
                    }
                });

                instance.onFileUploaded = function () {
                    const $view = $(this).closest('.file-upload').parent().parent().find('a[data-view]');
                    if ($view.length !== 1)
                        return;

                    $(String($view.data('view'))).val(inSite.common.fileUploadV2.getFileName(this.id));
                    $view.hide();
                };

                instance.onFileUploadFailed = function () {
                    alert('Error: Upload failed.');
                };

                function onUploadClick(e) {
                    e.preventDefault();

                    const uploadSelector = $(this).data('upload');
                    if (typeof uploadSelector === 'string') {
                        inSite.common.fileUploadV2.trigger(uploadSelector);
                    }
                }

                function onViewClick(e) {
                    e.preventDefault();

                    const url = $(this).data('url');
                    if (!isUrlString(url))
                        return;

                    const win = window.open(url, '_blank');
                    win.focus();
                }

                function isUrlString(value) {
                    return typeof value === 'string' && value.length > 0 && (value[0] === '/' || value.startsWith('http'));
                }
            })();

            (function () {
                const $el = $(window.location.hash);
                if ($el.length != 1)
                    return;

                const $tabPane = $el.closest('div.tab-pane');
                if ($tabPane.length != 1 || $tabPane.is(':visible'))
                    return;

                var $button = $('button[data-bs-target="#' + $tabPane.attr('id') + '"][data-bs-toggle]');
                if ($button.length != 1)
                    return;

                bootstrap.Tab.getOrCreateInstance($button[0]).show()
            })();

            (function () {
                const instance = window.profileDetail = window.profileDetail || {};
                const lockIds = [
                    '<%= JobInterest.ClientID %>',
                    '<%= LanguageAbility.ClientID %>',
                    '<%= Document.ClientID %>'
                ];
                const lockWarningHtml = `
<div class="loading-panel" style="display: block;">
    <div>
        <div>
            <div class="alert alert-warning" role="alert">
                Please add your Experience and Education entries first, then complete your Profile.
            </div>
        </div>
    </div>
</div>`;

                instance.setLocked = function (isLocked) {
                    for (var i = 0; i < lockIds.length; i++) {
                        var $card = $('.card#' + lockIds[i]);

                        if (isLocked) {
                            if (!$card.hasClass('card-locked'))
                                $card.addClass('card-locked').prepend($(lockWarningHtml));
                        } else {
                            if ($card.hasClass('card-locked'))
                                $card.removeClass('card-locked').find('> .loading-panel').remove();
                        }

                    }
                };
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>
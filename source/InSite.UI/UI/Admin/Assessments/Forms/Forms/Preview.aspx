<%@ Page Language="C#" CodeBehind="Preview.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Preview" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="PreviewSectionPanel" Src="~/UI/Admin/Assessments/Forms/Controls/PreviewSectionPanel.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" id="PreviewSection" visible="false">
        <h2 runat="server" id="FormTitle"></h2>
        <div runat="server" id="IntroductionHtml" class="mb-4" visible="false"></div>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Nav runat="server" ID="SectionNav" Visible="false">

                </insite:Nav>

                <asp:Repeater runat="server" ID="SectionRepeater" Visible="false">
                    <ItemTemplate>
                        <h3 runat="server" visible='<%# (bool)Eval("HasTitle") %>'><%# Eval("Title") %></h3>
                        <uc:PreviewSectionPanel runat="server" ID="SectionPanel" />
                    </ItemTemplate>
                </asp:Repeater>

                <uc:PreviewSectionPanel runat="server" ID="SectionPanel" Visible="false" />

            </div>
        </div>

        <div class="mt-3">
            <insite:NextButton runat="server" ID="NextTabButton" Visible="false" />
            <insite:NextButton runat="server" ID="NextQuestionButton" Visible="false" />
            <insite:CloseButton runat="server" ID="CloseButton" />
        </div>
    </section>

    <insite:PageHeadContent runat="server">
        <insite:ResourceLink runat="server" Type="Css" Url="/UI/Admin/assessments/questions/content/styles/preview.css" />

        <style type="text/css">
            .single-question .tab-content .card-question {
                display: none;
            }

            .single-question .tab-content .card-question.active {
                display: inherit;
            }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <insite:ResourceBundle runat="server" Type="JavaScript">
            <Items>
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.js" />
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.likert.js" />
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.hotspot.js" />
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.hotspot.js" />
            </Items>
        </insite:ResourceBundle>

        <script type="text/javascript">

            $(function () {
                const imgUrlPattern = new RegExp('<%= InSite.UI.Portal.Assessments.Attempts.Utilities.AttemptImageInfo.ImageUrlPattern %>');
                const images = <%= Shift.Common.JsonHelper.SerializeJsObject(Images) %>;

                $('.card-question > .card-header > .question-text img').each(function () {
                    const $this = $(this);
                    const imgSrc = $this.prop('src').toLowerCase();
                    let imgKey = imgSrc;
                    
                    const imgUrlMatches = imgUrlPattern.exec(imgKey);
                    if (imgUrlMatches != null)
                        imgKey = imgUrlMatches[3] + imgUrlMatches[5];

                    const $anchor = $('<a>')
                        .addClass('img-zoom')
                        .attr('href', imgSrc);

                    if (images.hasOwnProperty(imgKey))
                        $anchor.attr('data-width', images[imgKey].width);

                    let caption = $this.prop('title');
                    if (!caption)
                        caption = $this.prop('alt');
                    if (caption)
                        $anchor.attr('data-caption', caption);

                    $this.wrap($anchor).tooltip({
                        title: caption,
                        placement: 'right',
                        delay: {
                            show: 500,
                            hide: 100
                        }
                    });
                });

                $('.card-question > .card-header > .question-text .img-zoom').fancybox({});

                $('.card-question > .card-body  table.table-option > tbody > tr > td.text')
                    .each(function () {
                        const $cell = $(this);
                        const $row = $(this).closest('tr');

                        let $input = $row.find('> td.input input[type="radio"]');
                        if ($input.length !== 1)
                            $input = $row.find('> td.input input[type="checkbox"]');

                        if ($input.length !== 1)
                            return;

                        $cell.data('$input', $input);
                    }).on('click', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        const $input = $(this).data('$input');
                        if ($input instanceof jQuery && $input.length === 1)
                            $input.click();
                    });
            });

            $(function () {
                const $nextTabButton = $('#<%= NextTabButton.ClientID %>');
                const $nextQuestionButton = $('#<%= NextQuestionButton.ClientID %>');

                if ($nextTabButton.length !== 1 && $nextQuestionButton.length !== 1)
                    return;

                const $sectionNav = $('#<%= SectionNav.ClientID %>');

                let nextTabId = null;
                let isLastTab = $sectionNav.find('> ul.nav > li.nav-item').length <= 1;

                if (isLastTab)
                    $nextTabButton.hide();

                $nextTabButton.on('click', onNextTabClick);
                $nextQuestionButton.on('click', onNextQuestionClick);

                if ($nextQuestionButton.length > 0)
                    showFirstQuestion();

                $sectionNav.find('> ul.nav button.nav-link')
                    .on('show.bs.tab', onTabShow)
                    .each(function () {
                        const $this = $(this).addClass('disabled')
                            .on('hidden.bs.tab', function () {
                                $(this).parent('li').css('cursor', 'not-allowed');
                            })
                            .on('shown.bs.tab', function () {
                                $(this).parent('li').css('cursor', '');
                            });

                        if (!$this.hasClass('active'))
                            $this.parent('li').css('cursor', 'not-allowed');
                    });

                function goToNextTab() {
                    if (isLastTab)
                        return;

                    const $activeTab = $sectionNav.find('> ul.nav button.nav-link.active');
                    if ($activeTab.length !== 1)
                        return;

                    const $nextLi = $activeTab.parent('li').next('li');
                    if ($nextLi.length !== 1)
                        return;

                    const $nextLiButton = $nextLi.find('button.nav-link');
                    nextTabId = $nextLiButton.attr('aria-controls');
                    $nextLiButton.tab('show');

                    if ($nextLi.next('li').length == 0) {
                        $nextTabButton.hide();
                        isLastTab = true;
                    }
                }

                function goToNextQuestion() {
                    const $activeQuestion = $sectionNav.find('> .tab-content .tab-pane.active .card-question.active');
                    if ($activeQuestion.length !== 1)
                        return;

                    $activeQuestion.removeClass('active').trigger('hidden.question.card');

                    let $nextQuestion = $activeQuestion.next('.card-question');
                    if ($nextQuestion.length === 1) {
                        $nextQuestion.addClass('active').trigger('shown.question.card');
                    } else {
                        goToNextTab();
                        showFirstQuestion();

                        $nextQuestion = $sectionNav.find('> .tab-content .tab-pane.active .card-question.active');
                    }

                    if ($nextQuestion.length > 0) {
                        scrollToElement($nextQuestion);

                        if (isLastTab && $nextQuestion.next('.card-question').length == 0)
                            $nextQuestionButton.hide();
                    }
                }

                function scrollToElement($el) {
                    const headerHeight = $('header.navbar:first').outerHeight();
                    let scrollTo = $el.offset().top - headerHeight;

                    if (scrollTo < 0)
                        scrollTo = 0;

                    $('html, body').animate({ scrollTop: scrollTo }, 250);
                }

                function showFirstQuestion() {
                    $sectionNav.find('> .tab-content .tab-pane.active .card-question').removeClass('active');
                    $sectionNav.find('> .tab-content .tab-pane.active .card-question:first').addClass('active').trigger('shown.question.card');
                }

                function onNextTabClick(e) {
                    e.preventDefault();

                    goToNextTab();

                    $(window).trigger('attempts:init');
                }

                function onNextQuestionClick(e) {
                    e.preventDefault();

                    goToNextQuestion();

                    $(window).trigger('attempts:init');
                }

                function onTabShow(e) {
                    if (nextTabId && this.getAttribute('aria-controls') === nextTabId) {
                        nextTabId = null;
                        scrollToElement($sectionNav);
                    } else {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                }
            });

            $(function () {
                $(window).trigger('attempts:init');
            });

            $(function () {
                $('.card-body > .form-group.composed-voice-input').each(function () {
                    const $group = $(this);
                    const $recorder = $group.find('> .insite-input-audio');
                    const recorderObj = $recorder.prop('inputAudio');
                    const $player = $group.find('> .insite-output-audio');
                    const playerObj = $player.prop('outputAudio');

                    $recorder.on(inSite.common.inputAudio.event.stopped, function () {
                        const audioBlob = recorderObj.mediaData;
                        if (audioBlob && audioBlob.size > 0)
                            showPlayer(audioBlob);
                        else
                            showRecorder();
                    });

                    $player.on(inSite.common.outputAudio.event.dataError, function () {
                        showRecorder();
                    }).on(inSite.common.outputAudio.event.delete, function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        if (confirm('Are you sure you want to re-record your answer?'))
                            showRecorder();
                    });

                    showRecorder();

                    function showPlayer(blob) {
                        playerObj.show();
                        recorderObj.hide();

                        playerObj.attemptLimit = recorderObj.attemptLimit;
                        playerObj.attemptNow = recorderObj.attemptNow;
                        playerObj.setData(blob);
                        playerObj.loadData();

                        recorderObj.clear();
                    }

                    function showRecorder() {
                        playerObj.setData(null);
                        playerObj.hide();
                        recorderObj.clear();
                        recorderObj.show();
                    }
                });

                $(window).on('attempts:init', function () {
                    $('.card-body > .form-group.composed-voice-input').each(function () {
                        const $group = $(this);
                        $group.find('> .insite-input-audio').prop('inputAudio').stop();
                        $group.find('> .insite-output-audio').prop('outputAudio').stop();
                    });
                });
            });

            $(function () {
                $('.card-body > .form-group.ordering-list > .ordering-list-container').each(function () {
                    $(this).sortable({
                        items: '> div',
                        cursor: 'grabbing',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        axis: 'y',
                        forceHelperSize: true,
                        start: function (s, e) {
                            e.placeholder.height(e.item.height());
                        }
                    });
                });
            });

        </script>
    </insite:PageFooterContent>
</asp:Content>

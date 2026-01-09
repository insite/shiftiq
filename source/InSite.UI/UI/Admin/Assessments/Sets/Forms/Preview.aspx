<%@ Page Language="C#" CodeBehind="Preview.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.Preview" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="QuestionPreviewPanel" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionPreviewPanel.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div runat="server" id="PreviewContainer" class="mb-3" visible="false">
        <h2 runat="server" id="SetName"></h2>
        <div runat="server" id="IntroductionHtml" class="mb-4"></div>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Repeater runat="server" ID="QuestionRepeater" Visible="false">
                    <ItemTemplate>
                        <uc:QuestionPreviewPanel runat="server" ID="PreviewPanel" />
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </div>

    <insite:PageHeadContent runat="server">
        <insite:ResourceLink runat="server" Type="Css" Url="/UI/Admin/assessments/questions/content/styles/preview.css" />
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
                var imgUrlPattern = new RegExp('<%= InSite.UI.Portal.Assessments.Attempts.Utilities.AttemptImageInfo.ImageUrlPattern %>');
                var images = <%= Shift.Common.JsonHelper.SerializeJsObject(Images) %>;

                $('.card-question > .card-header > .question-text img').each(function () {
                    var $this = $(this);
                    var imgSrc = $this.prop('src').toLowerCase();
                    var imgKey = imgSrc;

                    var imgUrlMatches = imgUrlPattern.exec(imgKey);
                    if (imgUrlMatches != null)
                        imgKey = imgUrlMatches[3] + imgUrlMatches[5];

                    var $anchor = $('<a>')
                        .addClass('img-zoom')
                        .attr('href', imgSrc);

                    if (images.hasOwnProperty(imgKey))
                        $anchor.attr('data-width', images[imgKey].width);

                    var caption = $this.prop('title');
                    if (!caption)
                        caption = $this.prop('alt');
                    if (caption)
                        $anchor.attr('data-caption', caption);

                    $this.wrap($anchor)
                        .tooltip({
                            title: caption,
                            placement: 'right',
                            delay: {
                                show: 500,
                                hide: 100
                            }
                        });
                });

                $('.card-question > .card-header > .question-text .img-zoom').fancybox({});

                $('.card-question > .card-body table.table-option > tbody > tr > td.text')
                    .each(function () {
                        var $cell = $(this);
                        var $row = $(this).closest('tr');

                        var $input = $row.find('> td.input input[type="radio"]');
                        if ($input.length !== 1)
                            $input = $row.find('> td.input input[type="checkbox"]');

                        if ($input.length !== 1)
                            return;

                        $cell.data('$input', $input);
                    }).on('click', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        var $input = $(this).data('$input');
                        if ($input instanceof jQuery && $input.length === 1)
                            $input.click();
                    });
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

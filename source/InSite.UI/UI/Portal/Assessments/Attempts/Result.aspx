<%@ Page Language="C#" CodeBehind="Result.aspx.cs" Inherits="InSite.Portal.Assessments.Attempts.Result" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="QuestionOutput" Src="~/UI/Portal/Assessments/Attempts/Controls/ResultQuestionOutput.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:PageHeadContent runat="server">
    <insite:ResourceBundle runat="server" Type="Css">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.css" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/result.css" />
        </Items>
    </insite:ResourceBundle>
</insite:PageHeadContent>

<insite:Container runat="server" ID="KioskMode" Visible="false">
    <script type="text/javascript">
        (function () {
            $('header.navbar').remove();

            $(document).ready(function () {
                $('#attempt-commands').find('> div').addClass('position-fixed');
                $('footer.footer').remove();
                $('body').css('margin-bottom', '0px');
                $('.breadcrumb').closest('section.container').remove();
            });
        })();
    </script>
</insite:Container>

<section class="container mb-2 mb-sm-0 pb-sm-5">

    <p class="text-danger m-0">
        <asp:Literal ID="PageLearner" runat="server"></asp:Literal>
    </p>
    <h2 runat="server" id="PageTitle" class="pt-4 mb-4"></h2>

    <insite:Alert runat="server" ID="AlertMessage" />

    <asp:Literal runat="server" ID="Notification" />

    <insite:Container runat="server" ID="MainContainer">

        <div runat="server" id="DownloadPanel" class="text-end pb-3" visible='<%# Eval("AllowDownloadAssessmentsQA") %>'>
            <insite:DownloadButton runat="server" ID="DownloadPDFButton" Text="Download PDF" />
        </div>

        <asp:Panel runat="server" ID="ScorePanel" style="page-break-inside:avoid;">

            <h3 runat="server" id="ScoreHeader">
                <%= Translate("Your score is") %> <asp:Literal runat="server" ID="ScoreScaled" />
                <span runat="server" id="AttemptPoints" style="font-size:0.875rem;" class="float-end mt-1" ></span>
            </h3>

            <h3 runat="server" id="SubmissionCompletionHeader">
                <%= Translate("Your assessment submission is complete. You will receive your results after the assessment is marked.") %>
            </h3>

            <asp:Literal runat="server" ID="CompletionInstruction" />

            <p runat="server" id="AttemptStatus" visible="false"></p>
        </asp:Panel>

        <div class="mt-4 mb-4">
            <asp:Repeater runat="server" ID="CommandRepeater">
                <ItemTemplate>
                    <insite:Button runat="server" NavigateUrl='<%# Eval("Url") %>'
                        ButtonStyle='<%# Eval("Style") %>' Icon='<%# Eval("Icon") %>' Text='<%# Eval("Text") %>' CssClass="me-3" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <insite:Container runat="server" ID="AnswersContainer">
            <h3 class="mb-4"><%= Translate("Your Answers") %></h3>

            <asp:Repeater runat="server" ID="QuestionRepeater">
                <ItemTemplate>
                    <div class="card shadow mb-4 card-question bg-secondary" style="page-break-inside:avoid;">

                        <div class="card-header border-bottom-0">
                            <div class="question-tags"><%# GetTagsAndLabels() %></div>
                            <h3><%# Translate("Question") %> <%# Eval("AttemptQuestion.QuestionNumber") %></h3>
                            <div class="question-text"><%# Shift.Common.Markdown.ToHtml((string)Eval("AttemptQuestion.QuestionText")) %></div>
                        </div>

                        <div class="card-body bg-white">
                            <uc:QuestionOutput runat="server" ID="QuestionOutput" />

                            <div runat="server" ID="AssessorCommentPanel" class="mt-4" visible="false">
                                <h3>Assessor Comment</h3>

                                <div class="form-group mb-3">
                                    <div>
                                        <asp:Literal runat="server" ID="AssessorCommentOutput" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="FooterPanel" class="card-footer bg-custom-default border-top-0">
                        </div>
                    </div>

                </ItemTemplate>
            </asp:Repeater>

            <div class="mt-4">
                <asp:Repeater runat="server" ID="CommandRepeaterBottom">
                    <ItemTemplate>
                        <insite:Button runat="server" NavigateUrl='<%# Eval("Url") %>'
                            ButtonStyle='<%# Eval("Style") %>' Icon='<%# Eval("Icon") %>' Text='<%# Eval("Text") %>' CssClass="button_margin_right" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </insite:Container>
    </insite:Container>

    <asp:HiddenField runat="server" ID="PdfBody" ViewStateMode="Disabled" />

</section>

<insite:PageFooterContent runat="server">
    <insite:ResourceBundle runat="server" Type="JavaScript">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.likert.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.hotspot.js" />
        </Items>
    </insite:ResourceBundle>

    <script type="text/javascript">

        $(function () {
            $('[data-action]:submit').click(function (e) {
                e.preventDefault();

                const actionValue = $(this).attr('data-action');
                const form = $('form[action="' + String(window.location.pathname) + String(window.location.search) + '"]');
                const actionInput = form.find('#action');

                if (actionInput.length > 0) {
                    actionInput.val(actionValue);
                } else {
                    form.append('<input type="hidden" id="action" name="action" value="' + actionValue + '" />');
                }
                form.submit();
            });

            $('.card-question > .card-body > .composed-essay-input a').each(function () {
                const $anchor = $(this);

                const href = $anchor.attr('href');
                if (href && !$anchor.attr('target'))
                    $anchor.attr('target', '_blank');
            });

            $(window).trigger('attempts:init');
        });

        $(function () {
            const data = [];

            $('.card-question > .card-body > .hotspot-image').each(function () {
                const $container = $(this);

                const item = {};
                item.image = window.attempts.hotspot.createImage($container.data('img'));
                item.pins = window.attempts.hotspot.createPins(item.image, $container.data('pins'));
                item.shapes = window.attempts.hotspot.createShapes($container.data('shapes'));
                item.konva = {
                    container: null,
                    stage: null,
                    layer: null
                };
                item.stageResize = function () { onStageScaled(item.konva.layer); };

                this.append(item.konva.container = document.createElement('div'))
                this.getHotspotData = function () {
                    return item;
                };

                const stage = item.konva.stage = new Konva.Stage({
                    container: item.konva.container,
                    visible: false
                });
                stage.add(item.konva.layer = new Konva.Layer());
                stage.on('scaled', () => onStageScaled(item.konva.layer));

                item.image.initKonva(item.konva.layer);

                if (item.shapes != null) {
                    for (let i = 0; i < item.shapes.length; i++)
                        item.shapes[i].initKonva(item.konva.layer);
                }

                if (item.pins.length > 0) {
                    for (let i = 0; i < item.pins.length; i++)
                        item.konva.layer.add(item.pins[i].obj);
                }

                data.push(item);

                onStageScaled(item.konva.layer);
            });

            $(window).on('resize', function () {
                for (let i = 0; i < data.length; i++)
                    data[i].image.updateSize();
            });

            if (document.fonts)
                document.fonts.ready.then(function () {
                    for (let i = 0; i < data.length; i++) {
                        const stage = data[i].konva.stage;
                        if (stage)
                            stage.draw();
                    }
                });

            function onStageScaled(layer) {
                const layerScale = layer.getAbsoluteScale().x;
                const objScale = {
                    x: 1 / layerScale,
                    y: 1 / layerScale
                };

                const pins = layer.find('.pin');
                for (let i = 0; i < pins.length; i++) {
                    const pin = pins[i];
                    for (let j = 0; j < pin.children.length; j++) {
                        pin.children[j].scale(objScale);
                    }
                }
            }
        });

    </script>
</insite:PageFooterContent>

<insite:PageFooterContent runat="server" ID="DownloadPdfScript">
    <script type="text/javascript">
        (() => {
            const btnDownload = document.getElementById('<%= DownloadPDFButton.ClientID %>');
            const inputData = document.getElementById('<%= PdfBody.ClientID %>');
            if (!btnDownload)
                return;

            const maxHotspotWidth = 1000;

            btnDownload.addEventListener('click', onDownload);

            function onDownload(e) {
                const container = document.createElement('div');

                const questions = document.querySelectorAll('.card-question');
                for (let srcQuestion of questions) {
                    const cpyQuestion = srcQuestion.cloneNode(true);
                    cpyQuestion.classList.remove('shadow');
                    cpyQuestion.querySelectorAll('[id]').forEach(el => el.removeAttribute('id'));
                    cpyQuestion.querySelectorAll('input[type="hidden"]').forEach(el => el.remove());

                    const srcHotspot = srcQuestion.querySelector('.hotspot-image');
                    if (srcHotspot) { 
                        const cpyHotspot = cpyQuestion.querySelector('.hotspot-image');
                        cpyHotspot.replaceChildren();

                        const data = srcHotspot.getHotspotData();
                        const origSize = data.konva.stage.size();
                        const origScale = data.konva.stage.scale();
                        const tempSize = { width: data.image.width, height: data.image.height };
                        const tempScale = { x: 1, y: 1 };

                        if (tempSize.width > maxHotspotWidth) {
                            const scale = maxHotspotWidth / tempSize.width;

                            tempSize.width *= scale;
                            tempSize.height *= scale;
                            tempScale.x *= scale;
                            tempScale.y *= scale;
                        }

                        data.konva.stage.size(tempSize);
                        data.konva.stage.scale(tempScale);
                        data.stageResize();

                        const img = document.createElement('img'); {
                            img.alt = '';
                            img.src = data.konva.stage.toDataURL({ pixelRatio: 1.5 });
                            cpyHotspot.append(img);
                        }

                        data.konva.stage.size(origSize);
                        data.konva.stage.scale(origScale);
                        data.stageResize();
                    }

                    container.append(cpyQuestion);
                }

                inputData.value = container.innerHTML;
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>

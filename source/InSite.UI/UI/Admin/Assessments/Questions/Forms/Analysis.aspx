<%@ Page Language="C#" CodeBehind="Analysis.aspx.cs" Inherits="InSite.Admin.Assessments.Questions.Forms.Analysis" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ChangeRepeater" Src="../../../Reports/Changes/Controls/ChangeRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AnalysisStatus" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="QuestionSection" Title="Question" Icon="far fa-question" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Question
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Nav runat="server" ID="AnalysisNav">

                        </insite:Nav>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CommentsSection" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Comments
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Nav runat="server" ID="CommentsNav">

                        </insite:Nav>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="HistorySection" Title="History" Icon="far fa-history" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    History
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Nav runat="server" ID="HistoryNav">
                            <insite:NavItem runat="server" Title="Events">
                                <uc:ChangeRepeater runat="server" ID="ChangeRepeater" />
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="SnapshotTab" Title="Snapshots">
                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SnapshotUpdatePanel" />
                                <insite:UpdatePanel runat="server" ID="SnapshotUpdatePanel">
                                    <ContentTemplate>
                                        <div class="d-none">
                                            <uc:QuestionRepeater runat="server" ID="SnapshotRepeater" ViewStateMode="Disabled" />
                                            <asp:Button runat="server" ID="SnapshotLoad" />
                                            <asp:HiddenField runat="server" ID="SnapshotState" />
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6">
                                                <insite:ComboBox runat="server" ID="SnapshotLeftSelector" MaxHeight="300px" />
                                                <table class="table question-grid"><tbody></tbody></table>
                                            </div>
                                            <div class="col-lg-6">
                                                <insite:ComboBox runat="server" ID="SnapshotRightSelector" MaxHeight="300px" />
                                                <table class="table question-grid"><tbody></tbody></table>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </insite:NavItem>
                        </insite:Nav>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="row">
        <div class="col-lg-12">
            <insite:CloseButton runat="server" ID="CloseButton" />
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (typeof URLSearchParams !== 'undefined') {
                    var location = window.location;
                    var queryParams = new URLSearchParams(location.search);
                    var scroll = queryParams.get('scroll');

                    if (scroll) {
                        try {
                            var scrollTop = inSite.common.base64.toInt(scroll);

                            $(document).ready(function () {
                                $(window).scrollTop(scrollTop);
                            });

                            queryParams.delete('scroll');

                            var path = location.pathname + '?' + queryParams.toString();
                            var url = location.origin + path;

                            window.history.replaceState({}, '', url);

                            $('form#aspnetForm').attr('action', path);
                        } catch (e) {

                        }
                    }
                }

                $('a.scroll-send').on('click', function () {
                    var scroll = $(window).scrollTop();

                    if (scroll > 0)
                        scroll = inSite.common.base64.fromInt(Math.floor(scroll));
                    else
                        scroll = null;

                    var url = $(this).attr('href');

                    $(this).attr('href', inSite.common.updateQueryString('scroll', scroll, url));
                });
            })();
        </script>

        <script type="text/javascript">
            (function () {
                var instance = window.questionAnalysis = window.questionAnalysis || {};

                $('button[data-bs-target="#<%= SnapshotTab.ClientID %>"][data-bs-toggle="tab"]').one('shown.bs.tab', function () {
                    $('#<%= SnapshotLoad.ClientID %>').click();
                });

                instance.afterSnapshotInit = function (inputData) {
                    var $stateInput = $('#<%= SnapshotState.ClientID %>');
                    if ($stateInput.val())
                        return;

                    var outpuData = [];

                    var $tab = $('#<%= SnapshotTab.ClientID %>');
                    $tab.find('div.d-none table.question-grid').find('> tbody > tr[data-question]').each(function () {
                        var $tr = $(this);
                        var id = $tr.data('question').toLowerCase();

                        var item = null;
                        for (var i = 0; i < inputData.length; i++) {
                            var dataItem = inputData[i];
                            if (dataItem.id == id) {
                                item = dataItem;
                                inputData.splice(i, 1);
                                break;
                            }
                        }

                        if (item != null) {
                            item.value = $tr.find('*').removeAttr('id').removeAttr('name').end().html().trim();
                            if (outpuData.length == 0 || outpuData[outpuData.length - 1].value != item.value)
                                outpuData.push(item);
                        }
                    }).end().remove();

                    $('#<%= SnapshotState.ClientID %>').val(JSON.stringify(outpuData));
                };

                Sys.Application.add_load(function () {
                    var $leftSelector = $('#<%= SnapshotLeftSelector.ClientID %>').on('change', onSelected);
                    var $rightSelector = $('#<%= SnapshotRightSelector.ClientID %>').on('change', onSelected);

                    var $stateInput = $('#<%= SnapshotState.ClientID %>');
                    if ($stateInput.data('inited') === true)
                        return;

                    var dataJson = $stateInput.val();
                    if (!dataJson)
                        return;

                    var data = JSON.parse(dataJson);
                    if (data.length == 0)
                        return;

                    $leftSelector.empty();
                    $rightSelector.empty();

                    addItem('', '');

                    for (var i = 0; i < data.length; i++) {
                        var dataItem = data[i];
                        addItem(dataItem.date + ' by ' + dataItem.user, dataItem.id);
                    }

                    $leftSelector.selectpicker('refresh');
                    $rightSelector.selectpicker('refresh');

                    $stateInput.data('inited', true);

                    var $lastLeftOption = $leftSelector.find('option').last();
                    if ($lastLeftOption.length == 1)
                        $leftSelector.selectpicker('val', $lastLeftOption.prop('value'));

                    onSelected.call($leftSelector[0]);

                    function addItem(text, value) {
                        $leftSelector.append($('<option>').text(text).attr('value', value));
                        $rightSelector.append($('<option>').text(text).attr('value', value));
                    }
                });

                function onSelected() {
                    var $this = $(this);
                    var $tbody = $this.parent().parent().find('table.question-grid > tbody').empty();

                    var value = $this.selectpicker('val');
                    if (!value)
                        return;

                    var $stateInput = $('#<%= SnapshotState.ClientID %>');
                    if ($stateInput.data('inited') !== true)
                        return;

                    var dataJson = $stateInput.val();
                    if (!dataJson)
                        return;

                    var data = JSON.parse(dataJson);
                    if (data.length == 0)
                        return;

                    for (var i = 0; i < data.length; i++) {
                        var dataItem = data[i];
                        if (dataItem.id == value) {
                            $tbody.append($('<tr>').append(dataItem.value));
                            break;
                        }
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

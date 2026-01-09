<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeRepeater.ascx.cs" Inherits="InSite.Admin.Reports.Changes.Controls.ChangeRepeater" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table id="<%# Repeater.ClientID %>" class="table table-striped"><thead>
            <tr>
                <insite:Container runat="server" Visible='<%# ShowAssetNumber %>'>
                    <th>Asset</th>
                </insite:Container>
                <th>User</th>
                <th>Event Time</th>
                <th>Event Name</th>
                <th style="width:40px;"></th>
            </tr>
        </thead><tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr data-id='<%# Eval("Version") %>'>
            <insite:Container runat="server" Visible='<%# ShowAssetNumber %>'>
                <td><%# Eval("AssetNumber") %></td>
            </insite:Container>
            <td><%# Eval("User") %></td>
            <td><%# Eval("Time") %></td>
            <td><%# Eval("Name") %></td>
            <td style="text-align:right;">
                <a href="#data" data-action="data"><i class="far fa-search"></i></a>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<insite:Modal runat="server" ID="DataViewerWindow" Title="Event Data" Width="1000px">
    <ContentTemplate>
        <div class="data-output" style="min-height:80px;"></div>

        <div class="text-end" style="margin-top:15px;">
            <insite:CloseButton runat="server" OnClientClick="modalManager.close(this); return false;" />
        </div>

        <insite:LoadingPanel runat="server" />
    </ContentTemplate>
</insite:Modal>

<insite:PageHeadContent runat="server" ID="StyleLiteral" RenderRequired="true">
    <style type="text/css">
        .table-properties {
            word-break: normal;
        }

            .table-properties > tbody > tr > td {
                vertical-align: top;
            }

                .table-properties > tbody > tr > td.name {
                    padding-right: 10px;
                }

                .table-properties > tbody > tr > td.value {
                    word-break: break-word;
                }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="ScriptLiteral" RenderRequired="true">
    <script type="text/javascript">
        (function () {
            var instance = window.changeRepeater = window.changeRepeater || {};

            var dateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(\+|-)\d{2}:\d{2}$/;

            instance.init = function (aggregateId, tableId, windowId) {
                var $modal = $('#' + String(windowId))
                    .on('hide.bs.modal', onModalHide);
                var $modalBody = $modal.find('> .modal-dialog > .modal-content > .modal-body');
                var $output = $modalBody.find('div.data-output');
                var $loadingPanel = $modalBody.find('.loading-panel');

                $('#' + String(tableId) + ' > tbody > tr[data-id]').each(function () {
                    var $row = $(this);
                    var id = parseInt($row.data('id'));

                    $row.find('[data-action]').on('click', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        var $this = $(this);
                        var action = $this.data('action');

                        if (action === 'data') {
                            if (loadData(id))
                                modalManager.show($modal);
                        }
                    });
                });

                function onModalHide() {
                    $output.empty();
                }

                function loadData(version) {
                    if ($.active > 0)
                        return false;

                    $loadingPanel.show();

                    var data = {
                        aggregate: aggregateId,
                        version: version,
                    };

                    $.ajax({
                        type: 'GET',
                        dataType: 'json',
                        url: '/api/changes/get-data',
                        data: data,
                        headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                        success: function (result) {
                            $output.empty();

                            var $table = objectToTable(result);
                            if ($table)
                                $output.append($table);
                            else
                                $output.html('<h3 style="margin:30px 0;" class="text-center">No Data</h3>')
                        },
                        error: function (xhr) {
                            alert('Error: ' + xhr.status);
                        },
                        complete: function () {
                            $loadingPanel.hide();
                        },
                    });

                    return true;
                }
            };

            function objectToTable(data) {
                var propList = [];

                appendProps(data, propList, 0);

                if (propList.length == 0)
                    return null;

                var $body = $('<tbody>');

                for (var i = 0; i < propList.length; i++) {
                    var prop = propList[i];
                    $body.append(
                        $('<tr>')
                            .append(
                                $('<td class="name">')
                                    .append($('<strong>').css('margin-left', String(prop.depth * 20) + 'px').text(prop.name))
                                    .append(document.createTextNode(':')),
                                $('<td class="value">')
                                    .text(prop.value)
                            )
                    );
                }

                return $('<table class="table table-hover table-properties">').append($body);

                function appendProps(data, result, depth) {
                    for (var p in data) {
                        if (!data.hasOwnProperty(p))
                            continue;

                        var item = {
                            name: p,
                            depth: depth,
                            value: ''
                        };

                        result.push(item);

                        var value = data[p];
                        var valueType = typeof value;

                        if (valueType === 'string') {
                            if (dateRegex.test(value)) {
                                item.value = moment(value).format('MMM D, YYYY h:mm:ss A');
                            } else {
                                item.value = value;
                            }
                        } else if (valueType === 'object') {
                            appendProps(value, result, depth + 1);
                        } else {
                            item.value = String(value);
                        }
                    }
                }
            }
        })();
    </script>
</insite:PageFooterContent>
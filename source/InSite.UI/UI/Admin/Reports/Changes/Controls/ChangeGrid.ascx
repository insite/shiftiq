<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeGrid.ascx.cs" Inherits="InSite.Admin.Reports.Changes.Controls.ChangeGrid" %>


<div class="row">
    <div class="col-md-6">
        <div class="d-flex align-items-center mb-2">
            <insite:TextBox runat="server" ID="FilterTextBox" Width="30em" EmptyMessage="Filter" />
            <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
        </div>
    </div>
    <div class="col-md-6 text-end">
        <insite:DownloadButton runat="server" ID="DownloadButton" ButtonStyle="Primary" />
    </div>
</div>
<insite:PageFooterContent runat="server">
            <script type="text/javascript"> 
                (function () {
                    Sys.Application.add_load(function () {
                        $('#<%= FilterTextBox.ClientID %>')
                            .off('keydown', onKeyDown)
                            .on('keydown', onKeyDown);
                    });

                    function onKeyDown(e) {
                        if (e.which === 13) {
                            e.preventDefault();
                            $('#<%= FilterButton.ClientID %>')[0].click();
                        }
                    }
                })();
            </script>
        </insite:PageFooterContent>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Version" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40">
            <ItemTemplate>
                <%# (Guid)Eval("AggregateIdentifier") == AggregateID ? Eval("Version") : null %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# Eval("Time") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Change Type">
            <ItemTemplate>
                <%# Eval("Name") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <%# Eval("User") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Data" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40">
            <ItemTemplate>
                <a href="#data" data-action="data" data-id='<%# Eval("AggregateIdentifier") %>' data-version='<%# Eval("Version") %>'>
                    <i class="far fa-search"></i>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:Modal runat="server" ID="DataViewerWindow" Title="Event Data" Width="1000px">
    <ContentTemplate>
        <div class="data-output" style="min-height: 80px;"></div>

        <div class="text-end" style="margin-top: 15px;">
            <insite:CloseButton runat="server" OnClientClick="modalManager.close(this); return false;" />
        </div>

        <insite:LoadingPanel runat="server" />
    </ContentTemplate>
</insite:Modal>

<insite:PageHeadContent runat="server" ID="StyleLiteral">
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

<insite:PageFooterContent runat="server" ID="ScriptLiteral">
    <script type="text/javascript">
        (function () {
            var instance = window.changeGrid = window.changeGrid || {};

            var dateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(\+|-)\d{2}:\d{2}$/;

            instance.init = function () {
                var $modal = $('#<%= DataViewerWindow.ClientID %>')
                    .on('hide.bs.modal', onModalHide);
                var $modalBody = $modal.find('> .modal-dialog > .modal-content > .modal-body');
                var $output = $modalBody.find('div.data-output');
                var $loadingPanel = $modalBody.find('.loading-panel');

                $('#<%= Grid.ClientID %> a[data-action]').on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var $this = $(this);
                    var action = $this.data('action');

                    if (action === 'data') {
                        var id = $this.data('id');
                        var version = $this.data('version');

                        if (loadData(id, version)) {
                            modalManager.show($modal);
                        }
                    }
                });

                function onModalHide() {
                    $output.empty();
                }

                function loadData(aggregateId, version) {
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

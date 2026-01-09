<%@ Page Language="C#" CodeBehind="Info.aspx.cs" Inherits="InSite.Admin.Assets.Uploads.Forms.Info" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style type="text/css">
        .commands {
        }

            .commands > .sub-commands {
                display: none;
                position: absolute;
                right: 0;
            }

            .commands .btn[disabled].selected {
                background-image: none;
                outline: 0;
                -webkit-box-shadow: inset 0 3px 5px rgba(0, 0, 0, .125);
                box-shadow: inset 0 3px 5px rgba(0, 0, 0, .125);
                background-color: #e6e6e6;
                border-color: #adadad;
                opacity: 1;
            }

        ul[data-reorder] > li {
            padding-left: 40px;
            cursor: grab;
        }

            ul[data-reorder] > li > .fa,
            ul[data-reorder] > li > .fas,
            ul[data-reorder] > li > .far,
            ul[data-reorder] > li > .fab,
            ul[data-reorder] > li > .fal {
                position: absolute;
                left: 15px;
                margin-top: 6px;
                opacity: 0.75;
            }

    </style>

    <div class="mb-3 d-flex commands">
        <button type="button" class="btn btn-sm btn-icon btn-primary me-1" title="Download" data-action="download">
            <i class='fa fa-download'></i>
        </button>
        <button type="button" class="btn btn-sm btn-icon btn-default me-1" title="Edit" data-action="edit">
            <i class='fas fa-pencil'></i>
        </button>

        <span class="sub-commands edit">
            <button type="button" class="btn btn-sm btn-icon btn-success me-1" title="Save" data-action="edit-save">
                <i class='far fa-cloud-upload'></i>
            </button>
            <button type="button" class="btn btn-sm btn-icon btn-default me-1" title="Cancel" data-action="edit-cancel">
                <i class='fa fa-ban'></i>
            </button>
        </span>

        <button type="button" class="btn btn-sm btn-icon btn-default me-1" title="Delete" data-action="delete">
            <i class='fa fa-trash-alt'></i>
        </button>
    </div>

    <div class="mt-3 ps-3">

        <div class="form-group mb-3">
            <label class="form-label">File</label>
            <div>
                <div class="readonly-value" id="fileNameField"><asp:Literal runat="server" ID="FileName" /></div>
                <insite:TextBox runat="server" ID="FileNameEdit" CssClass="form-control" MaxLength="128" required style="display: none;" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Folder</label>
            <div>
                <div class="readonly-value" id="folderField">
                    <asp:Literal runat="server" ID="Folder" /></div>
                <insite:TextBox runat="server" ID="FolderEdit" CssClass="form-control" MaxLength="128" required style="display: none;" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Size</label>
            <div>
                <div class="readonly-value"><asp:Literal runat="server" ID="FileSize" /></div>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">URL</label>
            <div>
                <div class="readonly-value"><asp:HyperLink runat="server" ID="DownloadURL" Target="_blank" /></div>
            </div>
        </div>

        <asp:Literal runat="server" ID="PostedBy" />
    </div>

    <insite:LoadingPanel runat="server" />

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/js/modal-manager.js" />

    <script type="text/javascript">
        (function () {
            var filePath = '<%= FilePath %>';

        modalManager.setModalTitle('<%= FileTitle %>');

        var $loadingPanel = $('.loading-panel');

        $('.commands button').on('click', onCommandButtonClick);

        // events

        function onCommandButtonClick() {
            var $this = $(this);
            var action = $this.data('action');

            if (action === 'edit') {
                doAction(action, null, initEdit);
            } else if (action === 'delete') {
                if (!confirm('Are you sure you want to delete this file?'))
                    return;

                doAction(action, { path: filePath }, function () {
                    modalManager.closeModal({ action: 'refresh' });
                });
            } else if (action === 'download') {
                doRequest('POST', '/ui/admin/assets/uploads/info?path=' + filePath, { action: action });
            } else if (action === 'edit-cancel') {
                cancelEdit();
            } else if (action === 'edit-save') {
                saveEdit();
            }
        }

        // methods (view)

        function initView(result) {
            if (result.type == 'OK') {
                $('#<%= FileNameEdit.ClientID %>').hide();
                $('#<%= FolderEdit.ClientID %>').hide();
                $('#fileNameField').show();
                $('#folderField').show();
                modalManager.updateModalHeight();
            }
        }

        // methods (edit)

        function initEdit(result) {
            if (result.type == 'OK') {
                showSubCommands('edit');

                $('#fileNameField').hide();
                $('#folderField').hide();
                $('#<%= FileNameEdit.ClientID %>').show();
                $('#<%= FolderEdit.ClientID %>').show();
                modalManager.updateModalHeight();
            }
        }

        function cancelEdit() {
            showSubCommands('edit', false);

            doAction('view', null, initView);
        }

        function saveEdit() {
            $('#<%= FileNameEdit.ClientID %>').val();
            $('#<%= FolderEdit.ClientID %>').val();

            doAction('save-edit', { name: $('#<%= FileNameEdit.ClientID %>').val(), location: $('#<%= FolderEdit.ClientID %>').val() },
                function () {
                    modalManager.closeModal({ action: 'refresh' });
                });
            }

            // methods (request)

            function doAction(action, data, callback) {
                if ($.active > 0)
                    return;

                showLoadingPanel();

                if (data == null || typeof data !== 'object')
                    data = {};

                if (data instanceof Array)
                    data.push({ name: 'action', value: action });
                else
                    $.extend(data, { IsPageAjax: true, action: action });

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: '/ui/admin/assets/uploads/info?path=' + encodeURIComponent(filePath),
                    data: data,
                    success: function (result) {
                        if (result.type == 'SUCCESS' || result.type == 'OK') {
                            showLoadingPanel(false);
                            if (typeof callback == 'function')
                                setTimeout(callback, 0, result);
                        } else if (result.type == 'ERROR') {
                            alert('Error: ' + result.message);
                        } else {
                            alert('Unexpected result type: ' + JSON.stringify(result));
                        }
                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    },
                    complete: function () {
                        showLoadingPanel(false);
                    },
                });
            }

            function doRequest(method, action, data) {
                var $form = $('<form>')
                    .attr({ method: method, action: action })
                    .css({ display: 'none' });

                populateForm($form, data);

                $form
                    .appendTo('body')
                    .submit()
                    .remove();
            }

            // methods (helpers)

            function populateForm($form, data) {
                for (var key in data) {
                    if (data.hasOwnProperty(key))
                        populate(key, data[key]);
                }

                function populate(name, data) {
                    if ($.isArray(data)) {
                        for (var i = 0; i < data.length; i++)
                            populate(name + '[' + String(i) + ']', data[i]);
                    } else if ($.isPlainObject(data)) {
                        for (var key in data) {
                            if (data.hasOwnProperty(key)) {
                                populate(name + '[' + key + ']', data[key]);
                            }
                        }
                    } else if (data != null) {
                        $form.append($('<input>').attr({
                            type: 'hidden',
                            name: String(name),
                            value: String(data)
                        }));
                    }
                }
            }

            function showSubCommands(action, show) {
                var $cmdButtons = $('.commands > button');
                var $subCmd = $('.commands > .sub-commands.' + action);

                show = show !== false;

                $cmdButtons.each(function () {
                    var $this = $(this);

                    var btnAction = $this.data('action');
                    if (btnAction == action) {
                        if (show)
                            $this.prop('disabled', true).addClass('selected').blur();
                        else
                            $this.prop('disabled', false).removeClass('selected');
                    } else {
                        if (show)
                            $this.css('visibility', 'hidden');
                        else
                            $this.css('visibility', '');
                    }
                });

                if (show)
                    $subCmd.css('display', 'inline');
                else
                    $subCmd.hide();
            }

            function showLoadingPanel(show) {
                if (show !== false) {
                    if (window.parent)
                        window.parent.postMessage('insite.fileInfo:disable-close', '*');

                    $loadingPanel.show();
                } else {
                    $loadingPanel.hide();

                    if (window.parent)
                        window.parent.postMessage('insite.fileInfo:enable-close', '*');
                }
            }
        })();
    </script>
</asp:Content>

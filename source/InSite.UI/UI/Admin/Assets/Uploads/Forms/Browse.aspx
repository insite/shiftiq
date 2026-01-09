<%@ Page Language="C#" CodeBehind="Browse.aspx.cs" Inherits="InSite.Admin.Assets.Uploads.Forms.Browse" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <insite:ResourceLink runat="server" Type="Css" Url="/UI/Layout/common/parts/plugins/magnific-popup/magnific-popup.css" />

    <style type="text/css">

        .mfp-bg {
            z-index: 10000;
        }

        .mfp-wrap {
            z-index: 10001;
        }

        .file-browser-text-viewer {
            background-color: #f5f5f5;
            border: 1px solid #cccccc;
            border-radius: 4px;
            margin: 20px auto;
            max-width: 850px;
            min-height: 600px;
            padding: 20px 30px;
            position: relative;
        }

            .file-browser-text-viewer > pre {
                background-color: transparent;
                border: none;
                margin: 0;
                max-height: 545px;
                padding: 0;
            }

            .file-browser-text-viewer > .mfp-close {
                display: none;
            }

        #content.content {
            position: relative;
            top: -45px;
            padding-top: 0;
        }

            #content .loading-indicator {
                margin-top: 17px;
                position: absolute;
                right: 0;
                opacity: 0;
                transition: opacity 0.3s ease-in-out 0.1s;
            }

            #content.file-browser-locked .loading-indicator {
                opacity: 1;
            }

            #content #browse_path {
            }

                #content #browse_path > ul {
                    padding: 0;
                    margin: 0 0 0 2px;
                    list-style: none;
                }

                    #content #browse_path > ul > li {
                        display: inline-block;
                        font-size: 16px;
                        color: #337ab7;
                        margin-right: 7px !important;
                    }

                        #content #browse_path > ul > li::before {
                            content: "/";
                        }

                        #content #browse_path > ul > li:first-child {
                        }

                            #content #browse_path > ul > li:first-child > a,
                            #content #browse_path > ul > li:first-child > span {
                                margin-left: 0px;
                            }

                            #content #browse_path > ul > li:first-child::before {
                                display: none;
                            }

                        #content #browse_path > ul > li > a,
                        #content #browse_path > ul > li > span {
                            margin-left: 6px;
                        }

                        #content #browse_path > ul > li > span {
                            color: #303030;
                        }

        .form-subtitle {
            padding-bottom: 20px;
        }

            #content #browse_folders {
                position: relative;
            }

                #content #browse_folders > a {
                    display: block;
                    padding: 5px 35px 5px 25px;
                }

                    #content #browse_folders > a:hover,
                    #content #browse_folders > a:focus {
                        background-color: #f5f5f5;
                        text-decoration: none;
                    }

                    #content #browse_folders > a + a {
                        border-top: 1px solid #e9e9e9;
                    }

                    #content #browse_folders > a > i.fa,
                    #content #browse_folders > a > i.fas,
                    #content #browse_folders > a > i.far,
                    #content #browse_folders > a > i.fab,
                    #content #browse_folders > a > i.fal {
                        position: absolute;
                        margin: 3px 0 0 -21px;
                    }

                    #content #browse_folders > a > span.name {
                        display: block;
                        margin-right: 5px;
                        overflow: hidden;
                    }

                    #content #browse_folders > a > span.file-count {
                        position: absolute;
                        right: 3px;
                        font-size: 80%;
                        line-height: 1.9;
                    }

        .file-thumbnail > .file-preview {
            text-align: center;
        }

        .file-thumbnail > .file-preview > i.fa,
        .file-thumbnail > .file-preview > i.fas,
        .file-thumbnail > .file-preview > i.far,
        .file-thumbnail > .file-preview > i.fab,
        .file-thumbnail > .file-preview > i.fal {
            font-size: 56px;
            margin: 21px 0 22px 0;
        }

        #FileCommandsPanel > span.separator:before {
            content: '|';
            font-size: 14px;
        }

        .file-thumbnail > div.file-name {
            cursor: pointer;
            overflow: hidden;
        }

        .btn-fileinfo {
            position: absolute;
            right: 4px;
            bottom: 4px;
            opacity: 0;
            visibility: hidden;
            transition: opacity 0.3s;
            margin-right: 5px;
        }

        #folders h2, #browse_files h2 {
            margin-top: 0px;
        }

        #browse_files > .row > .col-md-3:hover .file-thumbnail > .btn-fileinfo {
            opacity: 0.75;
            visibility: visible;
        }
    </style>  
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <div id="content">
        <insite:Alert runat="server" ID="SearchAlert" />
        <insite:Alert runat="server" ID="ScreenStatus" />

        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
          <li class="nav-item">
            <a href="#results" class="nav-link active" data-bs-toggle="tab" role="tab">
              <i class="fas fa-database me-2"></i>
              Results
            </a>
          </li>
          <li class="nav-item">
            <a href="#criteria" class="nav-link" data-bs-toggle="tab" role="tab">
              <i class="fas fa-search me-2"></i>
              Criteria
            </a>
          </li>
            <li>
                <button type="button" class="btn btn-default btn-upload" title="Upload File">
                    <i class="fa fa-upload" style="padding-right: 5px;"></i>Upload File
                </button>
            </li>
        </ul>

        <!-- Tabs content -->
        <div class="tab-content">
            <div class="tab-pane fade show active" id="results" role="tabpanel">
                <div class="row">
                    <div class="col-lg-12">
                        <div id="desktop">        
                            <asp:HiddenField runat="server" ID="CompanyName" />

                            <div class="row">
                                <div id="browse_path" class="col-xs-12">
                                    <div class="text-center">
                                        <asp:Image runat="server" ImageUrl="/Images/Animations/loader.gif" CssClass="loading-indicator" />
                                    </div>
                                    
                                    <p class="error_message no_items">No Data.</p>
                                    <ul>

                                    </ul>
                                </div>
                                <div class="col-xs-12">
                                    <div class="form-subtitle">Test</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div id="folders" class="col-3">
                        <h2>Folders</h2>
                        <div id="browse_folders">
                            <p class="error_message no_items">No Folders</p>
                        </div>
                    </div>

                    <div id="browse_files" class="col-9">
                        <h2>Files</h2>
                        <div class="clearfix"></div>
                        <p class="error_message no_items">No Files.</p>
                    </div>
                </div>
            </div>
            <div class="tab-pane fade" id="criteria" role="tabpanel">
                <div class="row">
                    <div class="col-sm-6 col-xxl-3">
                        <div class="mb-2">
                            <insite:TextBox ID="FileName" runat="server" EmptyMessage="File Name" MaxLength="256" />
                        </div>

                        <div class="mb-2">
                            <insite:DateTimeOffsetSelector ID="FileCreatedSince" runat="server" EmptyMessage="Created Since" />
                        </div>

                        <div class="mb-2">
                            <insite:DateTimeOffsetSelector ID="FileCreatedBefore" runat="server" EmptyMessage="Created Before" />
                        </div>
                    </div>
                    <div class="col-sm-6 col-xxl-9">
                        <div runat="server" id="FileCommandsPanel" ClientIDMode="Static">
                            <label>
                                <insite:FileTypeMultiComboBox runat="server" ID="FileTypeFilter" Width="230px" Multiple-ActionsBox="true" EmptyMessage="File Type" />
                            </label>

                            <button type="button" data-include="imgs" class="btn btn-default btn-quickfilter">Images</button>
                            <button type="button" data-include="not-imgs" class="btn btn-default btn-quickfilter">Documents</button>
                        </div><br />
                    </div>
                </div>

                <div class="mt-2">
                    <insite:FilterButton runat="server" ID="SearchButton" Width="97px" />
                    <insite:ClearButton runat="server" ID="ClearButton" Width="97px" />
                </div>
            </div>
        </div>

        <insite:Modal runat="server" ID="FileInfoWindow" />

        <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/magnific-popup/jquery.magnific-popup.min.js" />
    </div>
    <script type="text/javascript">

        Sys.Application.add_load(function () {
            var autoUnlockHandler = null;
            var currentPath = '/';
            var previewItems = null;
            var allowCloseInfoWindow = true;

            $('.btn-upload').on('click', onUploadClick);
            var $fileInput = null;

            var $fileTypeFilter = $('#<%= FileTypeFilter.ClientID %>');
            var fileNameInput = document.getElementById('<%= FileName.ClientID %>');
            var fileCreatedSince = document.getElementById('<%= FileCreatedSince.ClientID %>');
            var fileCreatedBefore = document.getElementById('<%= FileCreatedBefore.ClientID %>');

            var quickFilterSettings = null;

            var $quickFilterButtons = $('#FileCommandsPanel button.btn-quickfilter').on('click', function () {
                var $this = $(this);

                setupQuickFilter($this)

                $this.blur();
            });

            $('#<%= SearchButton.ClientID %>').on('click', function () {
                initFilter();
                loadPath(currentPath);

                return false;
            });

            $('#<%= ClearButton.ClientID %>').on('click', function () {
                setupFilterDefaults();
                initFilter();

                return false;
            });

            $(document).ready(function () {
                var folder = inSite.common.queryString['folder'];
                if (!!folder && folder.length > 0)
                    currentPath = folder;

                setupFilterDefaults();
                $('#<%= SearchButton.ClientID %>').click();
            });

            $(window).on('message', function (e) {
                var eventData = String(e.originalEvent.data);
                if (!eventData.startsWith('insite.fileInfo:'))
                    return;

                var command = eventData.substring(15);

                if (command === 'disable-close')
                    allowCloseInfoWindow = false;
                else if (command === 'enable-close')
                    allowCloseInfoWindow = true;
            });

            function initFilter(data) {
                if (data instanceof Array) {
                    var fileTypes = {};

                    $fileTypeFilter.find('option').each(function () {
                        var $option = $(this);
                        var ext = $option.text().trim();

                        if (typeof ext !== 'undefined') {
                            if (!fileTypes.hasOwnProperty(ext))
                                fileTypes[ext] = $option.prop('selected');
                        }
                    });
                }

                quickFilterSettings = getQuickFilterSettings();

                setupQuickFilter();
            }

            // methods

            function loadTypes(completed) {
                if (!lock())
                    return;

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/files/typelist',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    complete: function () {
                        unlock();
                    },
                    success: function (data) {
                        unlock();

                        if (data == null || typeof data.type != 'string') {
                            alert('Unexpected response:\r\n' + String(data));
                        } else if (data.type === 'ERROR') {
                            alert(String(data.message));
                        } else if (data.type === 'TypeList') {
                            initFilter(data.types);

                            if (typeof completed === 'function')
                                completed();
                        } else {
                            alert('Unexpected response type:\r\n' + String(data.type));
                        }
                    },
                    error: function () {
                        alert('An error occurred during request');
                    },
                });
            };

            function loadPath(path) {
                if (!lock())
                    return;

                var fileTypeFilter = '';

                $fileTypeFilter.find('option').each(function () {
                    var $option = $(this);
                    if ($option.prop('selected')) {
                        var ext = $option.text().trim();

                        if (typeof ext !== 'undefined') {
                            fileTypeFilter += ',' + String(ext);
                        }
                    }
                });

                if (fileTypeFilter.length > 0)
                    fileTypeFilter = fileTypeFilter.substring(1);

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/files/list',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: {
                        path: path,
                        type: fileTypeFilter,
                        name: fileNameInput.value,
                        since: fileCreatedSince.getDate()?.format(),
                        before: fileCreatedBefore.getDate()?.format(),
                    },
                    complete: function () {
                        unlock();
                    },
                    success: function (data) {
                        unlock();

                        if (data == null || typeof data.type != 'string') {
                            alert('Unexpected response:\r\n' + String(data));
                        } else if (data.type === 'ERROR') {
                            alert(String(data.message));
                        } else if (data.type === 'FileList') {
                            if (setCurrentPath(data.path))
                                currentPath = data.path;

                            setFolders(data.folders);
                            setFiles(data.files, true);
                            setTooltips();

                            document.querySelector('a[href="#results"]').click();
                        } else {
                            alert('Unexpected response type:\r\n' + String(data.type));
                        }
                    },
                    error: function () {
                        alert('An error occurred during request');
                    },
                });
            };

            function setCurrentPath(data) {
                var isLoaded = false;

                var container = $('#browse_path > ul').first();
                container.find('> li').remove();

                if (typeof data == 'string' && data.length > 0) {
                    var path = data.split('/');
                    if (path.length >= 2) {
                        var lastIndex = path.length - 2;
                        var folderPath = '/';

                        for (var i = 0; i <= lastIndex; i++) {
                            var folderName = path[i];
                            if (folderName.length == 0) {
                                folderName = 'Home';
                            } else {
                                folderPath += folderName + '/';
                            }

                            if (i == lastIndex) {
                                container.append('<li><span>' + folderName + '</span></li>');
                            } else {
                                container.append(
                                    $('<li>').append(
                                        $('<a>').attr('href', '?folder=' + folderPath).data('folder', folderPath).on('click', function () {
                                            loadPath($(this).data('folder'));
                                            return false;
                                        }).text(folderName)
                                    )
                                );
                            }
                        }

                        isLoaded = true;
                    }
                }

                if (isLoaded) {
                    $('#browse_path > ul').css('display', '');
                    $('#browse_path > .error_message.no_items').css('display', 'none');
                } else {
                    $('#browse_path > ul').css('display', 'none');
                    $('#browse_path > .error_message.no_items').css('display', '');
                }

                return isLoaded;
            }

            function setFolders(data) {
                var isLoaded = false;
                var allowMoveUp = currentPath != '/';

                var container = $('#browse_folders').first();
                container.find('> a').remove();

                if (allowMoveUp) {
                    var path = currentPath.split('/');
                    if (path.length > 2) {
                        var moveUpPath = '/';
                        for (var i = 1; i < path.length - 2; i++) {
                            moveUpPath += path[i] + '/';
                        }

                        container.append(
                            $('<a>')
                                .attr('href', '?folder=' + moveUpPath)
                                .data('folder', moveUpPath)
                                .on('click', function () { loadPath($(this).data('folder')); return false; })
                                .append('<i class="fas fa-level-up-alt"></i><span>..</span>'));
                    }
                }

                var sum = 0;

                if (data != null && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var folder = data[i];
                        var folderPath = currentPath + folder.name + '/';

                        sum += parseInt(folder.files.replace(',', ''));

                        container.append(
                            $('<a>')
                                .attr('href', '?folder=' + folderPath)
                                .data('folder', folderPath)
                                .on('click', function () { loadPath($(this).data('folder')); return false; })
                                .append('<i class="far fa-folder"></i><span class="file-count">' + String(folder.files) + '</span><span class="name"> ' + folder.name + '</span>'));
                        container.append($('</li>'));
                    }
                }

                var companyName = $('input[id$="CompanyName"]');
                if (companyName != null) {
                    $('.form-subtitle').html(companyName.val() + ' has <span class="folder-files-count">' + sum.toLocaleString() + '</span> uploaded files in selected folder');
                }
                isLoaded = true;

                if (isLoaded || allowMoveUp) {
                    $('#browse_folders > .error_message.no_items').css('display', 'none');
                } else {
                    $('#browse_folders > .error_message.no_items').css('display', '');
                }

                return isLoaded;
            }

            function setFiles(data, enablePreview) {
                previewItems = new Array();

                var isLoaded = false;

                var container = $('#browse_files').first();
                container.find('> div.row').remove();

                if (data != null && data.length > 0) {
                    var $row = null;

                    for (var i = 0; i < data.length; i++) {
                        if (i % 4 == 0) {
                            if ($row != null)
                                container.append($row);

                            $row = $('<div class="row">');
                        }

                        var fileInfo = data[i];
                        var fileUrl = getFileUrl(fileInfo.name);
                        var iconName = getIconName(fileInfo.ext);
                        var isImage = iconName == 'far fa-file-image';

                        var $thumbnail = $('<div class="file-thumbnail">');

                        if (isImage) {
                            previewItems.push({ src: fileUrl, type: 'image' });

                            $thumbnail.addClass('enable-preview')

                        } else if (iconName == 'far fa-file-alt') {
                            previewItems.push({ src: fileUrl, type: 'ajax' });

                            $thumbnail.addClass('enable-preview');
                        }

                        $row.append(
                            $('<div class="col-md-3">').append(
                                $thumbnail.append(
                                    !enablePreview ? null : $('<div class="file-preview">').append(
                                        '<i class="' + iconName + '"></i>',
                                        isImage ? $('<img alt="">').attr('src', fileUrl).css({ opacity: 0 }).on('load', onThumbnailImageLoaded) : null
                                    ),
                                    $('<div class="file-name">' + fileInfo.name + '</div>'),
                                    $('<button class="btn btn-xs btn-icon btn-default btn-fileinfo" title="View Details"><i class="fa fa-bars"></i></button>')
                                        .on('click', onFileInfoClick),
                                    $('<small>' + fileInfo.date + " (" + fileInfo.size + ')</small>')
                                )
                            )
                        );

                        $('.folder-files-count').text((parseInt($('.folder-files-count').text().replace(',', '')) + 1).toLocaleString());
                    }

                    if ($row != null)
                        container.append($row);

                    $('#browse_files .file-thumbnail').each(function () {
                        $(this).click(onFileClick);
                    });

                    isLoaded = true;
                }

                if (isLoaded) {
                    $('#browse_files > .error_message.no_items').css('display', 'none');
                } else {
                    $('#browse_files > .error_message.no_items').css('display', '');
                }

                return isLoaded;
            }

            function setTooltips() {
                $('#browse_folders > a > span,#browse_files .file-thumbnail > span').each(function () {
                    var $this = $(this);
                    if (this.scrollWidth > $this.innerWidth())
                        $this.attr('title', $this.text());
                });
            }

            function setupFilterDefaults() {
                $fileTypeFilter.selectpicker('selectAll');
                fileNameInput.value = '';
                fileCreatedSince.clearDate();
                fileCreatedBefore.clearDate();
            }

            function setupQuickFilter(value) {
                var valueType = typeof value;
                var hasChanges = false;

                if (valueType !== 'undefined') {
                    var $btn = null;
                    var filter = null;
                    var mergeType = 0; // 0 = set, 1 = enable, 2 = disable

                    if (valueType === 'string') {
                        $btn = $quickFilterButtons.filter(function () {
                            return $(this).data('include') === value;
                        });

                        if (quickFilterSettings.hasOwnProperty(value))
                            filter = quickFilterSettings[value];
                    } else if (valueType == 'object' && value instanceof jQuery) {
                        $btn = value;

                        var filterType = String($btn.data('include'));
                        if (quickFilterSettings.hasOwnProperty(filterType))
                            filter = quickFilterSettings[filterType];

                        if ($btn.hasClass('active'))
                            mergeType = 2;
                        else
                            mergeType = 1;
                    }

                    if ($btn == null || $btn.length != 1 || filter == null || typeof filter.data !== 'object')
                        return false;

                    $fileTypeFilter.find('option').each(function () {
                        var $option = $(this);
                        var ext = $option.text().trim();
                        var isChecked = $option.prop('selected');
                        var isMatchFilter = filter.data.hasOwnProperty(ext);

                        if (mergeType === 0) {
                            if (isChecked != isMatchFilter) {
                                $option.prop('selected', isMatchFilter);
                                hasChanges = true;
                            }
                        } else if (mergeType === 1) {
                            if (isMatchFilter && !isChecked) {
                                $option.prop('selected', true);
                                hasChanges = true;
                            }
                        } else if (mergeType === 2) {
                            if (isMatchFilter && isChecked) {
                                $option.prop('selected', false);
                                hasChanges = true;
                            }
                        }
                    });
                    $fileTypeFilter.selectpicker('refresh');
                }

                var checkedInputs = { data: {}, count: 0 };

                $fileTypeFilter.find('option').each(function () {
                    var $option = $(this);
                    if ($option.prop('selected')) {
                        var ext = $option.text().trim();

                        if (typeof ext !== 'undefined') {
                            checkedInputs.data[ext] = true;
                            checkedInputs.count++;
                        }
                    }
                });

                var activeButtons = {};

                for (var fName in quickFilterSettings) {
                    if (!quickFilterSettings.hasOwnProperty(fName))
                        continue;

                    var filter = quickFilterSettings[fName];
                    if (filter.count > 0 && checkedInputs.count > 0) {
                        var isMatch = true;
                        for (var prop in filter.data) {
                            if (!filter.data.hasOwnProperty(prop))
                                continue;

                            if (!checkedInputs.data.hasOwnProperty(prop)) {
                                isMatch = false;
                                break;
                            }
                        }

                        if (isMatch)
                            activeButtons[fName] = true;
                    }
                }

                $quickFilterButtons.each(function () {
                    var $this = $(this);
                    var filterType = $this.data('include');

                    if (typeof filterType !== 'undefined' && activeButtons.hasOwnProperty(filterType))
                        $this.addClass('active');
                    else
                        $this.removeClass('active');
                });

                return hasChanges;
            }

            // event handlers

            function onUploadClick(e) {
                e.preventDefault();
                e.stopPropagation();

                var fileInputId = 'upload_' + new Date().getTime();

                removeFileInput();

                $fileInput = $('<input type="file">')
                    .attr('id', fileInputId)
                    .attr('name', fileInputId)
                    .css('display', 'none')
                    .on('change', onFileUploadChange);

                $('body').append($fileInput);

                $fileInput.trigger('click');
            }

            function onFileUploadChange(e) {
                if (typeof this.files === 'undefined' || this.files.length == 0 || !lock()) {
                    removeFileInput();
                    return;
                }

                var formData = new FormData();
                formData.append('path', currentPath);
                formData.append('file', this.files[0]);

                $.ajax({
                    url: '/api/files/upload',
                    type: 'POST',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,

                    complete: function () {
                        unlock();
                        removeFileInput();

                        loadTypes(function () { loadPath(currentPath); });
                    },
                    error: function (xhr, status, error) {
                        unlock();

                        if (xhr.status == 400) {
                            alert(xhr.responseJSON);
                        }
                        else {
                            alert('An error occurred during request');
                        }
                    }
                });
            }

            function onFileInfoClick(e) {
                e.preventDefault();
                e.stopPropagation();

                if ($.active > 0)
                    return;

                var $sender = $(e.currentTarget).blur();
                var filename = $sender.siblings('div').text();
                
                if (typeof filename != 'string' || filename.length == 0)
                    return;

                allowCloseInfoWindow = true;

                var path = getFilePath(filename);
                var wnd = modalManager.load('<%= FileInfoWindow.ClientID %>', '/ui/admin/assets/uploads/info?path=' + encodeURIComponent(path));

                modalManager.setTitle(wnd, 'Loading...');

                $(wnd)
                    .on('hide.bs.modal', function (e, s, a) {
                        if (!allowCloseInfoWindow) {
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            return false;
                        }
                    })
                    .one('closing.modal.insite', function (e, s, a) {
                        if (a === null)
                            return;

                        if (a.action === 'refresh')
                            loadPath(currentPath);
                    });
            }

            function onFileClick(e) {
                var $sender = $(e.currentTarget);

                var filename = $sender.find('> span').text();
                if (typeof filename != 'string' || filename.length == 0)
                    return;

                var isPreview = false;
                var $target = $(e.target);

                if ($sender.hasClass('enable-preview')) {
                    isPreview = $target.hasClass('file-preview') || $target.parents('.file-preview').length > 0;
                }

                if (isPreview || $target.hasClass('file-name')) {
                    if (!lock())
                        return;
                }

                var url = getFileUrl(filename);

                if (isPreview) {
                    var index = 0;
                    for (var i = 0; i < previewItems.length; i++) {
                        if (previewItems[i].src == url) {
                            index = i;
                            break;
                        }
                    }

                    $.magnificPopup.open({
                        items: previewItems,
                        gallery: { enabled: true },
                        callbacks: {
                            parseAjax: function (r) {
                                r.data = $('<div>').addClass('file-browser-text-viewer').append($('<pre>').text(r.data));
                            },
                        }
                    }, index);

                    setTimeout(unlock, 100);
                } else if ($target.hasClass('file-name')) {
                    window.open(url + '?download=1', '_self');

                    setTimeout(unlock, 100);
                }
            }

            function onThumbnailImageLoaded() {
                var $this = $(this);

                var $thumbnail = $this.parents('.file-preview').first();
                if ($thumbnail.length == 0)
                    return;

                var maxWidth = $thumbnail.width();
                var maxHeight = $thumbnail.height();

                var imageWidth = $this.width();
                var imageHeight = $this.height();

                var hOffset = 0, vOffset = 0;

                if (imageWidth < maxWidth)
                    hOffset = (maxWidth - imageWidth) / 2;

                if (imageHeight < maxHeight)
                    vOffset = (maxHeight - imageHeight) / 2;

                $this.css({
                    opacity: 1,
                    marginTop: vOffset,
                    marginLeft: hOffset,
                    marginBottom: vOffset,
                    marginRight: hOffset,
                });

                $thumbnail.find('> i.fa,> i.fas,> i.far,> i.fab,> i.fal').css('display', 'none');
            }

            // helpers

            function lock() {
                if (autoUnlockHandler != null)
                    return false;

                autoUnlockHandler = setTimeout(function () { unlock(); }, 10000);

                $('#content').addClass('file-browser-locked');

                return true;
            }

            function unlock() {
                if (autoUnlockHandler == null)
                    return false;

                clearTimeout(autoUnlockHandler);

                autoUnlockHandler = null;

                $('#content').removeClass('file-browser-locked');

                return true;
            }

            function getIconName(extension) {
                extension = extension.toLowerCase();

                if (
                    extension == '.tga' ||
                    extension == '.targa' ||
                    extension == '.pix' ||
                    extension == '.jp2' ||
                    extension == '.ico' ||
                    extension == '.jpg' ||
                    extension == '.jpeg' ||
                    extension == '.bmp' ||
                    extension == '.png' ||
                    extension == '.tif' ||
                    extension == '.tiff' ||
                    extension == '.gif'
                )
                    return 'far fa-file-image';

                if (extension == '.txt')
                    return 'far fa-file-alt';

                if (
                    extension == '.vb' ||
                    extension == '.java' ||
                    extension == '.d' ||
                    extension == '.cls' ||
                    extension == '.clj' ||
                    extension == '.s' ||
                    extension == '.c' ||
                    extension == '.h' ||
                    extension == '.asm' ||
                    extension == '.cpp' ||
                    extension == '.cs' ||

                    extension == '.vbs' ||
                    extension == '.sh' ||
                    extension == '.r' ||
                    extension == '.py' ||
                    extension == '.pm' ||
                    extension == '.pl' ||
                    extension == '.php' ||
                    extension == '.lua' ||
                    extension == '.erb' ||
                    extension == '.coffee' ||
                    extension == '.cljs' ||
                    extension == '.ps1' ||
                    extension == '.js' ||
                    extension == '.cmd' ||
                    extension == '.bat' ||

                    extension == '.xml' ||
                    extension == '.html' ||
                    extension == '.htm' ||
                    extension == '.xhtml'
                )
                    return 'far fa-file-code';

                if (extension == '.pdf')
                    return 'far fa-file-pdf';

                if (
                    extension == '.odm' ||
                    extension == '.odt' ||
                    extension == '.rtf' ||
                    extension == '.doc' ||
                    extension == '.docx'
                )
                    return 'far fa-file-word';

                if (
                    extension == '.tsv' ||
                    extension == '.xlr' ||
                    extension == '.xlsx' ||
                    extension == '.xlsm' ||
                    extension == '.xlsb' ||
                    extension == '.sxc' ||
                    extension == '.stc' ||
                    extension == '.xls' ||
                    extension == '.ots' ||
                    extension == '.ods' ||
                    extension == '.csv'
                )
                    return 'far fa-file-excel';

                if (
                    extension == '.ogg' ||
                    extension == '.mid' ||
                    extension == '.aac' ||
                    extension == '.wav' ||
                    extension == '.flac' ||
                    extension == '.wav' ||
                    extension == '.aiff' ||
                    extension == '.wma' ||
                    extension == '.mp3'
                )
                    return 'far fa-file-sound';

                if (
                    extension == '.zip' ||
                    extension == '.rar' ||
                    extension == '.cab' ||
                    extension == '.gzip' ||
                    extension == '.tar' ||
                    extension == '.gz' ||
                    extension == '.tgz' ||
                    extension == '.7z'
                )
                    return 'far fa-file-archive';

                if (
                    extension == '.flv' ||
                    extension == '.wmv' ||
                    extension == '.m4v' ||
                    extension == '.m2v' ||
                    extension == '.m1v' ||
                    extension == '.3gp' ||
                    extension == '.avi' ||
                    extension == '.mp4' ||
                    extension == '.mov' ||
                    extension == '.mpeg' ||
                    extension == '.mpg' ||
                    extension == '.mpe' ||
                    extension == '.mkv'
                )
                    return 'far fa-file-video';

                if (
                    extension == '.dll' ||
                    extension == '.jar' ||
                    extension == '.exe' ||
                    extension == '.com'
                )
                    return 'fa fa-file';

                return 'far fa-file';
            }

            function getFileUrl(filename) {
                return '<%= FileAppUrl %>' + getFilePath(filename);
            }

            function getFilePath(filename) {
                return currentPath + filename;
            }

            function getQuickFilterSettings() {
                var imageExtensions = {
                    '.ico': true,
                    '.jpg': true,
                    '.jpeg': true,
                    '.bmp': true,
                    '.png': true,
                    '.tif': true,
                    '.tiff': true,
                    '.gif': true
                };

                var imgs = { data: {}, count: 0 };
                var notImgs = { data: {}, count: 0 };

                $fileTypeFilter.find('option').each(function () {
                    var $option = $(this);
                    var ext = $option.text().trim();

                    if (typeof ext === 'undefined')
                        return;

                    if (imageExtensions.hasOwnProperty(ext)) {
                        imgs.data[ext] = true;
                        imgs.count++;
                    } else {
                        notImgs.data[ext] = true;
                        notImgs.count++;
                    }
                });

                return {
                    'imgs': imgs,
                    'not-imgs': notImgs
                };
            }

            function removeFileInput() {
                if ($fileInput == null)
                    return;

                $fileInput.remove();
                $fileInput = null;
            }
        });

    </script>
</asp:Content>

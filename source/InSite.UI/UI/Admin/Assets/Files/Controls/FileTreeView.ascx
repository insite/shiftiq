<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileTreeView.ascx.cs" Inherits="InSite.UI.Admin.Assets.Files.Controls.FileTreeView" %>

<%@ Register TagPrefix="uc" TagName="FileTreeViewNode" Src="FileTreeViewNode.ascx" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .widget-categories .widget-link.nosub:before {
            display:none;
        }

        .widget-categories .widget-link.sub:after {
            display:none;
        }

        .widget-categories .widget-link.selected {
            color: #457897;
            background-color: #F7F7FC !important;
        }

        table.files tr td {
            cursor: pointer;
        }

        table.files tr.selected td {
            background-color: #337ab7;
            color: white;
        }

        img.preview-image {
            margin: 10px;
            max-width: 100%;
            max-height: 500px;
            vertical-align: middle;
            overflow: auto;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var treeView = window.treeView = window.treeView || {};

            treeView.showFolder = function (item) {
                $(".widget-categories a.widget-link").removeClass("selected");
                $(item).addClass("selected");

                previewImage(null);

                var isRoot = $(item).data("root") == "1";

                if (isRoot) {
                    $('#<%= RenameFolderButton.ClientID %>').hide();
                    $('#<%= DeleteFolderButton.ClientID %>').hide();
                } else {
                    $('#<%= RenameFolderButton.ClientID %>').show();
                    $('#<%= DeleteFolderButton.ClientID %>').show();
                }

                $('#<%= RenameFileButton.ClientID %>').hide();
                $('#<%= DeleteFileButton.ClientID %>').hide();
                $('#<%= DownloadButton.ClientID %>').hide();

                var path = getPath($(item));

                $('#<%= FolderPath.ClientID %>').val(path);

                __doPostBack('<%= ShowFolderButton.UniqueID %>', '');
            };

            treeView.showFile = function (item) {
                $("table.files tr").removeClass("selected");
                $(item).addClass("selected");

                var url = $(item).data("url");

                previewImage($(item).data("image") == "1" ? url : null);

                $('#<%= DownloadButton.ClientID %>').attr("href", url);

                $('#<%= RenameFileButton.ClientID %>').show();
                $('#<%= DeleteFileButton.ClientID %>').show();
                $('#<%= DownloadButton.ClientID %>').show();
            };

            $('#<%= NewFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();
                $('#<%= NewFolderName.ClientID %>').val("");
                modalManager.show('<%= NewFolderWindow.ClientID %>');
            });

            $('#<%= DeleteFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                deleteFolder();
            });

            $('#<%= CloseNewFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();
                modalManager.close('<%= NewFolderWindow.ClientID %>');
            });

            $('#<%= CreateFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                var name = $('#<%= NewFolderName.ClientID %>').val();

                if (typeof name != 'string' || name.length == 0) {
                    alert("The name cannot be empty");
                    return;
                }

                modalManager.close('<%= NewFolderWindow.ClientID %>');

                createNewFolder();
            });

            $('#<%= RenameFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                var $item = $(".widget-categories a.widget-link.selected");
                var name = $item.data("folder");

                $('#<%= FolderName.ClientID %>').val(name);
                modalManager.show('<%= RenameFolderWindow.ClientID %>');
            });

            $('#<%= CloseRenameFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();
                modalManager.close('<%= RenameFolderWindow.ClientID %>');
            });

            $('#<%= SaveFolderButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                var name = $('#<%= FolderName.ClientID %>').val();

                if (typeof name != 'string' || name.length == 0) {
                    alert("The name cannot be empty");
                    return;
                }

                modalManager.close('<%= RenameFolderWindow.ClientID %>');

                renameFolder();
            });

            $('#<%= UploadFileButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                $('#<%= FileUpload.SelectedFilesClientID %>').val("");
                $('#<%= FileUpload.SelectedFileNamesClientID %>').val("");
                $('#<%= FileUpload.UploadProgressClientID %>').css('width', '0%');
                $('#<%= FileUpload.UploadProgressClientID %>').text('');

                modalManager.show('<%= UploadWindow.ClientID %>');
            });

            $('#<%= CloseUploadButton.ClientID %>').on("click", function (e) {
                e.preventDefault();
                modalManager.close('<%= UploadWindow.ClientID %>');
            });

            $('#<%= SubmitUploadButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                if ($('#<%= FileUpload.UploadProgressClientID %>').text().length == 0) {
                    alert("Please select the file and upload it before saving changes");
                    return;
                }

                uploadFile();

                modalManager.close('<%= UploadWindow.ClientID %>');
            });

            $('#<%= RenameFileButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                var $tr = $("table.files tr.selected");
                var name = $tr.data("filename");

                $('#<%= FileName.ClientID %>').val(name);
                modalManager.show('<%= RenameFileWindow.ClientID %>');
            });

            $('#<%= CloseRenameFileButton.ClientID %>').on("click", function (e) {
                e.preventDefault();
                modalManager.close('<%= RenameFileWindow.ClientID %>');
            });

            $('#<%= SaveFileButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                var name = $('#<%= FileName.ClientID %>').val();

                if (typeof name != 'string' || name.length == 0) {
                    alert("The name cannot be empty");
                    return;
                }

                modalManager.close('<%= RenameFileWindow.ClientID %>');

                renameFile();
            });

            $('#<%= DeleteFileButton.ClientID %>').on("click", function (e) {
                e.preventDefault();

                deleteFile();
            });

            function createNewFolder() {
                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "createFolder",
                        name: $('#<%= NewFolderName.ClientID %>').val(),
                        folderPath: $('#<%= FolderPath.ClientID %>').val(),
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var $item = $(".widget-categories a.widget-link.selected");
                        var $div = $item.parent().find("> div").first();

                        $item
                            .removeClass("collapsed")
                            .removeClass("nosub")
                            .addClass("sub");

                        $div.addClass("collapse show");

                        $div.show();
                        $div.html(data);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function deleteFolder() {
                var folderPath = $('#<%= FolderPath.ClientID %>').val();
                var name = folderPath.substring(folderPath.lastIndexOf('/') + 1);

                if (!confirm("Are you sure you want to delete '" + name + "'?")) {
                    return;
                }

                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "deleteFolder",
                        folderPath: folderPath,
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var $div = $(".widget-categories a.widget-link.selected").closest("li > div").first();

                        $div.show();
                        $div.html(data);

                        var $parent = $div.parent().find("> a.widget-link").first();

                        if (data.length == 0) {
                            $div.removeClass("collapse").removeClass("show");
                            $parent.removeClass("sub").addClass("nosub");
                        }

                        treeView.showFolder($parent[0]);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function renameFolder() {
                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "renameFolder",
                        name: $('#<%= FolderName.ClientID %>').val(),
                        folderPath: $('#<%= FolderPath.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var index = data.indexOf('\n');
                        var name = data.substring(0, index);
                        var nameEncoded = data.substring(index + 1);

                        var $item = $(".widget-categories a.widget-link.selected");
                        $item.data("folder", name);

                        $item.find("span").html(nameEncoded);

                        var path = getPath($item);

                        $('#<%= FolderPath.ClientID %>').val(path);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function renameFile() {
                var $tr = $("table.files tr.selected");
                var oldName = $tr.data("filename");

                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "renameFile",
                        oldName: oldName,
                        newName: $('#<%= FileName.ClientID %>').val(),
                        folderPath: $('#<%= FolderPath.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var index = data.indexOf('\n');
                        var name = data.substring(0, index);
                        var nameEncoded = data.substring(index + 1);

                        $tr.data("filename", name);

                        $tr.find("td").first().html(nameEncoded);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function deleteFile() {
                var $tr = $("table.files tr.selected");
                var name = $tr.data("filename");

                if (!confirm("Are you sure you want to delete '" + name + "'?")) {
                    return;
                }

                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "deleteFile",
                        name: name,
                        folderPath: $('#<%= FolderPath.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function () {
                        var $item = $(".widget-categories a.widget-link.selected");

                        treeView.showFolder($item[0]);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function uploadFile() {
                $('.loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        action: "uploadFile",
                        files: $('#<%= FileUpload.UploadedFilesClientID %>').val(),
                        folderPath: $('#<%= FolderPath.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function () {
                        var $item = $(".widget-categories a.widget-link.selected");

                        treeView.showFolder($item[0]);
                    },
                    complete: function () {
                        $('.loading-panel').hide();
                    }
                });
            }

            function getPath($item) {
                return $item.length > 0
                    ? getPath($item.closest("ul").closest("li").find("a[data-folder]").first()) + "/" + $item.data("folder")
                    : "";
            }

            function previewImage(url) {
                var $img = $('#<%= PreviewImage.ClientID %>');
                if (typeof url === 'string' && url.length > 0) {
                    $img.attr('src', url).attr('alt', url.substring(url.lastIndexOf('/') + 1)).css('display', 'inline-block');
                } else {
                    $img.css('display', 'none').attr('src', '');
                }
            }
        })();
    </script>
</insite:PageFooterContent>

<insite:LoadingPanel runat="server" />

<div class="mb-2">
    <insite:Button runat="server" ID="NewFolderButton" ButtonStyle="Default" Text="New Folder" Icon="fas fa-folder-plus" />
    <insite:Button runat="server" ID="RenameFolderButton" ButtonStyle="Default" style="display:none;" Text="Rename Folder" Icon="fas fa-pencil" />
    <insite:Button runat="server" ID="DeleteFolderButton" ButtonStyle="Default" style="display:none;" Text="Delete Folder" Icon="fas fa-trash" />

    <insite:Button runat="server" ID="UploadFileButton" ButtonStyle="Default" Text="Upload File" Icon="fas fa-upload" />
    <insite:Button runat="server" ID="RenameFileButton" ButtonStyle="Default" style="display:none;" Text="Rename File" Icon="fas fa-pencil" />
    <insite:Button runat="server" ID="DeleteFileButton" ButtonStyle="Default" style="display:none;" Text="Delete File" Icon="fas fa-trash" />

    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Default" style="display:none;" Text="Download File" Icon="fas fa-download" target="_blank" />
</div>

<div class="row">
    <div class="col-lg-4">
        <div style="height: 600px; overflow-y: scroll; border:1px solid #E9E9F2; padding:5px;">

            <div class="widget widget-categories">
                <uc:FileTreeViewNode runat="server" ID="Files" />
            </div>

        </div>
    </div>
    <div class="col-lg-5">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="FolderPath" />
                <asp:Button runat="server" ID="ShowFolderButton" style="display:none;" />

                <div runat="server" id="NoFiles" class="alert alert-warning" role="alert" visible="false">
                    There are no files in the folder.
                </div>

                <asp:Repeater runat="server" ID="FileRepeater">
                    <HeaderTemplate>
                        <div style="height: 600px; overflow-y: scroll; border:1px solid #E9E9F2; padding:5px;">
                            <table class="table table-striped files">
                                <thead>
                                    <th>File Name</th>
                                    <th>File Size</th>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                                </tbody>
                            </table>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr onclick="treeView.showFile(this);" data-filename='<%# Eval("FileNameJS") %>' data-url='<%# Eval("Url") %>' data-image='<%# Eval("IsImage") %>'>
                            <td>
                                <%# Eval("FileName") %>
                            </td>
                            <td class="text-end">
                                <%# Eval("FileSize") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
    <div class="col-lg-3">
        <img runat="server" id="PreviewImage" alt="Preview Image" style="display:none;" class="preview-image" src="" />
    </div>
</div>

<insite:Modal runat="server" ID="NewFolderWindow" Title="New Folder" Width="600px">
    <ContentTemplate>
        <div class="row mb-2">
            <div class="col-lg-12">
                <insite:TextBox runat="server" ID="NewFolderName" />
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="CreateFolderButton" Text="Create" />
                <insite:CancelButton runat="server" ID="CloseNewFolderButton" />
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="RenameFolderWindow" Title="Rename Folder" Width="600px">
    <ContentTemplate>
        <div class="row mb-2">
            <div class="col-lg-12">
                <insite:TextBox runat="server" ID="FolderName" />
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="SaveFolderButton" Text="Save" />
                <insite:CancelButton runat="server" ID="CloseRenameFolderButton" />
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="RenameFileWindow" Title="Rename File" Width="600px">
    <ContentTemplate>
        <div class="row mb-2">
            <div class="col-lg-12">
                <insite:TextBox runat="server" ID="FileName" />
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="SaveFileButton" Text="Save" />
                <insite:CancelButton runat="server" ID="CloseRenameFileButton" />
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="UploadWindow" Title="Upload File" Width="600px">
    <ContentTemplate>
        <div class="row mb-2">
            <div class="col-lg-12">
                <insite:FileUploadV1 runat="server" ID="FileUpload" LabelText="Select and Upload File:" FileUploadType="Unlimited" />
            </div>
        </div>

        <div class="row mb-2">
            <div class="col-lg-12">
                <b>Max files size allowed:</b> <asp:Literal runat="server" ID="MaxFileAllowedLiteral" /><br />
                <b>File extensions allowed:</b> <asp:Literal runat="server" ID="FileExtensionsAllowedLiteral" />
            </div>
        </div>
        
        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="SubmitUploadButton" />
                <insite:CancelButton runat="server" ID="CloseUploadButton" />
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>
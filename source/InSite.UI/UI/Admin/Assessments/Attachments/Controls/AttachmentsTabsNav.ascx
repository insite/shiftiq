<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentsTabsNav.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.AttachmentsTabsNav" %>

<insite:Nav runat="server" ID="Nav" >

    <insite:NavItem runat="server" ID="ImageTab" Title="Images">
        <div class="row row-attachments mt-3">
            <asp:Repeater runat="server" ID="ImageRepeater">
                <ItemTemplate>
                    <div class="col-md-4" data-keywords='<%#: Eval("KeywordsJson") %>'>
                        <div class="card mb-3">
                            <div class="card-body card-attachment">
                                <div class="file-preview">
                                    <img runat="server" alt="" src='<%# Eval("FileUrl") %>' style="opacity:0;" visible='<%# (bool)Eval("UploadExists") %>'>
                                    <div runat="server" class='<%# "fal fa-file-times no-upload" + ((bool)Eval("UploadExists") ? " d-none" : "") %>'></div>
                                </div>
                                <hr />
                                <table class="table table-striped">
                                    <tr>
                                        <td class="info-label">Asset Title</td>
                                        <td class="info-value" title="<%# Eval("Title") %>"><%# Eval("Title") %></span></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Asset #</td>
                                        <td class="info-value">
                                            <%# Eval("AssetNumber") %>.<%# Eval("AssetVersion") %>
                                            <div class="d-inline-block float-end">
                                                <a runat="server" visible="<%# CanWrite %>" title="Edit Attachment" href='<%# GetEditUrl() %>'><i class="far fa-pencil"></i></a>
                                                <a runat="server" visible="<%# CanWrite %>" title="Delete Attachment" href='<%# GetRemoveUrl() %>'><i class="far fa-trash-alt text-danger"></i></a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Condition</td>
                                        <td class="info-value"><%# Eval("Condition") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Publication<br/>Status</td>
                                        <td class="info-value"><%# Eval("PublicationStatus") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Name</td>
                                        <td class="info-value" title="<%# Eval("FileName") %>">
                                            <a runat="server" href='<%# Eval("FileUrl", "{0}?download=1") %>' visible='<%# (bool)Eval("UploadExists") %>'><%# Eval("FileName") %></a>
                                            <insite:Container runat="server" Visible='<%# !(bool)Eval("UploadExists") %>'>
                                                <span><%# Eval("FileName") %></span>
                                                <span class="fas fa-exclamation-triangle text-danger"></span>
                                            </insite:Container>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Size</td>
                                        <td class="info-value"><%# Eval("FileSize") %></td>
                                    </tr>
                                    <tr runat="server" visible='<%# Eval("ImageResolution") != null %>'>
                                        <td class="info-label">Resolution</td>
                                        <td class="info-value"><%# Eval("ImageResolution") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Dimensions</td>
                                        <td class="info-value"><%# Eval("ImageDimensions") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Palette</td>
                                        <td class="info-value"><%# Eval("Color") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Timestamp</td>
                                        <td class="info-value form-text">
                                            Posted by <%# Eval("Author") %>
                                            <br />
                                            on <%# Eval("PostedOn") %>
                                        </td>
                                    </tr>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("ChangesCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Changes</td>
                                            <td class="info-value">
                                                <%# Eval("ChangesCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Change History" href='<%# GetHistoryUrl() %>'><i class="fas fa-history"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("QuestionsCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Questions</td>
                                            <td class="info-value">
                                                <%# Eval("QuestionsCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Usage Statistic" href='<%# GetUsageUrl() %>'><i class="fas fa-chart-pie"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                </table>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <%# (Container.ItemIndex + 1) % 3 == 0 ? "<div class='row-separator'></div>" : string.Empty %>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="DocumentTab" Title="Documents">
        <div class="row row-attachments mt-3">
            <asp:Repeater runat="server" ID="DocumentRepeater">
                <ItemTemplate>
                    <div class="col-md-4" data-keywords='<%#: Eval("KeywordsJson") %>'>
                        <div class="card mb-3">
                            <div class="card-body card-attachment">
                                <table class="table table-striped">
                                    <tr>
                                        <td class="info-label">Asset Title</td>
                                        <td class="info-value" title="<%# Eval("Title") %>"><%# Eval("Title") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Asset #</td>
                                        <td class="info-value">
                                            <%# Eval("AssetNumber") %>.<%# Eval("AssetVersion") %>
                                            <div class="d-inline-block float-end">
                                                <a runat="server" visible="<%# CanWrite %>" title="Edit Attachment" href='<%# GetEditUrl() %>'><i class="far fa-pencil"></i></a>
                                                <a runat="server" visible="<%# CanWrite %>" title="Delete Attachment" href='<%# GetRemoveUrl() %>'><i class="far fa-trash-alt text-danger"></i></a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Condition</td>
                                        <td class="info-value"><%# Eval("Condition") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Publication<br/>Status</td>
                                        <td class="info-value"><%# Eval("PublicationStatus") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Name</td>
                                        <td class="info-value" title="<%# Eval("FileName") %>">
                                            <a runat="server" href='<%# Eval("FileUrl", "{0}?download=1") %>' visible='<%# (bool)Eval("UploadExists") %>'><%# Eval("FileName") %></a>
                                            <insite:Container runat="server" Visible='<%# !(bool)Eval("UploadExists") %>'>
                                                <span><%# Eval("FileName") %></span>
                                                <span class="fas fa-exclamation-triangle text-danger"></span>
                                            </insite:Container>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Size</td>
                                        <td class="info-value"><%# Eval("FileSize") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Timestamp</td>
                                        <td class="info-value form-text">
                                            Posted by <%# Eval("Author") %>
                                            <br />
                                            on <%# Eval("PostedOn") %>
                                        </td>
                                    </tr>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("ChangesCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Changes</td>
                                            <td class="info-value">
                                                <%# Eval("ChangesCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Change History" href='<%# GetHistoryUrl() %>'><i class="fas fa-history"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("QuestionsCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Questions</td>
                                            <td class="info-value">
                                                <%# Eval("QuestionsCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Usage Statistic" href='<%# GetUsageUrl() %>'><i class="fas fa-chart-pie"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                </table>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <%# (Container.ItemIndex + 1) % 3 == 0 ? "<div class='row-separator'></div>" : string.Empty %>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="OtherTab" Title="Others">
        <div class="row row-attachments mt-3">
            <asp:Repeater runat="server" ID="OtherRepeater">
                <ItemTemplate>
                    <div class="col-md-4" data-keywords='<%#: Eval("KeywordsJson") %>'>
                        <div class="card mb-3">
                            <div class="card-body card-attachment">
                                <table class="table table-striped">
                                    <tr>
                                        <td class="info-label">Asset Title</td>
                                        <td class="info-value" title="<%# Eval("Title") %>"><%# Eval("Title") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Asset #</td>
                                        <td class="info-value">
                                            <%# Eval("AssetNumber") %>.<%# Eval("AssetVersion") %>
                                            <div class="d-inline-block float-end">
                                                <a runat="server" visible="<%# CanWrite %>" title="Edit Attachment" href='<%# GetEditUrl() %>'><i class="far fa-pencil"></i></a>
                                                <a runat="server" visible="<%# CanWrite %>" title="Delete Attachment" href='<%# GetRemoveUrl() %>'><i class="far fa-trash-alt text-danger"></i></a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Condition</td>
                                        <td class="info-value"><%# Eval("Condition") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Publication<br/>Status</td>
                                        <td class="info-value"><%# Eval("PublicationStatus") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Name</td>
                                        <td class="info-value" title="<%# Eval("FileName") %>">
                                            <a runat="server" href='<%# Eval("FileUrl", "{0}?download=1") %>' visible='<%# (bool)Eval("UploadExists") %>'><%# Eval("FileName") %></a>
                                            <insite:Container runat="server" Visible='<%# !(bool)Eval("UploadExists") %>'>
                                                <span><%# Eval("FileName") %></span>
                                                <span class="fas fa-exclamation-triangle text-danger"></span>
                                            </insite:Container>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">File Size</td>
                                        <td class="info-value"><%# Eval("FileSize") %></td>
                                    </tr>
                                    <tr>
                                        <td class="info-label">Timestamp</td>
                                        <td class="info-value form-text">
                                            Posted by <%# Eval("Author") %>
                                            <br />
                                            on <%# Eval("PostedOn") %>
                                        </td>
                                    </tr>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("ChangesCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Changes</td>
                                            <td class="info-value">
                                                <%# Eval("ChangesCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Change History" href='<%# GetHistoryUrl() %>'><i class="fas fa-history"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                    <insite:Container runat="server" Visible='<%# (int)Eval("ChangesCount") > 0 %>'>
                                        <tr>
                                            <td class="info-label">Questions</td>
                                            <td class="info-value">
                                                <%# Eval("QuestionsCount") %>
                                                <div class="d-inline-block ms-2">
                                                    <a title="View Usage Statistic" href='<%# GetUsageUrl() %>'><i class="fas fa-chart-pie"></i></a>
                                                </div>
                                            </td>
                                        </tr>
                                    </insite:Container>
                                </table>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <%# (Container.ItemIndex + 1) % 3 == 0 ? "<div class='row-separator'></div>" : string.Empty %>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </insite:NavItem>

</insite:Nav>

<insite:PageHeadContent runat="server" ID="CommonStyle">
<style type="text/css">
    .card-attachment > .file-preview {
        position: relative;
        cursor: default;
    }

        .card-attachment > .file-preview img {
            max-height: 100%;
            max-width: 100%;
            transition: opacity 0.2s ease-in-out 0s;
            position: absolute;
            left: 0;
        }

        .card-attachment > .file-preview > .no-upload {
            font-size: 80px;
            margin: 10px 0;
            text-align: center;
            display: block;
            color: #888888;
        }

    .card-attachment > table {
        table-layout: fixed;
        margin-bottom: 0;
    }

        .card-attachment > table > tbody > tr > td.info-label {
            white-space: nowrap;
            width: 110px;
        }

        .card-attachment > table > tbody > tr > td.info-value {
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
        }

</style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            var instance = window.attachmentsTabsNav = window.attachmentsTabsNav || {};

            instance.registerKeywordInput = function (navId, inputId) {
                var inputSelector = '';
                if (inputId)
                    inputSelector = '#' + String(inputId);

                var data = {
                    $nav: $('#' + String(navId)),
                    $input: $(inputSelector)
                };
                if (data.$nav.length !== 1 || data.$input.length !== 1 || data.$nav.data('keyword'))
                    return;

                data.$nav.data('keyword', data);
                data.$input.on('keydown change', onKeywordChange).data('keyword', data);

                onKeywordChange.call(data.$input[0]);
            };

            function clearKeywordTimeout(input) {
                var $input = $(input);

                var timeoutId = $input.data('timeout');
                if (timeoutId)
                    clearTimeout(timeoutId);

                $input.data('timeout', null);
            }

            function onKeywordChange() {
                var $this = $(this);
                clearKeywordTimeout($this);
                timeoutId = setTimeout(onKeywordUpdate, 500, this);
                $this.data('timeout', timeoutId)
            }

            function onKeywordUpdate(input) {
                clearKeywordTimeout(input);
                var $input = $(input);
                var value = $input.val().toUpperCase().trim();
                var hasValue = value.length > 0;
                var data = $input.data('keyword');


                data.$nav.find('.row-attachments').each(function () {
                    var $row = $(this);
                    $row.find('> .row-separator').remove();

                    var counter = 0;

                    $row .find('> [data-keywords]').each(function () {
                        var $item = $(this);
                        var isMatch = false;

                        if (hasValue) {
                            var keywords = $item.data('keywords');
                            for (var i = 0; i < keywords.length; i++) {
                                if (keywords[i].indexOf(value) != -1) {
                                    isMatch = true;
                                    break;
                                }
                            }
                        } else {
                            isMatch = true;
                        }

                        if (isMatch) {
                            $item.removeClass('d-none');

                            counter++;

                            if (counter % 3 == 0)
                                $item.after('<div class="row-separator">');
                        } else {
                            $item.addClass('d-none');
                        }
                    });
                });
            }
        })();

        (function () {
            var instance = window.attachmentsTabsNav = window.attachmentsTabsNav || {};

            var navs = {};
            var isResized = false;

            $(window).on('resize', onWindowResize).on('click', onWindowClick);

            instance.registerImages = function (navId) {
                var $nav = $('#' + String(navId));
                if ($nav.data('images'))
                    return;

                var $images = $nav.find('.row .card-attachment .file-preview img');
                if ($images.length == 0)
                    return;

                $images.on('load', onImageLoaded)
                $images.on('error', onImageError)
                $nav.data('images', $images);

                $images.each(function () {
                    this.src = this.src;
                });

                navs[navId] = true;

                onWindowResize();
            };

            // events handlers

            function onImageLoaded() {
                var $this = $(this);

                var $filePreview = $this.closest('.file-preview');
                if ($filePreview.length === 1) {
                    $this.css({ opacity: 1 });

                    setImageOffset($this);
                }
            }

            function onImageError() {
                setTimeout(function (img) {
                    $(img).siblings('.no-upload').removeClass('d-none').end().remove();
                }, 0, this);
            }

            function onWindowClick() {
                if (isResized)
                    onWindowResize();
            }

            function onWindowResize() {
                isResized = true;

                for (var id in navs) {
                    if (!navs.hasOwnProperty(id))
                        continue;

                    var $nav = $('#' + String(id));
                    var $images = $nav.data('images');
                    if (!$images || $images.length == 0)
                        continue;

                    if (!$images.is(':visible'))
                        return;

                    $images.each(function () {
                        setImageOffset(this);
                    });
                }

                isResized = false;
            }

            // methods

            function setImageOffset(img) {
                var $img = $(img);

                var $preview = $img.closest('.file-preview');
                if ($preview.length == 0)
                    return;

                var maxWidth = $preview.width();
                var maxHeight = maxWidth / 1.777;

                if (maxWidth == 0)
                    return;

                $preview.height(maxHeight);

                var imageWidth = $img.width();
                var imageHeight = $img.height();

                var hOffset = 0, vOffset = 0;

                if (imageWidth < maxWidth)
                    hOffset = (maxWidth - imageWidth) / 2;

                if (imageHeight < maxHeight)
                    vOffset = (maxHeight - imageHeight) / 2;

                $img.css({
                    marginTop: vOffset,
                    marginLeft: hOffset,
                    marginBottom: vOffset,
                    marginRight: hOffset,
                });
            }
        })();
    </script>
</insite:PageFooterContent>
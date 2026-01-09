<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.Admin.Courses.Courses.Forms.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/CourseInfo.ascx" TagName="CourseDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Download" />

    <section runat="server" ID="DownloadPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-download me-1"></i>
            Download
        </h2>

        <div class="row mb-3">
            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Output Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                File Name
                                <insite:RequiredValidator runat="server" ControlToValidate="FileName" ValidationGroup="Download" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="FileName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">File Type</label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="FileTypeSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div runat="server" id="FileFormatField" class="form-group mb-3">
                            <label class="form-label">File Format</label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                                </asp:RadioButtonList>
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div  class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Course</h3>

                        <uc:CourseDetail ID="CourseDetail" runat="server" />

                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-lg-12">
                                <div runat="server" id="JsonPropertiesField" class="form-group mb-3" style="position:relative;" visible="false">
                                    <label class="form-label">
                                        Import Properties
                                    </label>
                                    <div>
                                        <asp:CheckBoxList runat="server" ID="ObjectPropertiesSelector" RepeatColumns="4" RepeatLayout="Table" CssClass="table table-condensed table-props">
                                        </asp:CheckBoxList>
                                    </div>
                                </div>

                                <div runat="server" id="MarkdownPropertiesField" class="form-group mb-3" style="position:relative;" visible="false">
                                    <label class="form-label">
                                        Import Properties
                                    </label>
                                    <div>
                                        <asp:RadioButtonList runat="server" ID="MarkdownMode">
                                            <asp:ListItem Value="Shell" Text="Only Shell (can be used on upload screen)" Selected="true" />
                                            <asp:ListItem Value="Full" Text="Full Outline" />
                                        </asp:RadioButtonList>

                                        <asp:CheckBoxList runat="server" ID="CheckBoxList1" RepeatColumns="4" RepeatLayout="Table" CssClass="table table-condensed table-props">
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">Compression</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="CompressionMode">
                                            <Items>
                                                <insite:ComboBoxOption Text="Disabled" Selected="true" />
                                                <insite:ComboBoxOption Text="ZIP File" Value="ZIP" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>



    <div class="row">
        <div class="col-lg-12">
            <insite:DownloadButton runat="server" ID="DownloadButton" ValidationGroup="Download" />
            <insite:CloseButton runat="server" ID="CancelLink" />
        </div>
    </div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .table-props > tbody > tr:first-child > td {
            border-top: none;
        }

        .table-props > tbody > tr > td > input[type="checkbox"] {
            position: absolute;
            margin: 5px 3px 0 5px;
        }

        .table-props > tbody > tr > td > label {
            margin-bottom: 0;
            padding-left: 22px;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $container = $('#<%= JsonPropertiesField.ClientID %>');
            if ($container.length === 0)
                return;

            var $icon = $('<i class="far">');
            var $toggle = $('<br/><a>')
                .on('click', onToggleClick)
                .attr('href', 'javascript:void(0);')
                .attr('title', 'Toggle')
                .css({
                    opacity: '0.6',
                    margin: '1px 5px',
                    display: 'none'
                })
                .append($icon);
            var isDeselectState = null;
            var isToggleDisabled = false;

            $container.find('> label').append($toggle);
            $container.find('> div table input[type="checkbox"]').on('change', onCheckChange);

            getState();

            function onToggleClick() {
                isToggleDisabled = true;

                if (isDeselectState) {
                    $container.find('> div table input[type="checkbox"]').prop('checked', false);
                } else {
                    $container.find('> div table input[type="checkbox"]').prop('checked', true);
                }

                isToggleDisabled = false;

                getState();
            }

            function onCheckChange() {
                if (!isToggleDisabled)
                    getState();
            }

            function getState() {
                if ($container.find('> div table input[type="checkbox"]').length == 0)
                    isDeselectState = null;
                else if ($container.find('> div table input[type="checkbox"]:checked').length == 0)
                    isDeselectState = false;
                else
                    isDeselectState = true;

                setupState();
            }

            function setupState() {
                if (isDeselectState === true) {
                    $toggle.attr('title', 'Deselect All').show();
                    $icon.removeClass('fa-check-square').addClass('fa-square');
                } else if (isDeselectState === false) {
                    $toggle.attr('title', 'Select All').show();
                    $icon.removeClass('fa-square').addClass('fa-check-square');
                } else {
                    $toggle.hide();
                }
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>

<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/StandardInfo.ascx" TagName="StandardDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="DocumentSettingsManager" Src="~/UI/Admin/Standards/Documents/Controls/PrintSettingsManager.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Download" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="DownloadSection" Title="Download" Icon="far fa-download" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Download</h2>
                <div class="row">
                    <div class="col-md-6 mb-3">
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
                                        <asp:RadioButtonList runat="server" ID="FileTypeSelector" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                </div>

                                <div runat="server" id="FileFormatField" class="form-group mb-3">
                                    <label class="form-label">File Format</label>
                                    <div>
                                        <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:RadioButtonList>
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
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label class="form-label">Language</label>
                                            <div>
                                                <insite:LanguageComboBox runat="server" ID="LanguageSelection" AllowBlank="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6 mb-3">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Standard</h3>
                                <uc:StandardDetails ID="StandardDetails" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>

                <div runat="server" id="ObjectPropertiesField" class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="form-group mb-3 position-relative">
                            <label class="form-label">
                                Import Properties
                            </label>
                            <div>
                                <asp:CheckBoxList runat="server" ID="ObjectPropertiesSelector" RepeatColumns="4" RepeatLayout="Table" CssClass="table table-condensed table-props"></asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DocumentSection" Title="Document Settings" Icon="far fa-file" IconPosition="BeforeText">
            <section class="mb-3">
                <h2 class="h4 mt-4 mb-3">Document Settings</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <insite:Alert runat="server" ID="DocumentSectionStatus" />

                        <insite:Container runat="server" ID="DocumentSectionContainer" Visible="false">
                            <div class="row">
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Language
                                        </label>
                                        <div>
                                            <insite:LanguageComboBox runat="server" ID="DocumentLanguageSelector" Width="100%" AllowBlank="false" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Competency Content Position
                                            <insite:RequiredValidator runat="server" ControlToValidate="DocumentCompetencyPositionSelector" FieldName="Competency Content Position" ValidationGroup="Download" />
                                        </label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="DocumentCompetencyPositionSelector" Width="100%" />
                                        </div>
                                        <div class="form-text">
                                            Determines where standards are inserted into document.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Competency Depth
                                            <insite:RequiredValidator runat="server" ControlToValidate="DocumentCompetencyDepthFrom" FieldName="Competency Depth From" ValidationGroup="Download" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="DocumentCompetencyDepthThru" FieldName="Competency Depth Thru" ValidationGroup="Download" />
                                        </label>
                                        <div>
                                            <div style="width: calc(50% - 30px);" class="d-inline-block">
                                                <insite:ComboBox runat="server" ID="DocumentCompetencyDepthFrom" Width="100%" />
                                            </div>
                                            <div style="width: 50px; display: inline-block;" class="d-inline-block text-center">
                                                thru
                                            </div>
                                            <div style="width: calc(50% - 30px);" class="d-inline-block">
                                                <insite:ComboBox runat="server" ID="DocumentCompetencyDepthThru" Width="100%" />
                                            </div>
                                        </div>
                                        <div class="form-text">
                                            Determines number of levels below Area.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Footer Text
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="DocumentFooterText" />
                                        </div>
                                    </div>

                                </div>
                                <div class="col-md-6">
                                    <uc:DocumentSettingsManager runat="server" ID="DocumentSettingsManager" />
                                </div>
                            </div>

                            <div class="row">
                                <div runat="server" id="DocumentCompetencyFieldsPanel" class="col-md-3">
                                    <div class="form-group mb-3">
                                        <label class="form-label">Competency Content Fields</label>
                                        <div>
                                            <asp:CheckBoxList runat="server" ID="DocumentCompetencyFields"></asp:CheckBoxList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group mb-3">
                                        <label class="form-label">Other Settings</label>
                                        <div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsShowFieldHeading" Text="Show Standards Headings" Checked="true" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsOrderedList" Text="Format Standards as An Ordered List" Checked="false" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsBulletedList" Text="Format Standards as A Bulleted List" Checked="false" Visible="false" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsPrintAsChecklist" Text="Print As Checklist" Checked="false" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsRenderPageNumbers" Text="Add Page Numbers" Checked="false" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsRenderToc" Text="Add Table Of Contents" Checked="false" /></div>
                                            <div>
                                                <asp:CheckBox runat="server" ID="DocumentIsRenderPageBreaks" Text="Page Breaks after sections" Checked="true" /></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </insite:Container>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:DownloadButton runat="server" ID="DownloadButton" ValidationGroup="Download" />
        <insite:CancelButton runat="server" ID="CancelLink" />
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
                var $container = $('#<%= ObjectPropertiesField.ClientID %>');
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
    
    <insite:PageFooterContent runat="server" ID="DocumentScript" Visible="false">
        <script type="text/javascript">
            (function () {
                var $isOrderedList = $('#<%= DocumentIsOrderedList.ClientID %>').on('change', onIsOrderedListChanged);
                var $isBulletedList = $('#<%= DocumentIsBulletedList.ClientID %>').on('change', onIsBulletedListChanged);
                var $isPrintAsChecklist = $('#<%= DocumentIsPrintAsChecklist.ClientID %>').on('change', onIsPrintAsChecklistChanged);
                var $competencyPositionCombo, $competencyDepthComboFrom, $competencyDepthComboThru;

                Sys.Application.add_load(onLoad);

                function onLoad() {
                    $competencyPositionCombo = $('#<%= DocumentCompetencyPositionSelector.ClientID %>');

                    $competencyDepthComboFrom = $('#<%= DocumentCompetencyDepthFrom.ClientID %>')
                        .off('change', onCompetencyDepthFromChanged)
                        .on('change', onCompetencyDepthFromChanged);

                    $competencyDepthComboThru = $('#<%= DocumentCompetencyDepthThru.ClientID %>')
                        .off('change', onCompetencyDepthThruChanged)
                        .on('change', onCompetencyDepthThruChanged);

                    onIsPrintAsChecklistChanged();
                    onIsBulletedListChanged();
                    onIsOrderedListChanged();
                    onCompetencyDepthFromChanged();
                    onCompetencyDepthThruChanged();
                }

                function onIsPrintAsChecklistChanged() {
                    if ($isPrintAsChecklist.prop('disabled') === true)
                        return;

                    var isChecked = $isPrintAsChecklist.prop('checked');

                    $('#<%= DocumentCompetencyFields.ClientID %> input[type="checkbox"]').prop('disabled', isChecked);
                    $('#<%= DocumentIsShowFieldHeading.ClientID %>').prop('disabled', isChecked);
                    $('#<%= DocumentIsOrderedList.ClientID %>').prop('disabled', isChecked);
                    $('#<%= DocumentIsBulletedList.ClientID %>').prop('disabled', isChecked);
                    $('#<%= DocumentIsRenderToc.ClientID %>').prop('disabled', isChecked);
                    $('#<%= DocumentIsRenderPageBreaks.ClientID %>').prop('disabled', isChecked);

                    $competencyPositionCombo.prop('disabled', isChecked)
                    $competencyPositionCombo.selectpicker('refresh');

                    $competencyDepthComboFrom.prop('disabled', isChecked)
                    $competencyDepthComboFrom.selectpicker('refresh');

                    $competencyDepthComboThru.prop('disabled', isChecked)
                    $competencyDepthComboThru.selectpicker('refresh');
                }

                function onIsBulletedListChanged() {
                    if ($isBulletedList.prop('disabled') === true)
                        return;

                    var isChecked = $isBulletedList.prop('checked');

                    $isOrderedList.prop('disabled', isChecked);

                    <% if (DocumentIsPrintAsChecklist.Enabled) { %>
                    $isPrintAsChecklist.prop('disabled', isChecked);
                    <% } %>
                }

                function onIsOrderedListChanged() {
                    if ($isOrderedList.prop('disabled') === true)
                        return;

                    var isChecked = $isOrderedList.prop('checked');

                    $isBulletedList.prop('disabled', isChecked);
                    <% if (DocumentIsPrintAsChecklist.Enabled) { %>
                    $isPrintAsChecklist.prop('disabled', isChecked);
                    <% } %>
                }

                function onCompetencyDepthFromChanged() {
                    var from = parseInt($competencyDepthComboFrom.selectpicker('val'));
                    var thru = parseInt($competencyDepthComboThru.selectpicker('val'));

                    if (thru < from)
                        $competencyDepthComboThru.selectpicker('val', String(from));
                }

                function onCompetencyDepthThruChanged() {
                    var from = parseInt($competencyDepthComboFrom.selectpicker('val'));
                    var thru = parseInt($competencyDepthComboThru.selectpicker('val'));

                    if (from > thru)
                        $competencyDepthComboFrom.selectpicker('val', String(thru));
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

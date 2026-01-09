<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Download" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="DocumentSettingsManager" Src="../Controls/DocumentSettingsManager.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
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
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" ID="MainAccordion" Visible="false">
        <insite:AccordionPanel ID="DownloadSection" runat="server" Title="Download" Icon="far fa-download">
            <div class="row settings">

                <div class="col-lg-6">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="File Name" />
                            <insite:RequiredValidator runat="server" ControlToValidate="FileName" ValidationGroup="Download" />
                        </label>
                        <insite:TextBox runat="server" ID="FileName" MaxLength="256" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="File Type" />
                        </label>
                        <div>
                            <asp:RadioButtonList runat="server" ID="FileTypeSelector" RepeatDirection="Vertical" RepeatLayout="Flow" />
                        </div>
                    </div>

                    <div runat="server" id="FileFormatField" class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="File Format" />
                        </label>
                        <div>
                            <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow" />
                        </div>
                    </div>

                    <div runat="server" id="ObjectPropertiesField" class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="File Format" />
                        </label>
                        <div>
                            <asp:CheckBoxList runat="server" ID="ObjectPropertiesSelector" RepeatColumns="3" RepeatLayout="Table" CssClass="table table-condensed table-props" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Compression" />
                        </label>
                        <insite:ComboBox runat="server" ID="CompressionMode" Width="200">
                            <Items>
                                <insite:ComboBoxOption Text="Disabled" Selected="true" />
                                <insite:ComboBoxOption Text="ZIP File" Value="ZIP" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                </div>

            </div>
        </insite:AccordionPanel>
        <insite:AccordionPanel ID="DocumentSection" runat="server" Title="Document Settings" Icon="far fa-file" Visible="false">
            <insite:Alert runat="server" ID="DocumentSectionStatus" />

            <insite:Container runat="server" ID="DocumentSectionContainer" Visible="false">
                <div class="row">
                    <div class="col-md-5">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Language" />
                            </label>
                            <insite:LanguageComboBox runat="server" ID="DocumentLanguageSelector" Width="100%" AllowBlank="false" EnableTranslation="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Footer Text" />
                            </label>
                            <insite:TextBox runat="server" ID="DocumentFooterText" />
                        </div>

                    </div>
                    <div class="d-none col-md-4">
                        <uc:DocumentSettingsManager runat="server" ID="DocumentSettingsManager" />
                    </div>
                </div>
            
                <div class="row">
                    <div runat="server" id="DocumentCompetencyFieldsPanel" class="col-md-12">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Competency Content Fields" />
                            </label>
                            <asp:CheckBoxList runat="server" ID="DocumentCompetencyFields" />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Other Settings" />
                            </label>
                            <div><asp:CheckBox runat="server" ID="DocumentIsShowFieldHeading" Text="Show Standards Headings" Checked="false" Visible="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsOrderedList" Text="Format Standards as An Ordered List" Checked="false" Visible="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsBulletedList" Text="Format Standards as A Bulleted List" Checked="false" Visible="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsPrintAsChecklist" Text="Print As Checklist" Checked="false" Visible="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsRenderPageNumbers" Text="Add Page Numbers" Checked="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsRenderToc" Text="Add Table Of Contents" Checked="false" Visible="false" /></div>
                            <div><asp:CheckBox runat="server" ID="DocumentIsRenderPageBreaks" Text="Page Breaks after sections" Checked="true" /></div>
                        </div>
                    </div>
                </div>

            </insite:Container>
        </insite:AccordionPanel>
    </insite:Accordion>

    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="DownloadButton" Text="Download" Icon="far fa-download" ValidationGroup="Download" ButtonStyle="Success" />
            <insite:CloseButton runat="server" ID="CancelLink" />
        </div>
    </div>

<insite:PageHeadContent runat="server">
    
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="ObjectPropertiesScript" Visible="false">
    <script type="text/javascript">
        (function () {
            var $container = $('#<%= ObjectPropertiesField.ClientID %>');
            if ($container.length === 0)
                return;

            var $icon = $('<i class="far">');
            var $toggle = $('<a>')
                .on('click', onToggleClick)
                .attr('href', 'javascript:void(0);')
                .attr('title', 'Toggle')
                .css({
                    position: 'absolute',
                    //right: '0',
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

            Sys.Application.add_load(onLoad);

            function onLoad() {
                onIsPrintAsChecklistChanged();
                onIsBulletedListChanged();
                onIsOrderedListChanged();
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
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>

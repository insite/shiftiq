<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormsSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.FormsSection" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="FormsUpdatePanel" />

<insite:UpdatePanel runat="server" ID="FormsUpdatePanel">
    <ContentTemplate>
        <div class="row mb-3">
            <div class="col-lg-6 mb-3 mb-lg-0">
                <insite:ComboBox runat="server" ID="FormSelector" />
            </div>
            <div class="col-lg-6 text-end">
                <insite:DropDownButton runat="server" ID="AddButton" IconName="plus-circle" Text="Add" CssClass="d-inline-block" MenuCssClass="dropdown-menu-end">
                    <Items>
                        <insite:DropDownButtonItem Name="AddForm" IconType="Regular" IconName="window" Text="Form" ToolTip="Add a new form" />
                        <insite:DropDownButtonItem Name="AddSection" IconType="Regular" IconName="th-list" Text="Section" ToolTip="Add one or more sections to this form" />
                        <insite:DropDownButtonItem Name="AddFields" IconType="Regular" IconName="question" Text="Existing Question" ToolTip="Add one or more questions to this section on this form" />
                        <insite:DropDownButtonItem Name="AddQuestion" IconType="Regular" IconName="question" Text="New Question" ToolTip="Add a new question to this set" />
                    </Items>
                </insite:DropDownButton>

                <insite:Button runat="server" ID="ReorderFieldsButton" ButtonStyle="Default" ToolTip="Reorder the questions on this form" Text="Reorder" Icon="fas fa-sort" />

                <insite:DropDownButton runat="server" ID="ActionButton" IconName="tools" Text="Action" CssClass="d-inline-block" MenuCssClass="dropdown-menu-end">
                    <Items>
                        <insite:DropDownButtonItem Name="Preview" IconType="Regular" IconName="window" Text="Preview" ToolTip="Preview the form" />
                        <insite:DropDownButtonItem Name="Prepublish" IconType="Regular" IconName="file-alt" Text="Prepublish" ToolTip="Prepublish the form" />
                        <insite:DropDownButtonItem Name="Publish" IconType="Regular" IconName="upload" Text="Publish" ToolTip="Publish the form" />
                        <insite:DropDownButtonItem Name="Print" IconType="Regular" IconName="print" Text="Print" ToolTip="Print the form" />
                        <insite:DropDownButtonItem Name="Unpublish" IconType="Regular" IconName="eraser" Text="Unpublish" ToolTip="Unpublish the form" />
                        <insite:DropDownButtonItem Name="Duplicate" IconType="Regular" IconName="copy" Text="New Version" ToolTip="Duplicate the form with a new version number" />
                        <insite:DropDownButtonItem Name="Archive" IconType="Regular" IconName="archive" Text="Archive" ToolTip="Archive the form" />
                        <insite:DropDownButtonItem Name="Unarchive" IconType="Regular" IconName="box-open" Text="Unarchive" ToolTip="Unarchive the form" />
                    </Items>
                </insite:DropDownButton>

                <insite:Button runat="server" ID="ReportButton" ButtonStyle="Default" ToolTip="View attempts report" Text="Report" Icon="far fa-chart-bar" />
                <insite:Button runat="server" ID="FormWorkshopButton" ButtonStyle="Default" ToolTip="Workshop" Text="Workshop" Icon="far fa-industry-alt" />

                <div runat="server" id="PinSection" class="mt-3">
                    <span runat="server" id="PinLabel" class="pe-1">
                    </span>
                    <insite:IconLink runat="server" ID="SwapFieldsLink" ToolTip="Swap Fields" Name="exchange" />
                    <insite:IconButton runat="server" ToolTip="Unpin All Fields" OnClientClick="bankRead.forms.unpinAllFields(); return false;" Name="thumbtack" />
                </div>
            </div>
        </div>

        <div runat="server" id="FormRow" class="row">
            <div class="col-lg-2 col-4">
                <insite:Nav runat="server" ID="FormSectionsNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="FormSectionsNavContent">

                </insite:Nav>
            </div>
            <div id="forms-nav-content" class="col-lg-10 col-8">
                <insite:NavContent runat="server" ID="FormSectionsNavContent" />
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = window.bankRead = window.bankRead || {};
            instance = window.bankRead.forms = window.bankRead.forms || {};

            instance.pinField = function (link, fieldAssetNumber) {
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: '/ui/admin/assessments/fields/pin',
                    data: { action: 'pin', bank: '<%= BankID %>', fieldAssetNumber: fieldAssetNumber },
                    success: function (result) {
                        if (result.status === 'ok') {
                            if (result.pinned) {
                                $(link).children().attr('class', '<%= UnpinClass %>');
                                $(link).attr('title', '<%= UnpinText %>');
                            } else {
                                $(link).children().attr('class', '<%= PinClass %>');
                                $(link).attr('title', '<%= PinText %>');
                            }

                            if (result.numberOfPins > 0) {
                                $('#<%= PinSection.ClientID %>').show();
                                $('#<%= PinLabel.ClientID %>').html(String(result.numberOfPins) + ' ' + (result.numberOfPins == 1 ? 'Pin' : 'Pins'));

                                if (result.numberOfPins == 2)
                                    $('#<%= SwapFieldsLink.ClientID %>').show();
                                else
                                    $('#<%= SwapFieldsLink.ClientID %>').hide();
                            }
                            else {
                                $('#<%= PinSection.ClientID %>').hide();
                            }
                        } else if (result.status === 'too much') {
                            alert('You cannot pin more than 2 fields.')
                        } else {
                            alert('The second field must be from another section.')
                        }
                    },
                    error: function (xhr) {
                        alert(xhr.status);
                    },
                });
            };

            instance.unpinAllFields = function () {
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: '/ui/admin/assessments/fields/pin',
                    data: { action: 'unpinall', bank: '<%= BankID %>' },
                    success: function (result) {
                        $('.pin-link').children().attr('class', '<%= PinClass %>');
                        $('.pin-link').attr('title', '<%= PinText %>');
                        $('#<%= PinSection.ClientID %>').hide();
                    },
                    error: function (xhr) {
                        alert(xhr.status);
                    },
                });
            };
        })();
    </script>
</insite:PageFooterContent>
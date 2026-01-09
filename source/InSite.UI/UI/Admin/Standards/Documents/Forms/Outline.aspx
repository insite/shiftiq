<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Standards.Documents.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Standards/Occupations/Controls/CompetenciesPanel.ascx" TagName="CompetenciesPanel" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <insite:Alert runat="server" ID="OutlineStatus" />

    <div class="row mb-3">
        <div class="col-md-12">
            <insite:Button runat="server" ID="NewClassLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/standards/documents/create" />

            <insite:ButtonSpacer runat="server" ID="Separator1" />

            <insite:Button runat="server" ID="DuplicateLink" ButtonStyle="Default" Text="Duplicate" Icon="fas fa-copy" />

            <insite:ButtonSpacer runat="server" ID="Separator2" />

            <insite:Button runat="server" ID="DownloadLink" Text="Download JSON" icon="fas fa-download" ButtonStyle="Primary" />
            <insite:DownloadButton runat="server" id="DownloadButton" ToolTip="Download the document" />
            <insite:DeleteButton runat="server" ID="DeleteLink" />
        </div>
    </div>

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="GeneralSection" Title="Document" Icon="far file-alt" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <insite:Nav runat="server">
                                    <insite:NavItem runat="server" Title="Content">
                                        <div class="row row-doc-content settings" >
                                            <div class="col-md-12 mt-2">
                                                <div class="form-group mb-3" data-id="Title">
                                                    <div class="float-end">
                                                        <insite:IconLink Name="pencil" runat="server" id="TitleLink" style="padding:8px" ToolTip="Change Title" />
                                                    </div>
                                                    <label class="form-label">
                                                        Title
                                                        <asp:CheckBox runat="server" ID="IsTitleLocked" Visible="false" />
                                                    </label>
                                                    <div>
                                                        <asp:Literal runat="server" ID="TitleOutput" />
                                                    </div>
                                                </div>

                                                <asp:Repeater runat="server" ID="SectionRepeater">
                                                    <ItemTemplate>
                                                        <div class="form-group mb-3" data-id='<%# Eval("ID") %>'>
                                                            <div class="float-end">
                                                                <insite:IconLink Name="pencil" runat="server" style="padding:8px" ToolTip='<%# Eval("Title", "Change {0}") %>'
                                                                    NavigateUrl='<%# string.Format("/ui/admin/standards/documents/content?asset={0}&tab={1}", StandardIdentifier, Eval("ID")) %>'
                                                                    Visible='<%# CanWrite && (AllowManageFields || !(bool)Eval("IsLocked")) %>' />
                                                            </div>
                                                            <label class="form-label">
                                                                <%# Eval("Title") %>
                                                                <asp:CheckBox runat="server" ID="IsLocked" Checked='<%# Eval("IsLocked") %>' Visible='false' />
                                                            </label>
                                                            <div>
                                                                <%# Eval("Content") %>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                            </div>
                                        </div>
                                    </insite:NavItem>

                                    <insite:NavItem runat="server" Title="Settings">
                                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SettingsUpdatePanel" />
                                        <insite:UpdatePanel runat="server" ID="SettingsUpdatePanel">
                                            <ContentTemplate>
                                                <div class="row settings" >
                                                    <div class="col-md-6">

                                                        <div class="form-group mb-3">
                                                            <label class="form-label">Options</label>
                                                            <div>
                                                                <asp:CheckBox runat="server" ID="IsTemplate" Text="Template" />
                                                            </div>
                                                        </div>

                                                        <div class="form-group mb-3">
                                                            <label class="form-label">
                                                                Privacy Scope
                                                            </label>
                                                            <div>
                                                                <insite:ComboBox runat="server" ID="StandardPrivacyScope" Width="100%" style="max-width: 400px;">
                                                                    <Items>
                                                                        <insite:ComboBoxOption Text="User" Value="User" Selected="true" />
                                                                        <insite:ComboBoxOption Text="Organization" Value="Tenant" />
                                                                    </Items>
                                                                </insite:ComboBox>
                                                            </div>
                                                            <div class="form-text">
                                                                Select <strong>User</strong> if this document is visible only to you.
                                                                Select <strong>Organization</strong> if this document is visible to everyone in your organization.
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </insite:UpdatePanel>
                                    </insite:NavItem>
                                </insite:Nav>
                            </div>
                        </div>
                    </div>
                </div>
            </section>            
        </insite:NavItem>

        <insite:NavItem runat="server" ID="KnowledgeSection" Title="Competencies" Icon="far ruler-triangle" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:CompetenciesPanel runat="server" ID="KnowledgeCompetenciesPanel" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ItemsSection" Title="Competencies" Icon="far ruler-triangle" IconPosition="BeforeText">           
            <section class="pb-5 mb-md-2">        
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:CompetenciesPanel runat="server" ID="ItemsCompetenciesPanel" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RelationshipsSection" Title="Relationships" Icon="far sitemap" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Select Related NOS
                                        <insite:RequiredValidator runat="server" ControlToValidate="RelatedStandardSelector" FieldName="Select Related NOS" ValidationGroup="RelatedStandard" />
                                    </label>
                                    <div>
                                        <insite:FindStandard runat="server" ID="RelatedStandardSelector" Width="500" />
                                        <insite:Button runat="server" ID="RelatedAssignButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="RelatedStandard" Text="Assign NOS" Icon="far fa-plus-circle" ConfirmText="Are you sure to assign this NOS and its competencies?" />
                                    </div>
                                </div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="RelatedUpdatePanel" />
                                <insite:UpdatePanel runat="server" ID="RelatedUpdatePanel">
                                    <ContentTemplate>
                                        <div runat="server" id="RelatedStandardField" class="form-group mb-3" visible="false">
                                            <div>
                                                <asp:Repeater runat="server" ID="RelatedStandardRepeater">
                                                    <HeaderTemplate><table class="table table-striped"></HeaderTemplate>
                                                    <FooterTemplate></table></FooterTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="width:40px;">
                                                                <%# (ConnectionDirection)Eval("Direction") == ConnectionDirection.Incoming ? "<i title='Parent' class='fas fa-level-up-alt'></i>" : "<i title='Child' class='fas fa-level-down-alt'></i>" %>
                                                            </td>
                                                            <td>
                                                                <a href="/ui/admin/standards/documents/outline?asset=<%# Eval("Identifier") %>"><%# Eval("Title") %></a>
                                                            </td>
                                                            <td style="text-align:right;width:40px;">
                                                                <insite:IconButton runat="server" ToolTip="Delete Relationship" CommandName="Delete" Name="trash-alt"
                                                                    style="padding:8px" OnClientClick="return confirm('Are you sure you want to remove this relationship?');" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
    </insite:Nav>
</div>

<insite:PageHeadContent runat="server">

    <style type="text/css">
        .row-doc-content div.form-group > div img {
            max-width: 100%;
        }
    </style>

</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="LockScript">
    <script type="text/javascript">
        (function () {
            var instance = window.documentOutline = window.documentOutline || {};
            var fieldsState = {};
            var saveHandler = null;

            instance.registerLocks = function (ids) {
                if (!(ids instanceof Array) || ids.length === 0)
                    return;

                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var $input = $('#' + String(id));
                    if ($input.length === 0)
                        continue;

                    var $formGroup = $input.closest('.form-group');

                    var $anchor = $('<a style="padding:8px" href="#"><i class="far"></i></a>')
                        .data('data', {
                            $input: $input,
                            id: $formGroup.data('id'),
                            $link: $formGroup.find('> div > a:first')
                        })
                        .on('click', onLockToggle);

                    $input.after($anchor).hide();

                    updateLockAnchor($anchor);
                }
            };

            function onLockToggle(e) {
                e.preventDefault();

                var $anchor = $(this);
                var data = $anchor.data('data');
                var isLocked = !data.$input.prop('checked');

                data.$input.prop('checked', isLocked);

                updateLockAnchor($anchor);

                fieldsState[data.id] = isLocked;

                saveLocks();
            }

            function updateLockAnchor($anchor) {
                var data = $anchor.data('data');
                var isLocked = data.$input.prop('checked');

                var $i = $anchor.find('> i').removeClass('fa-lock-open-alt fa-lock-alt');
                if (isLocked) {
                    $anchor.attr('title', 'Unlock Field');
                    $i.addClass('fa-lock-alt');
                    data.$link.hide();
                } else {
                    $anchor.attr('title', 'Lock Field');
                    $i.addClass('fa-lock-open-alt');
                    data.$link.show();
                }
            }

            function saveLocks(isTimeout) {
                if (isTimeout !== true) {
                    if (saveHandler !== null)
                        clearTimeout(saveHandler);

                    saveHandler = setTimeout(saveLocks, 250, true);

                    return;
                } else {
                    saveHandler = null;
                }

                var hasData = false;

                for (var key in fieldsState) {
                    hasData = fieldsState.hasOwnProperty(key);
                    if (hasData)
                        break;
                }

                if (!hasData)
                    return;

                var requestData =
                {
                    isPageAjax: true,
                    state: JSON.stringify(fieldsState)
                };

                $.ajax({
                    type: 'POST',
                    data: requestData,
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var state = JSON.parse(requestData.state);
                        for (var key in state) {
                            if (fieldsState.hasOwnProperty(key) && state.hasOwnProperty(key) && fieldsState[key] === state[key]) {
                                delete fieldsState[key];
                            }
                        }
                    },
                    complete: function () {
                    }
                });
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>

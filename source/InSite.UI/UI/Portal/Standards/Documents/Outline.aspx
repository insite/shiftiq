<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../Controls/CompetenciesPanel.ascx" TagName="CompetenciesPanel" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        .row-doc-content div.form-group > div img {
            max-width: 100%;
        }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div runat="server" id="ButtonPanel" class="row mb-4">
        <div class="col-lg-12">
            <insite:Button runat="server" id="BackToAnalysis" Icon="far fa-arrow-turn-left" Text="Job Comparison Tool" ButtonStyle="Success" Visible="false"/>
            <insite:DownloadButton runat="server" id="DownloadButton" CssClass="btn-sm" />
        </div>
    </div>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" ID="MainAccordion">

        <insite:AccordionPanel runat="server" ID="GeneralSection" Title="Document" Icon="fas fa-file-alt">
            
            <div class="row row-doc-content settings" >
                <div class="col-md-12">
                    <div class="row mb-3" data-id="Title">
                        <div class="col-lg-11">
                            <h5 class="mt-2 mb-1">
                                <%= Translate("Title") %>
                                <asp:CheckBox runat="server" ID="IsTitleLocked" Visible="false" />
                            </h5>
                            <div>
                                <asp:Literal runat="server" ID="TitleOutput" />
                            </div>
                        </div>
                        <div class="col-lg-1">
                            <div class="float-end">
                                <insite:Button runat="server" id="TitleLink" style="padding:8px;" ToolTip="Change Title" Icon="far fa-pencil" ButtonStyle="Default" />
                            </div>
                        </div>
                    </div>

                    <asp:Repeater runat="server" ID="SectionRepeater">
                        <ItemTemplate>
                            <div class="row mb-3" data-id='<%# Eval("ID") %>'>
                                <div class="col-lg-11">
                                    <h5 class="mt-2 mb-1">
                                        <%# Eval("Title") %>
                                        <asp:CheckBox runat="server" ID="IsLocked" Checked='<%# Eval("IsLocked") %>' Visible='false' />
                                    </h5>
                                    <div>
                                        <%# Eval("Content") %>
                                    </div>
                                </div>
                                <div class="col-lg-1">
                                    <div class="float-end">
                                        <insite:Button runat="server" style="padding:8px" ToolTip='<%# Eval("Tooltip") %>'
                                            NavigateUrl='<%# GetUrl(string.Format("/ui/portal/standards/documents/content?standard={0}&tab={1}", StandardIdentifier, Eval("ID"))) %>'
                                            visible='<%# AllowManageFields && !(bool)Eval("IsLocked") %>'
                                            Icon="far fa-pencil" ButtonStyle="Default" />
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>  
        </insite:AccordionPanel>

        <insite:AccordionPanel ID="KnowledgeSection" runat="server" Title="Competencies" Icon="fas fa-ruler-triangle">
            <uc:CompetenciesPanel runat="server" ID="KnowledgeCompetenciesPanel" />
        </insite:AccordionPanel>

        <insite:AccordionPanel ID="ItemsSections" runat="server" Title="Competencies" Icon="fas fa-ruler-triangle">
            <uc:CompetenciesPanel runat="server" ID="ItemsCompetenciesPanel" />
        </insite:AccordionPanel>

        <insite:AccordionPanel runat="server" ID="RelationshipsTab" Title="Relationships" Icon="fas fa-sitemap">
            <div class="row">
                <div class="col-md-6">

                    <insite:Container runat="server" ID="StandardAdditionPanel">
                        <div class="form-group mb-3">

                            <label class="form-label">
                                <insite:Literal runat="server" Text="Select Related NOS" />
                                <insite:RequiredValidator runat="server" ControlToValidate="RelatedNosStandardSelector" FieldName="Select Related NOS" ValidationGroup="RelatedNosStandard" />
                            </label>

                            <insite:FindStandard runat="server" ID="RelatedNosStandardSelector" EnableTranslation="true" />

                            <div class="text-end mt-3">
                                <insite:Button runat="server" ID="RelatedNosAssignButton" ButtonStyle="Secondary" CausesValidation="true" ValidationGroup="RelatedNosStandard" Icon="far fa-plus-circle" Text="Assign NOS" ConfirmText="Are you sure to assign this NOS and its competencies?" />
                            </div>

                        </div>

                        <div runat="server" id="RelatedOccProfPanel" class="form-group mb-3">

                            <label class="form-label">
                                <insite:Literal runat="server" Text="Selected Related Occupation Profile" />
                                <insite:RequiredValidator runat="server" ControlToValidate="RelatedOccProfStandardSelector" FieldName="Select Related Occupation Profile" ValidationGroup="RelatedOccProfStandard" />
                            </label>

                            <insite:FindStandard runat="server" ID="RelatedOccProfStandardSelector" EnableTranslation="true" />

                            <div class="text-end mt-3">
                                <insite:Button runat="server" ID="RelatedOccProfAssignButton" ButtonStyle="Secondary" CausesValidation="true" ValidationGroup="RelatedOccProfStandard" Icon="far fa-plus-circle" Text="Assign Occupation Profile" ConfirmText="Are you sure to assign this Occupation Profile and its competencies?" />
                            </div>

                        </div>
                    </insite:Container>

                    <div runat="server" id="RelatedStandardField" class="mb-3" visible="false">
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
                                            <a href="/ui/portal/standards/documents/outline?standard=<%# Eval("Identifier") %>"><%# Eval("Title") %></a>
                                        </td>
                                        <td style="text-align:right;width:40px;">
                                            <insite:Button runat="server" ID="DeleteRelationshipButton" ToolTip="Delete Relationship" CommandName="Delete" Icon="far fa-trash-alt"
                                                style="padding:8px" ConfirmText="Are you sure you want to remove this relationship?" ButtonStyle="Default" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
        </insite:AccordionPanel>

    </insite:Accordion>

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

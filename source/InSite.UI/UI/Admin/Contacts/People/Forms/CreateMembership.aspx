<%@ Page Language="C#" CodeBehind="CreateMembership.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.CreateRole" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.table-results {
        }

            table.table-results > tbody > tr {
                cursor: pointer;
            }

                table.table-results > tbody > tr.table-active {
                    cursor: default;
                }

                table.table-results > tbody > tr > td.cell-select {
                    width: 40px;
                }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Membership" />

    <div class="row">
        <div class="col-md-4 mb-3 mb-md-0">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-search me-1"></i>
                        Search
                    </h4>

                    <div class="form-group mb-3 criteria-field">
                        <label class="form-label">
                            Group Type
                        </label>
                        <insite:GroupTypeComboBox runat="server" ID="CriteriaGroupType" DropDown-Size="15" />
                    </div>

                    <div class="form-group mb-3 criteria-field">
                        <label class="form-label">
                            Keyword
                        </label>
                        <insite:TextBox runat="server" ID="CriteriaKeyword" MaxLength="200" />
                    </div>

                    <div>
                        <insite:FilterButton runat="server" ID="CriteriaSearchButton" CausesValidation="false" ButtonStyle="OutlinePrimary" />
                        <insite:ClearButton runat="server" ID="CriteriaClearButton" ButtonStyle="OutlineSecondary" />
                    </div>

                </div>
            </div>

            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-sliders-h me-1"></i>
                        Role Settings
                    </h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Membership Function
                        </label>
                        <insite:ItemNameComboBox runat="server" ID="RoleType" Width="100%">
                            <Settings UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" CollectionName="Contacts/Memberships/Membership/Type" />
                        </insite:ItemNameComboBox>
                    </div>

                </div>
            </div>

            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-question me-1"></i>
                        Reason
                    </h4>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReasonUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="ReasonUpdatePanel">
                        <ContentTemplate>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Reason Subtype
                                </label>
                                <insite:ItemNameComboBox runat="server" ID="ReasonSubtype" Width="100%">
                                    <Settings UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" CollectionName="Contacts/Settings/Referrers/Name" />
                                </insite:ItemNameComboBox>
                            </div>

                            <insite:Container runat="server" ID="ReasonFieldsContainer" Visible="false">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Person Occupation
                                    </label>
                                    <insite:TextBox runat="server" ID="ReasonPersonOccupation" MaxLength="100" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Reason Effective Date
                                        <insite:RequiredValidator runat="server" ControlToValidate="ReasonEffectiveDate" FieldName="Reason Effective Date" ValidationGroup="Membership" />
                                    </label>
                                    <insite:DateTimeOffsetSelector runat="server" ID="ReasonEffectiveDate" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Reason Expiry Date
                                    </label>
                                    <insite:DateTimeOffsetSelector runat="server" ID="ReasonExpiryDate" />
                                </div>
                            </insite:Container>

                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

        </div>
        <div class="col-md-8 mt-3 mt-md-0">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="mb-3">
                        <h4 class="card-title mb-3">
                            <i class="far fa-database me-1"></i>
                            Groups
                            <span runat="server" id="SearchResultCount" class="badge rounded-pill bg-info ms-1"></span>
                            <span runat="server" id="SearchSelectCount" class="badge rounded-pill bg-success ms-1"></span>
                        </h4>

                        <insite:UpdatePanel runat="server" ID="SearchResultUpdatePanel" Visible="false">
                            <ContentTemplate>
                                <table class="table table-hover table-sm table-results">
                                    <thead>
                                        <tr>
                                            <th>
                                                <input type="checkbox" >
                                            </th>
                                            <th>Name</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater runat="server" ID="SearchResultRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="cell-select">
                                                        <asp:CheckBox runat="server" ID="Selected" />
                                                    </td>
                                                    <td>
                                                        <div><a title="Message Outline" href='/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>'><%# Eval("GroupName") %></a></div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                    <insite:Container runat="server" ID="SearchResultFooter">
                                        <tfoot>
                                            <tr>
                                                <td colspan="2">
                                                    <insite:Pagination runat="server" ID="SearchResultPagination" />
                                                </td>
                                            </tr>
                                        </tfoot>
                                    </insite:Container>
                                </table>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <insite:Container runat="server" ID="SearchNoResultContainer" Visible="true">
                            <p>No groups found matching your search criteria.</p>
                        </insite:Container>

                    </div>

                    <div>
                        <insite:SaveButton runat="server" ID="SearchResultSaveButton" Text="Add" Icon="fas fa-plus-circle" ValidationGroup="Membership" />
                        <insite:CloseButton runat="server" ID="SearchResultCloseButton" />
                    </div>

                </div>
            </div>

        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var $selectCount;
                var otherSelectCount;

                Sys.Application.add_load(function () {
                    $('.criteria-field')
                        .off('keydown', onCriteriaFieldKeyDown)
                        .on('keydown', onCriteriaFieldKeyDown);
                });

                function onCriteriaFieldKeyDown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        e.stopPropagation();
                        document.getElementById('<%= CriteriaSearchButton.ClientID %>').click();
                    }
                }

                Sys.Application.add_load(function () {
                    var $table = $('table.table-results');
                    if ($table.length == 0)
                        return;

                    $selectCount = $(document.getElementById('<%= SearchSelectCount.ClientID %>'));

                    otherSelectCount = parseInt($selectCount.text().replace(/[^0-9]/g, ''));
                    if (isNaN(otherSelectCount))
                        otherSelectCount = 0;

                    var tableData = {
                        $checkAll: $('table.table-results > thead > tr > th input[type="checkbox"]'),
                        $checkInputs: $('table.table-results > tbody > tr > td.cell-select input[type="checkbox"]')
                    };

                    $table.data('data', tableData);

                    tableData.$checkInputs.each(function () {
                        if (this.checked)
                            otherSelectCount--;
                    });

                    tableData.$checkAll
                        .off('change', onCheckAllChange)
                        .on('change', onCheckAllChange);

                    tableData.$checkInputs
                        .off('click', onCheckClick)
                        .on('click', onCheckClick)
                        .off('change', onCheckChange)
                        .on('change', onCheckChange);

                    $('table.table-results > tbody > tr')
                        .off('click', onRowClick)
                        .on('click', onRowClick);

                    onCheckChange.call($table[0]);
                });

                function onCheckAllChange() {
                    var tableData = $(this).closest('table').data('data');
                    var isChecked = this.checked;
                    var checkedCount = 0;

                    tableData.$checkInputs.prop('checked', isChecked);

                    if (isChecked)
                        checkedCount = tableData.$checkInputs.length;

                    $selectCount.text((otherSelectCount + checkedCount).toLocaleString());
                }

                function onCheckChange() {
                    var tableData = $(this).closest('table').data('data');
                    var isAllChecked = tableData.$checkInputs.length > 0;
                    var checkedCount = 0;

                    tableData.$checkInputs.each(function () {
                        if (this.checked)
                            checkedCount++;
                        else
                            isAllChecked = false;
                    });

                    tableData.$checkAll.prop('checked', isAllChecked);
                    $selectCount.text((otherSelectCount + checkedCount).toLocaleString());
                }

                function onCheckClick(e) {
                    e.stopPropagation();
                }

                function onRowClick(e) {
                    var $chk = $(this).find('> td.cell-select input[type="checkbox"]');
                    if ($chk.length == 1)
                        $chk.trigger('click');
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
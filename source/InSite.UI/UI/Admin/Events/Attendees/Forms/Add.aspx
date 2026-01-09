<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Events.Attendees.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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

    <div class="row mb-3">
        <div class="col-lg-12">

            <div class="row">
                <div class="col-md-4 mb-3 mb-md-0">
                    
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">

                            <h4 class="card-title mb-3">
                                <i class="far fa-search me-1"></i>
                                Search
                            </h4>

                            <div class="form-group mb-3 criteria-field">
                                <label class="form-label">
                                    Role
                                </label>
                                <div>
                                    <insite:MultiField runat="server" CssClass="form-group">
                                        <insite:MultiFieldView runat="server" ID="CriteriaRoleSelectorView">
                                            <span class="multi-field-input">
                                                <insite:GroupComboBox runat="server" ID="CriteriaRoleSelector" EnableSearch="true" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server">
                                            <span class="multi-field-input">
                                                <insite:TextBox runat="server" ID="CriteriaRoleText" MaxLength="20" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                        </insite:MultiFieldView>
                                    </insite:MultiField>
                                </div>
                                <div class="mt-2">
                                    <insite:ComboBox runat="server" ID="CriteriaEnableRoleFilter" Width="100%">
                                        <Items>
                                            <insite:ComboBoxOption Value="True" Text="Filter on selected role" />
                                            <insite:ComboBoxOption Value="False" Text="Do not filter on role" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                            </div>

                            <div class="form-group mb-3 criteria-field">
                                <label class="form-label">
                                    Name
                                </label>
                                <insite:TextBox ID="CriteriaContactName" runat="server" MaxLength="200" />
                            </div>

                            <div class="form-group mb-3 criteria-field">
                                <label class="form-label">
                                    Email
                                </label>
                                <insite:TextBox ID="CriteriaEmail" runat="server" MaxLength="200" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Code(s)
                                </label>
                                <insite:TextBox ID="CriteriaContactCode" runat="server" TextMode="MultiLine" Rows="2" />
                            </div>

                            <div>
                                <insite:FilterButton runat="server" ID="CriteriaSearchButton" CausesValidation="false" ButtonStyle="OutlinePrimary" />
                                <insite:ClearButton runat="server" ID="CriteriaClearButton" ButtonStyle="OutlineSecondary" />
                            </div>

                        </div>
                    </div>

                    <div runat="server" id="NewUserCard" class="card border-0 shadow-lg mb-3" visible="false">
                        <div class="card-body">

                            <h4 class="card-title mb-3">
                                <i class="far fa-plus-circle me-1"></i>
                                New User
                            </h4>

                            <div>
                                <insite:Button runat="server" ID="CreateContactLink" ButtonStyle="OutlineSuccess" Text="New Contact(s)" Icon="fas fa-plus-circle" CssClass="mb-3" />
                                <insite:Button runat="server" ID="UploadContactLink" ButtonStyle="OutlineSuccess" Text="Upload Contact(s)" Icon="fas fa-upload" CssClass="mb-3" />
                            </div>

                        </div>
                    </div>

                </div>

                <div class="col-md-8">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="mb-3">
                                <h4 class="card-title mb-3">
                                    <i class="far fa-database me-1"></i>
                                    People
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
                                                    <th>Person</th>
                                                    <th>Email</th>
                                                    <th>Roles</th>
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
                                                                <a href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'><%# Eval("FullName") %></a>
                                                                <span class="form-text"><%# Eval("PersonCode") %></span>
                                                            </td>
                                                            <td>
                                                                <%# Eval("Email", "<a href='mailto:{0}'>{0}</a>") %>
                                                                <span class="form-text"><%# Eval("EmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
                                                            </td>
                                                            <td>
                                                                <%# GetRoles(Container.DataItem) %>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                            <insite:Container runat="server" ID="SearchResultFooter">
                                                <tfoot>
                                                    <tr>
                                                        <td colspan="4">
                                                            <insite:Pagination runat="server" ID="SearchResultPagination" />
                                                        </td>
                                                    </tr>
                                                </tfoot>
                                            </insite:Container>
                                        </table>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <insite:Container runat="server" ID="SearchNoResultContainer" Visible="true">
                                    <p>No people found matching your search criteria.</p>
                                </insite:Container>

                            </div>

                            <div>
                                <insite:SaveButton runat="server" ID="SearchResultSaveButton" Text="Add" Icon="fas fa-plus-circle" />
                                <insite:CloseButton runat="server" ID="SearchResultCloseButton" />
                            </div>

                        </div>
                    </div>

                </div>

            </div>

        </div>
    </div>

    <insite:ProgressPanel runat="server" ID="SaveProgress" HeaderText="Adding Contacts">
        <Triggers>
            <insite:ProgressControlTrigger ControlID="SearchResultSaveButton" />
        </Triggers>
        <Items>
            <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
            <insite:ProgressStatus Text="Status: {status}{running_ellipsis}" />
            <insite:ProgressStatus>
                Elapsed time: <span style='display:inline-block;width:80px;'>{time_elapsed}s</span>
                Time remaining: <span style='display:inline-block;width:80px;'>{time_remaining}</span>
            </insite:ProgressStatus>
        </Items>
    </insite:ProgressPanel>

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

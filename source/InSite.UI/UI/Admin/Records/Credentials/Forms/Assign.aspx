<%@ Page Language="C#" CodeBehind="Assign.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Assign" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AssignGrid.ascx" TagName="AssignGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" TagName="ExpirationField" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Credential" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="SearchCriteriaSection" Title="Search" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Search
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <div class="row">
                        <div id="toolbox" class="col-lg-6 toolbox-section">
                            <label class="form-label">
                                <insite:TextBox ID="ContactName" runat="server" EmptyMessage="Contact Name" MaxLength="200" />
                            </label>
                            <label class="form-label">
                                <insite:TextBox ID="Email" runat="server" EmptyMessage="Email" MaxLength="200" />
                            </label>

                            <insite:SearchButton runat="server" ID="SearchButton" />
                            <insite:ClearButton runat="server" ID="ClearButton" />
                        </div>

                        <div class="col-lg-4 float-end text-end">
                            <insite:Button runat="server" ID="CreateContactLink" ButtonStyle="Success" Text="New Contact(s)" Icon="fas fa-plus-circle" />
                            <insite:Button runat="server" ID="UploadContactLink" ButtonStyle="Success" Text="Upload Contact(s)" Icon="fas fa-upload" />
                        </div>
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchResultsSection" Title="Results" Icon="far fa-database" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Results
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <uc:AssignGrid runat="server" ID="AssignGrid" />
                        </div>
                    </div>

                    <div style="padding-top:15px;">
                        <insite:NextButton runat="server" ID="NextButton" />
                        <insite:CancelButton runat="server" ID="CancelButton1" ConfirmText="Are you sure to cancel?" />
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SavedUsersSection" Title="Results" Icon="far fa-database" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Results
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Repeater runat="server" ID="SavedUsersRepeater">
                                <HeaderTemplate>
                                    <table id="SavedUsersTable" class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th style="width:40px;">
                                                    <input type="checkbox" id="SelectAll2" onclick="onSelectAll2();" />
                                                </th>
                                                <th>Person</th>
                                                <th>Email</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <FooterTemplate></tbody></table></FooterTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:CheckBox runat="server" ID="IsSelected" Checked="true" />
                                        </td>
                                        <td>
                                            <a href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'><%# Eval("FullName") %></a>
                                            <span class="form-text"><%# Eval("PersonCode") %></span>
                                        </td>
                                        <td>
                                            <%# Eval("Email", "<a href='mailto:{0}'>{0}</a>") %>
                                            <span class="form-text"><%# Eval("EmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <insite:NextButton runat="server" ID="NextSavedUsersButton" />
                            <insite:CancelButton runat="server" ID="CancelSavedUsersButton" ConfirmText="Are you sure to cancel?" />
                        </div>
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CredentialPanel" Title="Credential" Icon="far fa-award" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Credential
                </h2>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CredentialUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="CredentialUpdatePanel">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="card border-0 shadow-lg h-100">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Assigned
                                                <insite:RequiredValidator runat="server" ID="AssignedRequiredValidator" FieldName="Assigned" ControlToValidate="Assigned" ValidationGroup="Credential" />
                                            </label>
                                            <div>
                                                <insite:DateTimeOffsetSelector runat="server" ID="Assigned" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                        <uc:ExpirationField runat="server" ID="ExpirationField" LabelText="Expiration" ValidationGroup="Credential" />

                                    </div>
                                </div>
                                    
                            </div>
                            <div class="col-md-6">
                                <div class="card border-0 shadow-lg h-100">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Necessity
                                            </label>
                                            <div>
                                                <insite:ItemNameComboBox runat="server" ID="Necessity" Settings-CollectionName="Achievements/Credentials/Necessity/Status" Width="100%" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>
                        
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Priority
                                            </label>
                                            <div>
                                                <insite:ItemNameComboBox runat="server" ID="Priority" Settings-CollectionName="Achievements/Credentials/Priority/Status" Width="100%" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Status
                                            </label>
                                            <div>
                                                <asp:RadioButtonList runat="server" ID="CredentialStatus">
                                                    <asp:ListItem Value="Assigned" Text="Assigned but <strong>not</strong> Granted" Selected="True" />
                                                    <asp:ListItem Value="Granted" Text="Assigned <strong>and</strong> Granted" />
                                                </asp:RadioButtonList>
                                                <div runat="server" id="GrantedDateField" visible="false">
                                                    <insite:DateTimeOffsetSelector runat="server" ID="GrantedDate" />
                                                </div>
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

                <div class="pt-3">
                    <insite:SaveButton runat="server" ID="SaveButton" />
                    <insite:CancelButton runat="server" ID="CancelButton2" ConfirmText="Are you sure to cancel?" />
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

<insite:PageFooterContent runat="server" ID="ScriptLiteral">
    <script type="text/javascript">

        (function () {
            $('#<%= ContactName.ClientID %>')
                .off('keydown', onKeyDown)
                .on('keydown', onKeyDown);

            function onKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= SearchButton.ClientID %>')[0].click();
                }
            }
        })();

        function onSelectAll2() {
            var checked = $("#SelectAll2").prop("checked");
            $("#SavedUsersTable input[type=checkbox]").prop("checked", checked);
        }

    </script>
</insite:PageFooterContent>
</asp:Content>

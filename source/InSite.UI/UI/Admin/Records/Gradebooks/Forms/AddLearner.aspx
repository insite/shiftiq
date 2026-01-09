<%@ Page Language="C#" CodeBehind="AddLearner.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.AddStudent" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AddLearnerGrid.ascx" TagName="AddLearnerGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />

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
                            <div class="mb-2">
                                <insite:TextBox ID="FullName" runat="server" EmptyMessage="Name" MaxLength="200" />
                            </div>

                            <div class="mb-2">
                                <insite:TextBox ID="Email" runat="server" EmptyMessage="Email" MaxLength="200" />
                            </div>

                            <div class="mb-2">
                                <insite:FindEvent runat="server" ID="RegistrationEventIdentifier" ShowPrefix="false" EmptyMessage="Class" />
                            </div>

                            <div class="mb-2">
                                <insite:SearchButton runat="server" ID="SearchButton" />
                                <insite:ClearButton runat="server" ID="ClearButton" />
                            </div>
                        </div>
                        <div class="col-lg-4 float-end text-end">
                            <insite:Button runat="server" ID="CreateContactLink" Text="New Contact(s)" Icon="fas fa-plus-circle" ButtonStyle="Success" />
                            <insite:Button runat="server" ID="UploadContactLink" Text="Upload Contact(s)" Icon="fas fa-upload" ButtonStyle="Success" />
                        </div>
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchResultsSection" Title="Results" Icon="far fa-database" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    <asp:Literal runat="server" ID="SearchResultsTitle" Text="Results" />
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <div id="Students" class="row">
                        <div class="col-md-12">

                            <uc:AddLearnerGrid runat="server" ID="StudentGrid" />

                        </div>
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
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:Button runat="server" ID="SaveButton" Text="Add" Icon="fas fa-plus-circle" ButtonStyle="Success" Visible="false" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

<insite:PageFooterContent runat="server">
    <script>
        function onSelectAll() {
            var checked = $("#SelectAll").prop("checked");
            $("#Students input[type=checkbox]").prop("checked", checked);
        }
        function onSelectAll2() {
            var checked = $("#SelectAll2").prop("checked");
            $("#SavedUsersTable input[type=checkbox]").prop("checked", checked);
        }
    </script>
</insite:PageFooterContent>
</asp:Content>

<%@ Page Language="C#" CodeBehind="PersonOutline.aspx.cs" Inherits="InSite.UI.Admin.Records.Instructors.Forms.PersonOutline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/GradeTreeViewNode.ascx" TagName="GradeTreeViewNode" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:Nav runat="server" ID="NavPanel">

    <insite:NavItem runat="server" ID="PersonalSection" Title="Person" Icon="far fa-user" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Person
            </h2>

            <div class="row">

                <div class="col-md-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Personal</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                </label>
                                <div>
                                    <asp:Literal ID="ContactCode" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Full Name
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Honorific" />
                                    <asp:Literal runat="server" ID="FullName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Email
                                </label>
                                <div>
                                    <asp:Literal ID="Email" runat="server" />
                                    <asp:Literal ID="EmailAlt" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Birthdate</label>
                                <div>
                                    <asp:Literal ID="Birthdate" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Gender</label>
                                <div>
                                    <asp:Literal runat="server" ID="Gender" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Emergency contact</label>
                                <div>
                                    <asp:Literal ID="EmergencyContactName" runat="server" />
                                </div>
                                <div>
                                    <asp:Literal ID="EmergencyContactPhoneNumber" runat="server" />
                                </div>
                                <div>
                                    <asp:Literal ID="EmergencyContactRelationship" runat="server" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Phone Numbers</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Preferred</label>
                                <div>
                                    <asp:Literal ID="Phone" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Home</label>
                                <div>
                                    <asp:Literal ID="PhoneHome" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Work</label>
                                <div>
                                    <asp:Literal ID="PhoneWork" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Mobile</label>
                                <div>
                                    <asp:Literal ID="PhoneMobile" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Other</label>
                                <div>
                                    <asp:Literal ID="PhoneOther" runat="server" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            
                            <h3>Description</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Region</label>
                                <div>
                                    <asp:Literal ID="Region" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    English Language Learner
                                    <asp:CheckBox ID="ESL" runat="server" Text="" Enabled="false" />
                                </label>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                </label>
                                <div>
                                    <asp:Literal ID="PersonCode" runat="server" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="AddressSection" Title="Address" Icon="far fa-home" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Address
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <insite:Grid runat="server" ID="AddressList">

                        <Columns>
                            <asp:TemplateField HeaderText="Address Type">
                                <ItemTemplate>
                                    <%# Eval("AddressType") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Address">
                                <ItemTemplate>
                                    <%# Eval("Address.Street1") %> <%# Eval("Address.Street2") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="City">
                                <ItemTemplate>
                                    <%# Eval("Address.City") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Province">
                                <ItemTemplate>
                                    <%# Eval("Address.Province") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Postal Code">
                                <ItemTemplate>
                                    <%# Eval("Address.PostalCode") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </insite:Grid>

                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="AchievementSection" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Achievements
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <insite:Grid runat="server" ID="AchievementGrid">
                        <Columns>

                            <asp:TemplateField HeaderText="Achievement">
                                <ItemTemplate>
                                    <%# Eval("AchievementTitle") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Credential Status">
                                <ItemTemplate>
                                    <%# Eval("CredentialStatus") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Credential Granted">
                                <ItemTemplate>
                                    <%# GetLocalTime(Eval("CredentialGranted")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Credential Revoked">
                                <ItemTemplate>
                                    <%# GetLocalTime(Eval("CredentialRevoked")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Credential Expired">
                                <ItemTemplate>
                                    <%# GetLocalTime(Eval("CredentialExpired")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </insite:Grid>

                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="RegistrationSection" Title="Registrations" Icon="far fa-id-card" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Registrations
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <insite:Grid runat="server" ID="Registrations">


                        <Columns>
                            <asp:TemplateField HeaderText="Class Title">
                                <ItemTemplate>
                                    <%# Eval("Event.EventTitle") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Registered">
                                <ItemTemplate>
                                    <%# LocalizeDate(Eval("RegistrationRequestedOn")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Hours Worked to Date" ItemStyle-Width="50px" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <%# Eval("WorkBasedHoursToDate") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approval" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <%# Eval("ApprovalStatus") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer">
                                <ItemTemplate>
                                    <%# Eval("Customer.GroupName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Employer">
                                <ItemTemplate>
                                    <%# Eval("Employer.GroupName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Comments">
                                <ItemTemplate>
                                    <%# Eval("RegistrationComment") != null ? ((string)Eval("RegistrationComment")).Replace("\n", "<br>") : "" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Attendance" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <%# Eval("AttendanceStatus") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </insite:Grid>

                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="GradeSection" Title="Records" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Records
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <asp:Literal runat="server" ID="NoGrades" Text="No records to display." />

                    <asp:Panel runat="server" ID="GradePanel">
                        <div id="grade-tree-accordion-panel">
                            <div class="row">
                                <div class="col-md-12">
                                    <button id="grade-expand-all" type="button" class="btn btn-sm btn-default d-inline"><i class="fas fa-chevron-down me-1"></i>Expand All</button>
                                    <button id="grade-collapse-all" type="button" class="btn btn-sm btn-default d-inline"><i class="fas fa-chevron-up me-1"></i>Collapse All</button>
                                </div>
                            </div>

                            <div class="tree-view-container" style="padding-top: 15px;">
                                <ul class='tree-view' data-init="code">
                                    <uc:GradeTreeViewNode runat="server" ID="Grades" />
                                </ul>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </section>
    </insite:NavItem>

</insite:Nav>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        function initGradeTreeView() {
            inSite.common.treeView.init($('#grade-tree-accordion-panel .tree-view-container > .tree-view'), {
                expand: $('#grade-tree-accordion-panel #grade-expand-all'),
                collapse: $('#grade-tree-accordion-panel #grade-collapse-all'),
                state: 'ui.desktops.admin.records.instructors.personoutline.grades.state.<%= UserIdentifier %>',
                defaultLevel: 2
            });
        };
    </script>
</insite:PageFooterContent>
</asp:Content>

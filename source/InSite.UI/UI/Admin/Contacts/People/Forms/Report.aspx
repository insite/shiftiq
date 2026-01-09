<%@ Page Language="C#" CodeBehind="Report.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.Report" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/GradeTreeViewNode.ascx" TagName="GradeTreeViewNode" TagPrefix="uc" %>
<%@ Register Src="../Controls/UserInvoiceGrid.ascx" TagName="UserInvoiceGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:PageHeadContent runat="server">
        <style>
            .tree-view > li > div > div table td {
                vertical-align: top;
            }

            .tree-view > li > div > div {
                padding-right: 5px;
            }

            .anchor {
                display: block;
                position: relative;
                top: -60px;
                visibility: hidden;
            }

            @media print {
                @page {
                    size: A4 portrait;
                }

                .panel-title i {
                    display: none !important;
                }

                .no-print {
                    display: none !important;
                }

                .panel-title a::after {
                    display: none !important;
                }

                .col-md-6 h3 {
                    display: none !important;
                }

                .col-md-5 h3 {
                    display: none !important;
                }
            }
        </style>
    </insite:PageHeadContent>

    <div class="mb-3">
        <insite:Button runat="server" ID="TrainingRecordsPrintButton" ButtonStyle="Default" CssClass="no-print" Icon="far fa-print" Text="Print Training Records" />
        <insite:Button runat="server" ID="PrintButton" ButtonStyle="Default" CssClass="no-print" icon="far fa-print" Text="Print Membership Summary" />
    </div>

    <section runat="server" id="MembershipSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Membership Summary</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-4 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Member</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Member Number
                                    </label>
                                    <div>
                                        <asp:Literal ID="MembershipNumber" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Full Name
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipHonorific" />
                                        <asp:Literal runat="server" ID="MembershipFullName" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Member Address
                                    </label>
                                    <div>
                                        <asp:Literal ID="MembershipHomeAddress" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Shipping Preference
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipShippingPreference" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Phone Numbers
                                    </label>
                                    <div>
                                        <asp:Literal ID="MembershipPhoneNumbers" runat="server" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div runat="server" id="MembershipSchool" class="col-md-4 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">School</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        School
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipSchoolDisctrict" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipSchoolName" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipSchoolNumber" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipSchoolAddress" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        School Phone Numbers
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipSchoolPhoneNumbers" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Membership</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Position
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipPosition" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Membership Status
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipStatus2" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Participations
                                    </label>
                                    <asp:Repeater runat="server" ID="ParticipationsRepeater">
                                        <ItemTemplate>
                                            <div><%# Eval("Group.GroupName") %></div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <a name="person" class="anchor"></a>

    <section id="PersonalSection" runat="server" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Person</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">

                    <div class="col-md-4 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Personal</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Contact Code
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
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Birthdate
                                    </label>
                                    <div>
                                        <asp:Literal ID="Birthdate" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Gender
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="Gender" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Emergency contact
                                    </label>
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

                    <div class="col-md-4 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Phone Numbers</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Preferred
                                    </label>
                                    <div>
                                        <asp:Literal ID="Phone" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Home
                                    </label>
                                    <div>
                                        <asp:Literal ID="PhoneHome" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Work
                                    </label>
                                    <div>
                                        <asp:Literal ID="PhoneWork" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Mobile
                                    </label>
                                    <div>
                                        <asp:Literal ID="PhoneMobile" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Other
                                    </label>
                                    <div>
                                        <asp:Literal ID="PhoneOther" runat="server" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Description</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Region
                                    </label>
                                    <div>
                                        <asp:Literal ID="Region" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Website URL
                                    </label>
                                    <div>
                                        <asp:Literal ID="WebSiteUrl" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Shipping Preference
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ShippingPreference" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        English Language Learner
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ESL" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>

            </div>        
        </div>

    </section>

    <a name="membership" class="anchor"></a>

    <section id="MemershipSection" runat="server" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Membership</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">

                    <div class="col-md-6 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Employment</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Employer Number
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="EmployerNumber" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label runat="server" id="EmployerFieldTitle" class="form-label">
                                        Employer
                                    </label>
                                    <div runat="server" id="EmployerFieldValue">
                                        <div>
                                            <asp:Literal runat="server" ID="DistrictName" />
                                        </div>
                                        <div>
                                            <asp:Literal runat="server" ID="EmployerName" />
                                        </div>
                                        <div>
                                            <asp:HyperLink
                                                runat="server"
                                                NavigateUrl="https://www.google.com/maps"
                                                ID="EmployerAddress"
                                                Target="_new" />
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Employer Phone
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="EmployerPhone" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-md-6 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">
                                <h5 class="card-title mb-3">Membership</h5>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Job Title
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="JobTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Membership Status
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="MembershipStatus1" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Union Info
                                    </label>
                                    <div>
                                        <asp:Literal ID="UnionInfo" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Tradeworker Number
                                    </label>
                                    <div>
                                        <asp:Literal ID="TradeworkerNumber" runat="server" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </section>

    <a name="addresses" class="anchor"></a>

    <section id="AddressSection" runat="server" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="AddressSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Grid runat="server" id="AddressList">

                    <columns>
                        <asp:TemplateField HeaderText="Address Type">
                            <ItemTemplate>
                                <%# Eval("AddressType") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Address 1">
                            <ItemTemplate>
                                <%# Eval("Address.Street1") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Address 2">
                            <ItemTemplate>
                                <%# Eval("Address.Street2") %>
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
                        <asp:TemplateField HeaderText="Map">
                            <headertemplate>
                                <span class="fa fa-map"></span>
                            </headertemplate>
                            <ItemTemplate>
                                <a href="<%# Eval("GmapsLink") %>" target="_blank">Locate</a>
                            
                            </ItemTemplate>
                        </asp:TemplateField>
                    </columns>
                </insite:Grid>

            </div>
        </div>

    </section>

    <a name="groups" class="anchor"></a>

    <section runat="server" id="RoleSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="RoleSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Grid runat="server" id="RoleGrid" DataKeyNames="GroupIdentifier">
                    <Columns>
                        <asp:BoundField HeaderText="Group Type" DataField="Group.GroupType" />
                        <asp:BoundField HeaderText="Group Name" DataField="Group.GroupName" />
                    </Columns>
                </insite:Grid>

            </div>
        </div>

    </section>

    <a name="registrations" class="anchor"></a>

    <section runat="server" id="RegistrationSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="RegistrationSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoRegistrations" Text="No registrations to display." />

                <insite:Grid runat="server" id="Registrations">
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
                        <asp:TemplateField HeaderText="Fee" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                            <ItemTemplate>
                                <%# Eval("RegistrationFee", "{0:n2}") %>
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

    <a name="invoices" class="anchor"></a>

    <section runat="server" id="InvoiceSection" class="pb-5 mb-md-2">

        <uc:UserInvoiceGrid runat="server" id="UserInvoice" />

    </section>

    <a name="gradebooks" class="anchor"></a>

    <section runat="server" id="GradeSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="GradeSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoGrades" Text="No records to display." />

                <asp:Panel runat="server" ID="GradePanel">
                    <div id="grade-tree-accordion-panel">
                        <div class="row">
                            <div class="col-md-3">
                                <button id="grade-expand-all" type="button" class="btn btn-sm btn-default"><i class="fas fa-chevron-down me-1"></i>Expand All</button>
                                <button id="grade-collapse-all" type="button" class="btn btn-sm btn-default"><i class="fas fa-chevron-up me-1"></i>Collapse All</button>
                            </div>
                            <div class="col-md-9">
                            </div>
                        </div>

                        <div class="tree-view-container mt-3">
                            <ul class='tree-view' data-init="code">
                                <uc:GradeTreeViewNode runat="server" id="Grades" />
                            </ul>
                        </div>
                    </div>
                </asp:Panel>

            </div>
        </div>

    </section>

    <a name="cases" class="anchor"></a>

    <section runat="server" id="CaseSection" class="pb-5 mb-md-2" visible="false">

        <h2 class="h4 mb-3">
            <asp:Literal runat="server" ID="CaseSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoCases" Text="No cases to display." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CaseUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="CaseUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" ID="CaseGrid">
                            <Columns>

                                <asp:TemplateField ItemStyle-Width="20">
                                    <ItemTemplate>
                                        <a href="/ui/admin/workflow/cases/outline?case=<%# Eval("IssueIdentifier") %>"><i class="fas fa-pencil"></i></a>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <%# Eval("IssueStatusName") %>
                                        <div><%# Eval("IssueStatusCategoryHtml") %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Summary">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server"
                                            NavigateUrl='<%# Eval("IssueIdentifier", "/ui/admin/workflow/cases/outline?case={0}") %>'
                                            Text='<%# Eval("IssueTitle") %>' />
                                        <div class="fs-sm text-body-secondary">
                                            <i class="far fa-paperclip me-2"></i><%# Eval("AttachmentCount") %> Attachments
                                        </div>
                                        <div class="fs-sm text-body-secondary">
                                            <i class="far fa-comment me-2"></i><%# Eval("CommentCount") %> Comments
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Opened" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# LocalizeDate(Eval("IssueOpened")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Closed" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# LocalizeDate(Eval("IssueClosed")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <a name="forms" class="anchor"></a>

    <section runat="server" id="SurveySection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="SurveySectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoSurveys" Text="No forms to display." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SurveyUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="SurveyUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" id="SurveyGrid">
                            <Columns>

                                <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                    <ItemTemplate>
                                        <insite:IconLink runat="server" Name="search" ToolTip="View Submission Session"
                                            NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/workflow/forms/submissions/outline?session={0}") %>' 
                                            Visible='<%# !(bool)Eval("EnableUserConfidentiality") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Started" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# GetLocalDate(Eval("Started")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
    
                                <asp:TemplateField HeaderText="Completed" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# GetLocalDate(Eval("Completed")) %>
                                        <div>
                                            <span runat="server" visible='<%# !(bool)Eval("IsLocked") %>' class='badge bg-success'>
                                                <i class='fas fa-lock-open'></i>&nbsp;Unlocked
                                            </span>
                                            <span runat="server" visible='<%# Eval("IsLocked") %>' class='badge bg-danger'>
                                                <i class='fas fa-lock'></i>&nbsp;Locked
                                            </span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
    
                                <asp:TemplateField HeaderText="Form">
                                    <ItemTemplate>
                                        <%# Eval("SurveyName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Group">
                                    <ItemTemplate>
                                        <%# Eval("GroupName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Period">
                                    <ItemTemplate>
                                        <%# Eval("PeriodName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <a name="attempts" class="anchor"></a>

    <section runat="server" id="AssesmentSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="AssesmentSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoAssesments" Text="No assesments to display." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AssesmentUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="AssesmentUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" id="AssesmentGrid">
                            <Columns>

                               <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <insite:IconLink runat="server" ID="ViewAttemptLink" Name="search"
                                            NavigateUrl='<%# Eval("AttemptIdentifier", "/ui/admin/assessments/attempts/view?attempt={0}") %>' 
                                            Visible='<%# (bool)Eval("IsAdmin") %>'
                                        />
                                    </ItemTemplate>
                                </asp:TemplateField>
                
                                <asp:TemplateField HeaderText="Exam Form">
                                    <ItemTemplate>
                                        <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>"><%# Eval("FormTitle") %></a>
                                        <div class="form-text">
                                            <%# Eval("FormName") %>
                                            &bull;
                                            Exam Form Asset #<%# Eval("FormAsset") %>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Date and Time">
                                    <ItemTemplate>
                                        <%# Eval("FormTime") %> 
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Grading Assessor">
                                    <ItemTemplate>
                                        <%# Eval("GradingAssessor") %> 
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <a name="achievements" class="anchor"></a>

    <section runat="server" id="AchievementSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="AchievementSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoAchievements" Text="No achievements to display." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchievementUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="AchievementUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" id="AchievementGrid">
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
                                        <%# GetLocalDate("CredentialGranted") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Credential Revoked">
                                    <ItemTemplate>
                                        <%# GetLocalDate("CredentialRevoked") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Credential Expired">
                                    <ItemTemplate>
                                        <%# GetLocalDate("CredentialExpired") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <a name="comments" class="anchor"></a>

    <section runat="server" id="CommentSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="CommentSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Literal runat="server" ID="NoComments" Text="No comments to display." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CommentUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="CommentUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" ID="CommentGrid">
                            <Columns>
                                <asp:TemplateField HeaderText="Author">
                                    <ItemTemplate>
                                        <%# Eval("AuthorName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Posted" ItemStyle-Width="110px">
                                    <ItemTemplate>
                                        <%# GetLocalDate("Posted") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Comment">
                                    <ItemTemplate>
                                        <%# Eval("Description") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </div>

    </section>

    <a name="messages" class="anchor"></a>

    <section runat="server" id="MessageSection" name="MessageSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3"><asp:Literal runat="server" ID="MessageSectionTitle" /></h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <asp:Literal runat="server" ID="NoMessages" Text="There are no deliveries." />

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="MessageUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="MessageUpdatePanel">
                    <ContentTemplate>
                        <insite:Grid runat="server" ID="MessageGrid">
                            <Columns>
                                <asp:TemplateField HeaderText="Sender Address">
                                    <ItemTemplate>
                                        <%# Eval("SenderEmail") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sender Name">
                                    <ItemTemplate>
                                        <%# Eval("SenderName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Message">
                                    <ItemTemplate>
                                        <%# Eval("ContentSubject") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sent">
                                    <ItemTemplate>
                                        <%# GetLocalTime("DeliveryCompleted") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="20px" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <insite:IconLink runat="server" Name="search" ToolTip="View Email" NavigateUrl='<%# Eval("ViewEmailUrl") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </insite:Grid>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </div>

    </section>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function initGradeTreeView() {
                inSite.common.treeView.init($('#grade-tree-accordion-panel .tree-view-container > .tree-view'), {
                    expand: $('#grade-tree-accordion-panel #grade-expand-all'),
                    collapse: $('#grade-tree-accordion-panel #grade-collapse-all'),
                    state: 'admin.contacts.people.report.grades.state.<%= UserIdentifier %>',
                    defaultLevel: 2
                });
            };
        </script>
    </insite:PageFooterContent>

</asp:Content>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Issues.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="IssueIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="assign-checkbox hide" ItemStyle-CssClass="assign-checkbox hide">
            <HeaderTemplate>
                <asp:CheckBox runat="server" ID="AllCheckBox" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="SelectCheckBox" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20">
            <ItemTemplate>
                <a href="/ui/admin/workflow/cases/outline?case=<%# Eval("IssueIdentifier") %>"><i class="fas fa-pencil"></i></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="#">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("IssueIdentifier", "/ui/admin/workflow/cases/outline?case={0}") %>'
                    Text='<%# Eval("IssueNumber") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <strong><%# Eval("IssueType") %></strong>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("IssueStatusName") %>
                <div><%# Eval("IssueStatusCategoryHtml") %></div>
                <div><%# LocalizeTime(Eval("IssueStatusEffective")) %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Summary">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("IssueIdentifier", "/ui/admin/workflow/cases/outline?case={0}") %>'
                    Text='<%# Eval("IssueTitle") %>' />
                <div class="fs-sm text-body-secondary">
                    <i class="far fa-paperclip me-2"></i><%# GetTotalAttachmentCount() %> Attachments
                </div>
                <div class="fs-sm text-body-secondary">
                    <i class="far fa-comment me-2"></i><%# Eval("CommentCount") %> Comments
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Administrator">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("AdministratorUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("AdministratorUserName") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Topic (Member/Account)">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("TopicUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("TopicUserName") %>' />
                <div class="form-text">
                    <a href="mailto:<%# Eval("TopicUserEmail") %>">
                        <%# Eval("TopicUserEmail") %>
                    </a>
                </div>
                <div class="form-text"><%# Eval("TopicAccountStatus") %></div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Department">
            <ItemTemplate>
                <%# GetTopicDepartments() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer">
            <ItemTemplate>
                <%# Eval("IssueEmployerGroupName") %>
                <div class="form-text">
                    <%# Eval("IssueEmployerGroupParentGroupName") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Current Employer">
            <ItemTemplate>
                <%# Eval("TopicEmployerGroupName") %>
                <div class="form-text">
                    <%# Eval("TopicGroupNames") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Owner">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("OwnerUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("OwnerUserName") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Reported" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("IssueReported")) %>
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

        <asp:TemplateField HeaderText="Commented" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Literal runat="server" ID="LastCommentDate" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organization(s)">
            <ItemTemplate>
                <%# Eval("TopicGroupNames") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Account Status">
            <ItemTemplate>
                <%# Eval("TopicAccountStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div runat="server" id="ButtonPanel" class="row mt-4 mb-5">
    <div class="col-lg-12">
        <div id="BulkButtons" runat="server">
            <insite:Button runat="server" ID="AssignButtonStart" ButtonStyle="Default" Text="Assign Cases" Icon="fas fa-stamp" />
            <insite:Button runat="server" ID="BulkCloseCasesButton" ButtonStyle="Default" Text="Bulk Close Cases" Icon="fas fa-folder" />
            <insite:Button runat="server" ID="StartBulkCaseStatusButton" ButtonStyle="Default" Text="Bulk Case Status" Icon="fas fa-folder" />
        </div>

        <div id="AssignPanel" runat="server" class="d-none">
            <div class="hstack">
                <div class="hstack w-25 me-2">
                    <insite:FindPerson runat="server" ID="NewOwnerID" EmptyMessage="Select a New Owner" CssClass="me-1" />
                    <insite:RequiredValidator runat="server" ControlToValidate="NewOwnerID" ValidationGroup="Assign" />
                </div>

                <div>
                    <insite:Button runat="server" ID="AssignButton" ButtonStyle="Success" Text="Assign Selected Cases" Icon="fas fa-stamp" ValidationGroup="Assign" DisableAfterClick="true" />
                    <insite:Button runat="server" ID="AssignButtonStop" ButtonStyle="Danger" Text="Stop Assigning Cases" Icon="fas fa-stop" />
                </div>
            </div>
        </div>

        <div id="BulkUpdateCaseStatusPanel" runat="server" class="row d-none">
            <div class="col-4">

                <div class="alert alert-danger" role="alert">
                    <i class="fas fa-stop-circle pe-2"></i><strong>Confirm:</strong>
                    Are you sure you want to bulk-update these cases?
                </div>

                <div class="card shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">Case Status</label>
                            <div>
                                <insite:IssueStatusComboBox runat="server" ID="BulkUpdateCaseStatus" AllowBlank="false" />
                            </div>
                        </div>

                        <insite:SaveButton runat="server" ID="SaveBulkCaseStatusButton" DisableAfterClick="true" />
                        <insite:CancelButton runat="server" ID="CancelBulkCaseStatusButton" />

                    </div>
                </div>

            </div>
        </div>

        <div id="BulkCloseCasePanel" runat="server" class="row d-none">
            <div class="col-4">

                <div class="alert alert-danger" role="alert">
                    <i class="fas fa-stop-circle pe-2"></i><strong>Confirm:</strong>
                    Are you sure you want to bulk-update these cases?
                </div>

                <div class="card shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">Case Status</label>
                            <div>
                                <insite:IssueStatusComboBox runat="server" ID="IssueStatus" AllowBlank="false" />
                            </div>
                        </div>

                        <insite:SaveButton runat="server" ID="SaveBulkButton" DisableAfterClick="true" />
                        <insite:CancelButton runat="server" ID="CancelBulkButton" />

                    </div>
                </div>

            </div>
        </div>

        <insite:Alert runat="server" ID="BulkUpdateStatusInfo" CssClass="mt-4"/>

    </div>
</div>

<asp:HiddenField ID="BulkMode" runat="server" />
<asp:HiddenField ID="BulkHasSelectionOnOtherPages" runat="server" />

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .assign-checkbox.hide {
            display:none;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            const bulkButtons = document.getElementById("<%= BulkButtons.ClientID %>");
            const bulkModeField = document.getElementById("<%= BulkMode.ClientID %>");

            if (bulkModeField.value) {
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.remove("hide"));

                switch (bulkModeField.value) {
                    case "CaseStatus":
                        showCaseStatus();
                        break;
                    case "CloseCase":
                        showCloseCase();
                        break;
                    case "Assign":
                        showAssign();
                        break;
                }

                document.querySelector("#<%= Grid.ClientID %> tr th").scrollIntoView();

                showOrHideSaveButton();
            }

            document.querySelectorAll("td.assign-checkbox > input").forEach(el => el.addEventListener("change", showOrHideSaveButton));

            document.getElementById("<%= StartBulkCaseStatusButton.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                showCaseStatus();
            });

            document.getElementById("<%= CancelBulkCaseStatusButton.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                hideCaseStatus();
            });

            document.getElementById("<%= BulkCloseCasesButton.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                showCloseCase();
            });

            document.getElementById("<%= CancelBulkButton.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                hideCloseCase();
            });

            document.getElementById("<%= AssignButtonStart.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                showAssign();
            });

            document.getElementById("<%= AssignButtonStop.ClientID %>")?.addEventListener("click", e => {
                e.preventDefault();
                hideAssign();
            });

            document.querySelector("th.assign-checkbox > input").addEventListener("click", e => {
                const isChecked = e.target.checked;
                document.querySelectorAll("td.assign-checkbox > input").forEach(e => e.checked = isChecked);
                showOrHideSaveButton();
            });

            function showOrHideSaveButton() {
                const hasChecked = document.getElementById("<%=BulkHasSelectionOnOtherPages.ClientID %>").value === "true"
                    || !!document.querySelector("td.assign-checkbox > input:checked");

                if (hasChecked) {
                    document.getElementById("<%= SaveBulkCaseStatusButton.ClientID %>")?.classList?.remove("disabled");
                    document.getElementById("<%= SaveBulkButton.ClientID %>")?.classList?.remove("disabled");
                    document.getElementById("<%= AssignButton.ClientID %>")?.classList?.remove("disabled");
                } else {
                    document.getElementById("<%= SaveBulkCaseStatusButton.ClientID %>")?.classList?.add("disabled");
                    document.getElementById("<%= SaveBulkButton.ClientID %>")?.classList?.add("disabled");
                    document.getElementById("<%= AssignButton.ClientID %>")?.classList?.add("disabled");
                }
            }

            function showCaseStatus() {
                showOrHideSaveButton();

                document.getElementById("<%= BulkUpdateCaseStatusPanel.ClientID %>").classList.remove("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.remove("hide"));
                bulkButtons.classList.add("d-none");

                document.getElementById("<%= CancelBulkCaseStatusButton.ClientID %>").scrollIntoView();

                bulkModeField.value = "CaseStatus";
            }

            function hideCaseStatus() {
                document.getElementById("<%= BulkUpdateCaseStatusPanel.ClientID %>").classList.add("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.add("hide"));
                bulkButtons.classList.remove("d-none");

                bulkModeField.value = "";
            }

            function showCloseCase() {
                showOrHideSaveButton();

                document.getElementById("<%= BulkCloseCasePanel.ClientID %>").classList.remove("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.remove("hide"));
                bulkButtons.classList.add("d-none");

                document.getElementById("<%= CancelBulkButton.ClientID %>").scrollIntoView();

                bulkModeField.value = "CloseCase";
            }

            function hideCloseCase() {
                document.getElementById("<%= BulkCloseCasePanel.ClientID %>").classList.add("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.add("hide"));
                bulkButtons.classList.remove("d-none");

                bulkModeField.value = "";
            }

            function showAssign() {
                showOrHideSaveButton();

                document.getElementById("<%= AssignPanel.ClientID %>").classList.remove("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.remove("hide"));
                bulkButtons.classList.add("d-none");

                document.getElementById("<%= AssignButtonStop.ClientID %>").scrollIntoView();

                bulkModeField.value = "Assign";
            }

            function hideAssign() {
                document.getElementById("<%= AssignPanel.ClientID %>").classList.add("d-none");
                document.querySelectorAll(".assign-checkbox").forEach(e => e.classList.add("hide"));
                bulkButtons.classList.remove("d-none");

                bulkModeField.value = "";
            }
        });

    </script>
</insite:PageFooterContent>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Issues.Controls.SearchResults" %>

<script type="text/javascript">
    function saveScrollPosition() {
        var scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
        $('#<%= hfScrollPosition.ClientID %>').val(scrollPosition);
    }

    function restoreScrollPosition() {
        var scrollPosition = $('#<%= hfScrollPosition.ClientID %>').val();
        if (scrollPosition && scrollPosition !== "0") {
            window.scrollTo(0, scrollPosition);
        }
    }

    $(document).ready(function () {
        restoreScrollPosition();
    });

    function enableBulkSaveButton() {
        var count = $('.checkbox-column input:checked').length;

        var saveBulkButton = $('#<%= SaveBulkButton.ClientID %>');
        if (count) {
            saveBulkButton.removeClass('disabled');
        } else {
            saveBulkButton.addClass('disabled');
        }
    }
</script>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="IssueIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="false" Visible="false" HeaderStyle-CssClass="checkbox-column" ItemStyle-CssClass="checkbox-column">
            <HeaderTemplate>
                <insite:CheckBox ID="SelectAllCases" runat="server" OnCheckedChanged="SelectAllCases_CheckedChanged" AutoPostBack="true" />
            </HeaderTemplate>
            <ItemTemplate>
                <insite:CheckBox ID="SelectCase" runat="server" OnCheckedChanged="SelectCase_CheckedChanged" CssClass="select-case-checkbox" AutoPostBack="true" OnClientChange="saveScrollPosition(); return true;" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="assign-checkbox hide" ItemStyle-CssClass="assign-checkbox hide">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="AssignCheckBox" onclick="" />
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
        <insite:Button runat="server" ID="AssignButtonStart" ButtonStyle="Default" Text="Assign Cases" Icon="fas fa-stamp" />
        <insite:Button runat="server" ID="BulkCloseCasesButton" ButtonStyle="Default" Text="Bulk Close Cases" Icon="fas fa-folder" />

        <div id="AssignPanel" class="d-none">
            <div class="hstack">
                <div class="hstack w-25 me-2">
                    <insite:FindPerson runat="server" ID="NewOwnerID" EmptyMessage="Select a New Owner" CssClass="me-1" />
                    <insite:RequiredValidator runat="server" ControlToValidate="NewOwnerID" ValidationGroup="Assign" />
                </div>

                <div>
                    <insite:Button runat="server" ID="AssignButton" ButtonStyle="Success" Text="Assign Selected Cases" Icon="fas fa-stamp" ValidationGroup="Assign" />
                    <insite:Button runat="server" ID="AssignButtonStop" ButtonStyle="Danger" Text="Stop Assigning Cases" Icon="fas fa-stop" />
                </div>
            </div>
        </div>


        <div class="row mt-3">
            <div class="col-4">

                <insite:Alert runat="server" ID="BulkUpdateStatus" />
                <insite:Alert runat="server" ID="BulkUpdateStatusInfo"/>

                <div id="BulkUpdatePanel" class="mt-3" runat="server" visible="false">
                    <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                        <i class="fas fa-stop-circle pe-2"></i><strong>Confirm:</strong>
                        Are you sure you want to bulk-update these cases?
                    </div>

                    <div class="card shadow-lg">
                        <div class="card-body">

                            <div class="row mb-3">
                                <div runat="server" id="NoCaseStatus">
                                    <p>No Case Status to select</p>
                                </div>
                                <div runat="server" id="OneCaseStatus">
                                    <p><span class="fw-bold">Set Case status to: </span>
                                        <asp:Literal ID="OneCaseStatusLiteral" runat="server"></asp:Literal></p>
                                </div>
                                <div runat="server" id="ManyCaseStatus">
                                    <insite:IssueStatusComboBox runat="server" ID="IssueStatus" EmptyMessage="Select a Case Status" AllowBlank="false" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12">
                                    <div class="mb-3">
                                        <insite:SaveButton runat="server" ID="SaveBulkButton"/>
                                        <insite:CancelButton runat="server" ID="CancelBulkButton" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

            </div>
        </div>

    </div>
</div>

<asp:HiddenField ID="hfShowCheckboxColumn" runat="server" Value="false" />
<asp:HiddenField ID="hfScrollPosition" runat="server" />

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

            $('#<%= AssignButtonStart.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $('#<%= BulkCloseCasesButton.ClientID %>').hide();

                $("#AssignPanel").removeClass("d-none");

                enableAssignButton();

                $('.assign-checkbox').removeClass('hide');
            });

            $('#<%= AssignButtonStop.ClientID %>').click(function (e) {
                e.preventDefault();

                $("#AssignPanel").addClass("d-none");

                $('#<%= AssignButtonStart.ClientID %>').show();
                $('#<%= BulkCloseCasesButton.ClientID %>').show();

                $('.assign-checkbox').addClass('hide');
            });

            $('#<%= AssignButton.ClientID %>').click(function (e) {
                if (typeof $(this).attr('disabled') !== 'undefined' && $(this).attr('disabled') !== false) {
                    e.preventDefault();
                }
            });

            $('.assign-checkbox input').change(enableAssignButton);

            function enableAssignButton() {
                var count = $('.assign-checkbox input:checked').length;

                if (count) {
                    $('#<%= AssignButton.ClientID %>').removeClass('disabled');
                } else {
                    $('#<%= AssignButton.ClientID %>').addClass('disabled');
                }
            }

            $('.checkbox-column input').change(enableBulkSaveButton);

            function enableBulkSaveButton() {
                var count = $('.checkbox-column input:checked').length;

                if (count) {
                    $('#<%= SaveBulkButton.ClientID %>').removeClass('disabled');
                } else {
                    $('#<%= SaveBulkButton.ClientID %>').addClass('disabled');
                }
            }

            $('#<%= BulkCloseCasesButton.ClientID %>').click(function (e) {
                e.preventDefault();

                enableBulkSaveButton();

                $('#<%= hfShowCheckboxColumn.ClientID %>').val("true");

                __doPostBack('<%= Grid.ClientID %>', '');
            });

            $('#<%= CancelBulkButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $('#<%= hfShowCheckboxColumn.ClientID %>').val("false");

                __doPostBack('<%= Grid.ClientID %>', '');
            });


        });

    </script>
</insite:PageFooterContent>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.SearchResults" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        a.disabled {
            color: gray !important;
        }

        a.editable {
            text-decoration: none !important;
        }

        .inline-email-input {
            width: 250px !important;
        }

        .merge-checkbox.hide {
            display:none;
        }

    </style>
</insite:PageHeadContent>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="merge-checkbox hide" ItemStyle-CssClass="merge-checkbox hide">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="MergeCheckBox" onclick="" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("FullName") %>' ToolTip="Open Person Edit page" />
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Job Title">
            <ItemTemplate>
                <%# Eval("JobTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <asp:HyperLink runat="server" ID="EmailLink" />
                <asp:Literal runat="server" ID="EmailText" />
                <div class="form-text"><%# Eval("EmailAlternate") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Shipping Address">
            <ItemTemplate>
                <asp:Literal ID="ShippingAddressLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Home Address">
            <ItemTemplate>
                <asp:Literal ID="HomeAddressLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Phone">
            <ItemTemplate>
                <asp:Literal ID="PhoneLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Birthdate">
            <ItemTemplate>
                <%# GetDateString((DateTime?)Eval("Birthdate")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roles" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("PermissionLists") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Membership Status" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("StatusText") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User Status" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <asp:Literal runat="server" ID="StatusLiteral" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Authenticated" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Localize(Eval("LastAuthenticated")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Employer">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("EmployerGroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("EmployerGroupName") %>' ToolTip="Open Employer Edit page" />
                <%# Eval("EmployerGroupCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Parent">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("EmployerDistrictIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("EmployerDistrictName") %>'  ToolTip="Open Employer District Edit page" />
                <span class="form-text"><%# Eval("EmployerDistrictAccountNumber") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# Localize(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Person Code">
            <ItemTemplate>
                <%# Eval("PersonCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <insite:TemplateField FieldName="IssueStatus" HeaderText="Case Status">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="IssueRepeater">
                    <HeaderTemplate>
                        <ul style="margin-bottom:0;">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <%# Container.DataItem %>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </insite:TemplateField>

        <insite:TemplateField FieldName="Documents" HeaderText="Documents" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a runat="server" id="DocumentsLink" href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}&panel=attachments") %>'>
                    <i class="far fa-paperclip" title="Has Documents"></i>
                </a>
            </ItemTemplate>
        </insite:TemplateField>

        <asp:BoundField HeaderText="Person's Region" DataField="Region" />

    </Columns>
</insite:Grid>

<div runat="server" id="ButtonPanel" class="row mt-4 mb-4">
    <div class="col-lg-12">
        <insite:Button runat="server" ID="StartBulkUpdateButton" ButtonStyle="Default" Text="Bulk Update" Icon="fas fa-pen-square" />
        <insite:Button runat="server" ID="StartMergingButton" ButtonStyle="Default" Text="Merge" Icon="fas fa-code-merge" />
        <insite:Button runat="server" ID="MergeButton" ButtonStyle="Success" style="display:none;" disabled="disabled" Text="Start Merge" Icon="fas fa-cloud-upload" />
        <insite:Button runat="server" ID="StopMergingButton" ButtonStyle="Danger" style="display:none;" Text="Stop Merge" Icon="fas fa-code-merge" />
    </div>
</div>

<insite:Alert runat="server" ID="BulkUpdateStatus" />

<insite:ProgressPanel runat="server" ID="BulkProgress" HeaderText="Bulk Update" Cancel="Custom">
    <Triggers>
        <insite:ProgressControlTrigger ControlID="SaveBulkButton" />
    </Triggers>
    <Items>
        <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
        <insite:ProgressStatus Text="Status: {status}{running_ellipsis}" />
        <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
    </Items>
</insite:ProgressPanel>

<div runat="server" id="BulkUpdatePanel" style="display: none;" class="mt-3">
    <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
        <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
        Are you sure you want to bulk-update these users?
    </div>

    <div class="card shadow-lg">
        <div class="card-body">

            <div class="row">
                <div class="col-6 mb-3">
                    <h4>Options</h4>
                    <insite:RadioButtonList runat="server" ID="BulkAction">
                        <asp:ListItem Text="Grant access and send welcome email with temporary password" Value="GrantAndWelcome" />
                        <asp:ListItem Text="Grant access but do not send welcome email" Value="JustGrant" />
                        <asp:ListItem Text="Revoke access" Value="Revoke" />
                    </insite:RadioButtonList>
                </div>
            </div>

            <div class="row">
                <div class="col-6">
                    <div class="mb-3">
                        <insite:SaveButton runat="server" ID="SaveBulkButton" />
                        <insite:CancelButton runat="server" ID="CancelBulkButton" />
                    </div>
                </div>
            </div>

        </div>
    </div>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            $('#<%= StartBulkUpdateButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $('#<%= BulkUpdatePanel.ClientID %>').show();
                $('html, body').animate({ scrollTop: $(document).height() }, 1000);
            });

            $('#<%= CancelBulkButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $('#<%= BulkUpdatePanel.ClientID %>').hide();
                $('html, body').animate({ scrollTop: 0 }, 1000);
            });

            $('#<%= StartMergingButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $('#<%= StopMergingButton.ClientID %>').show();
                $('#<%= MergeButton.ClientID %>').show();

                $('.merge-checkbox').removeClass('hide');
            });

            $('#<%= StopMergingButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $('#<%= StartMergingButton.ClientID %>').show();
                $('#<%= MergeButton.ClientID %>').hide();

                $('.merge-checkbox').addClass('hide');
            });

            $('#<%= MergeButton.ClientID %>').click(function (e) {
                if (typeof $(this).attr('disabled') !== 'undefined' && $(this).attr('disabled') !== false)
                    e.preventDefault();
            });

            $('.merge-checkbox input').change(function () {
                var count = $('.merge-checkbox input:checked').length;

                if (this.checked) {
                    if (count > 2) {
                        alert('It is possible to select only two person for combining');
                        $(this).prop('checked', false);

                        count--;
                    }
                }

                if (count == 2)
                    $('#<%= MergeButton.ClientID %>').removeAttr('disabled');
                else
                    $('#<%= MergeButton.ClientID %>').attr('disabled', 'disabled');
            });
        });

    </script>
</insite:PageFooterContent>
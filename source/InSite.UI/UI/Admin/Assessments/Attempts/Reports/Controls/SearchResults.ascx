<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Attempts.Reports.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<div class="mb-3">
    <insite:Button runat="server" ID="ReportLink" ButtonStyle="Default" Text="Report" Icon="fas fa-chart-bar" NavigateUrl="/ui/admin/assessments/attempts/report" />
    <insite:Button runat="server" ID="TagLink" ButtonStyle="Default" Text="Tag" Icon="fas fa-tag" NavigateUrl="/ui/admin/assessments/attempts/tag" />
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="AttemptIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="assign-checkbox hide" ItemStyle-CssClass="assign-checkbox hide">
        <ItemTemplate>
            <asp:CheckBox runat="server" ID="AssignCheckBox" />
            <asp:HiddenField runat="server" ID="HasGradingAssessor" Value='<%# Eval("GradingAssessorUserIdentifier") != null ? "1" : "0" %>' />
        </ItemTemplate>
    </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="SubmitButton"
                    Name="flag-checkered"
                    CommandName="Submit"
                    ToolTip="Force submission of this attempt"
                    ConfirmText="Are you sure you want to force the submission of this attempt? It will move to a status of Completed, or of Pending if there are composed response questions to be graded."
                />
                <insite:IconLink runat="server" ID="ViewAttemptLink" Name="search"
                    NavigateUrl='<%# Eval("AttemptIdentifier", "/ui/admin/assessments/attempts/view?attempt={0}") %>' 
                />
            </ItemTemplate>
        </asp:TemplateField>
                        
        <asp:TemplateField HeaderText="Exam Form">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("Form.BankIdentifier") %>&form=<%# Eval("Form.FormIdentifier") %>"><%# Eval("Form.FormTitle") %></a>
                <div class="form-text">
                    <%# Eval("Form.FormName") %>
                    &bull;
                    Exam Form Asset #<%# GetFormAsset() %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Exam Candidate" ItemStyle-Wrap="False">
            <ItemTemplate>

                <%# GetCandidateInfo( (InSite.Application.Contacts.Read.VPerson)Eval("AssessorPerson"), (InSite.Application.Contacts.Read.VPerson)Eval("LearnerPerson")) %>

                <div class="form-text">
                    <%# Eval("AttemptTag") %>
                </div>
                <div><%# GetBrowserInfo() %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# FormatTime() %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Score" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" ItemStyle-Wrap="False" >
            <ItemTemplate>
                <%# FormatScore() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Grading Assessor" ItemStyle-Wrap="False">
            <ItemTemplate>

                <%# Eval("GradingAssessor.UserFullName") %>

            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div runat="server" id="ButtonPanel" class="row mt-4 mb-5">
    <div class="col-lg-12">
        <insite:Button runat="server" ID="AssignButtonStart" ButtonStyle="Default" Text="Assign Attempts" Icon="fas fa-stamp" />

        <div id="AssignPanel" class="d-none">
            <div class="hstack">
                <div class="hstack w-25 me-2">
                    <insite:FindPerson runat="server" ID="NewAssessorID" EmptyMessage="Select Grading Assessor" CssClass="me-1" />
                    <insite:RequiredValidator runat="server" ControlToValidate="NewAssessorID" ValidationGroup="Assign" />
                </div>

                <div>
                    <insite:Button runat="server" ID="AssignButton" ButtonStyle="Success" Text="Assign Grading Assessor" Icon="fas fa-stamp" ValidationGroup="Assign" />
                    <insite:Button runat="server" ID="AssignButtonStop" ButtonStyle="Danger" Text="Cancel" Icon="fas fa-stop" />
                </div>
            </div>
        </div>

        <insite:Button runat="server" ID="UnassignButtonStart" ButtonStyle="Default" Text="Unassign Attempts" Icon="fas fa-eraser" />

        <div id="UnassignPanel" class="d-none">
            <div>
                <insite:Button runat="server" ID="UnassignButton" ButtonStyle="Success" Text="Unassign Grading Assessor" Icon="fas fa-eraser" />
                <insite:Button runat="server" ID="UnassignButtonStop" ButtonStyle="Danger" Text="Cancel" Icon="fas fa-stop ps-1" />
            </div>
        </div>
    </div>
</div>

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
                $('#<%= UnassignButtonStart.ClientID %>').hide();

                $("#AssignPanel").removeClass("d-none");

                enableAssignButton();

                $('.assign-checkbox').removeClass('hide');
            });

            $('#<%= AssignButtonStop.ClientID %>').click(function (e) {
                e.preventDefault();

                $("#AssignPanel").addClass("d-none");

                $('#<%= AssignButtonStart.ClientID %>').show();
                $('#<%= UnassignButtonStart.ClientID %>').show();

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

            $('#<%= UnassignButtonStart.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $("#UnassignPanel").removeClass("d-none");
                $('#<%= AssignButtonStart.ClientID %>').hide();
                $('.assign-checkbox').removeClass('hide');

                $('.assign-checkbox').each(function () {
                    var row = $(this).closest('tr');
                    var hasAssessor = row.find('input[type=hidden][id$=HasGradingAssessor]').val() === "1";
                    var checkbox = $(this).find('input[type=checkbox]');

                    if (hasAssessor) {
                        checkbox.prop('disabled', false);
                    } else {
                        checkbox.prop('disabled', true);
                    }
                });
            });

            $('#<%= UnassignButtonStop.ClientID %>').click(function (e) {
                e.preventDefault();

                $("#UnassignPanel").addClass("d-none");
                $('#<%= UnassignButtonStart.ClientID %>').show();
                $('#<%= AssignButtonStart.ClientID %>').show();

                $('.assign-checkbox').each(function () {
                    var checkbox = $(this).find('input[type=checkbox]');
                    checkbox.prop('checked', false);
                    checkbox.prop('disabled', false);
                });

                $('.assign-checkbox').addClass('hide');
            });

            $('#<%= UnassignButton.ClientID %>').click(function (e) {

            });
        });

    </script>
</insite:PageFooterContent>
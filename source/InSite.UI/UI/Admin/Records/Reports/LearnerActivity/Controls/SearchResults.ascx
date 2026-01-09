<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Reports.LearnerActivity.Controls.SearchResults" %>

<insite:PageHeadContent runat="server">
<style type="text/css">
    table { caption-side: top; }
    table.table-bordered caption { font-weight: bold; }
    table.table-bordered td.number { width: 60px; text-align: right; }
    table.table-striped>tbody>tr:nth-of-type(even) { background-color: #ffffff; }
    .radio-count-strategy label + input[type="radio"] { margin-left: 1.5rem; }
</style>
</insite:PageHeadContent>

<div class="d-inline-block">
    <asp:Literal ID="Instructions" runat="server" />

    <insite:Grid runat="server" ID="Grid" DataKeyNames="LearnerIdentifier" CssClass="table table-bordered table-striped table-responsive">
        <Columns>

            <asp:TemplateField HeaderText="Account Created" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("LearnerCreated")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Full Name">
                <ItemTemplate>
                    <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("LearnerIdentifier") %>"><%# Eval("LearnerName") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Last Name" DataField="LearnerNameLast" />
            <asp:BoundField HeaderText="First Name" DataField="LearnerNameFirst" />
            <asp:BoundField HeaderText="Email" DataField="LearnerEmail" />
            <asp:BoundField HeaderText="Gender" DataField="LearnerGender" />

            <asp:BoundField HeaderText="Phone" DataField="LearnerPhone" ItemStyle-Wrap="false" />
            <asp:TemplateField HeaderText="Birthdate" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# FormatDate(Eval("LearnerBirthdate")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Person Code" DataField="PersonCode" />
            <asp:BoundField HeaderText="Occupation" DataField="LearnerOccupation" />

            <asp:TemplateField HeaderText="Program">
                <ItemTemplate>
                    <%# GetProgramsHtml() %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Gradebook" DataField="GradebookName" />
            <asp:BoundField HeaderText="Enrollment Status" DataField="EnrollmentStatus" />

            <asp:TemplateField HeaderText="Enrollment Started" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("EnrollmentStarted")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="First Session" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("SessionStartedFirst")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Session" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("SessionStartedLast")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Session Count" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# Eval("SessionCount","{0:n0}") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Session Time" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# Eval("SessionMinutes","{0:n0} min") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Employed By / Belongs To" DataField="EmployerName" />

            <asp:BoundField HeaderText="Citizenship" DataField="LearnerCitizenship" />
            <asp:BoundField HeaderText="Membership Status" DataField="MembershipStatus" />

            <asp:TemplateField HeaderText="Achievement Granted" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# Eval("AchievementGranted", "{0:MMM d, yyyy}") %>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>

</div>

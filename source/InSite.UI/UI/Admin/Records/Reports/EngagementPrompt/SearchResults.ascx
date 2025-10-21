<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Records.Reports.EngagementPrompt.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="LearnerIdentifier,ProgramIdentifier" CssClass="table table-bordered table-striped table-responsive">
    <Columns>

        <asp:BoundField HeaderText="Last Name" DataField="LearnerNameLast" />
        <asp:BoundField HeaderText="First Name" DataField="LearnerNameFirst" />
        <asp:BoundField HeaderText="Email" DataField="LearnerEmail" />

        <asp:BoundField HeaderText="Program" DataField="ProgramName" />
        <asp:BoundField HeaderText="Gradebook" DataField="GradebookName" />
        <asp:BoundField HeaderText="Enrollment Status" DataField="EnrollmentStatus" ItemStyle-Wrap="false" />

        <asp:TemplateField HeaderText="Enrollment Started" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("EnrollmentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Session" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("SessionStartedLast")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
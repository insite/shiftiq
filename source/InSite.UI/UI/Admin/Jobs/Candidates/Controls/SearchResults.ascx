<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Jobs.Candidates.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
            <itemtemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/jobs/candidates/edit?contact={0}") %>' />
            </itemtemplate>
        </asp:TemplateField>
           
        <asp:TemplateField HeaderText="Date Registered">
            <itemtemplate>
                <asp:Literal ID="DateRegistered" runat="server" Text='<%# LocalizeTime(Eval("Created"), "div") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date Last Active" ItemStyle-CssClass="datetime-column">
            <itemtemplate>
                <asp:Literal ID="DateLastActive" runat="server" Text='<%# LocalizeTime(Eval("Modified"), "div") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name" ItemStyle-Wrap="False">
            <itemtemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/jobs/candidates/edit?contact={0}") %>'
                    Text='<%# string.Format("{0} {1}", Eval("FirstName"), Eval("LastName")) %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email" ItemStyle-Wrap="False">
            <itemtemplate>
                <insite:IconLink runat="server" ID="EmailStatus" Name="paper-plane" ToolTip="Send Email" CssClass="me-1" NavigateUrl='<%# CreateMailToLink(Eval("Email")) %>' /><asp:Literal ID="Email" runat="server" Text='<%# Eval("Email") %>'></asp:Literal>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="City">
            <itemtemplate>
                <asp:Literal ID="City" runat="server" Text='<%# Eval("City") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Approved">
            <itemtemplate>
                <asp:Literal ID="Approved" runat="server" Text='<%# Eval("Approved") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completion" ItemStyle-Width="100px">
            <itemtemplate>
                <%# GetCompletionProfileHtml((Guid?)Eval("UserIdentifier")) %>
                <%# GetCompletionResumeHtml((Guid?)Eval("UserIdentifier")) %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Occupation Interest">
            <itemtemplate>
                <%# Eval("OccupationInterest") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Occupation Profile">
            <itemtemplate>
                <asp:Literal ID="CandidateOccupations" runat="server" Text='<%# Eval("OccupationList") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Seeking Work">
            <itemtemplate>
                <asp:Literal ID="SeekingWork" runat="server" Text='<%# Eval("IsActivelySeeking") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Uploads">
            <itemtemplate>
                <asp:Literal ID="Uploads" runat="server" Text='<%# string.Join("<br/>", GetCandidateUploads(Eval("UserIdentifier"))) %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Comments">
            <itemtemplate>
                <%# string.Join(";<br/>", GetCommentsAboutCandidate(Eval("UserIdentifier"))) %>
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Jobs.Applications.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="OpportunityIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit" 
                    NavigateUrl='<%# Eval("ApplicationIdentifier", "/ui/admin/jobs/applications/edit?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Employer Name">
            <ItemTemplate>
                <asp:Literal ID="EmployerName" runat="server" Text='<%# Eval("EmployerName") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Job Position">
            <ItemTemplate>
                <asp:Literal ID="JobPosition" runat="server" Text='<%# Eval("JobPosition") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Candidate">
            <ItemTemplate>
                <%# Eval("CandidateFirstName") %> <%# Eval("CandidateLastName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Cover Letter">
            <ItemTemplate>
                <asp:Literal ID="CandidateLetter" runat="server" Text='<%# Eval("CandidateLetter") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Resume">
            <ItemTemplate>
                <asp:Literal ID="CandidateResume" runat="server" Text='<%# Eval("CandidateResume") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Updated" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Literal ID="Updated" runat="server" Text='<%# LocalizeDate(Eval("Updated")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

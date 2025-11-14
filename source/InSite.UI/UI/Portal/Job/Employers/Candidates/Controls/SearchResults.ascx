<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Candidates.Controls.SearchResults" %>

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
        <Columns>
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Text='<%# Eval("FullName") %>' ToolTip="View Person"
                        NavigateUrl='<%# Eval("UserIdentifier", "/ui/portal/job/employers/candidates/view?id={0}") %>' />
                    <div class="form-text">
                        <%# Eval("CurrentCity") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Credentials">
                <ItemTemplate>
                    <%# Eval("Qualifications") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Job Title">
                <ItemTemplate>
                    <%# Eval("JobTitle") %>
                    <div>
                        <%# Eval("Occupations") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Authenticated">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("LastAuthenticated")) %>
                    <div class="form-text">
                        <%# GetCandidateConnection(Eval("UserIdentifier")) %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <ItemTemplate>
                        <insite:Button runat="server" ID="RequestContactLink" Text="" ButtonStyle="Success" Icon="far fa-fw fa-envelope" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>

<insite:Modal runat="server" ID="RequestContactWindow" Title="Request Candidate Contact" Width="550px" MinHeight="300px" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        var candidateRequestContact = {
            showRequestContact: function (contactId) {
                var wnd = modalManager.load('<%= RequestContactWindow.ClientID %>', '/ui/portal/job/employers/candidates/request-contact?candidate=' + String(contactId));
                $(wnd).one('closed.modal.insite', candidateRequestContact.onWindowClose);
            }
        };

    </script>
</insite:PageFooterContent>

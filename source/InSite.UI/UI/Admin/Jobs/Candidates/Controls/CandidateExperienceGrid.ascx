<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CandidateExperienceGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.CandidateExperienceGrid" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div>
            <insite:Grid runat="server" ID="Grid" DataKeyNames="ExperienceIdentifier">
                <Columns>

                    <asp:TemplateField HeaderText="Date From">
                        <itemtemplate>
                            <%# GetDateString((DateTime?)Eval("ExperienceDateFrom")) %>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date To">
                        <itemtemplate>
                            <%# GetDateString((DateTime?)Eval("ExperienceDateTo")) %>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Employer" DataField="EmployerName"></asp:BoundField>
                    <asp:BoundField HeaderText="Job Title" DataField="ExperienceJobTitle"></asp:BoundField>
                    <asp:BoundField HeaderText="Country" DataField="ExperienceCountry"></asp:BoundField>
                    <asp:BoundField HeaderText="City" DataField="ExperienceCity"></asp:BoundField>

                    <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="False">
                        <itemtemplate>
                            <insite:IconButton runat="server" Visible="true" Name="pencil" OnClientClick='<%# GetModalUrl((Guid?)Eval("ExperienceIdentifier")) %>' ToolTip="Edit"/>
                            <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this experience?" />
                        </itemtemplate>
                    </asp:TemplateField>

                </Columns>
            </insite:Grid>

            <asp:Button runat="server" ID="RefreshGridButton" CssClass="d-none" />
        </div>

        <div class="mt-3">
            <insite:Button runat="server" ID="AddNewLink" Text="Add New" Visible="false" ButtonStyle="Default" Icon="fas fa-plus-circle" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:Modal runat="server" ID="CreatorWindow" Title="Add New Experience" Width="550px" MinHeight="300px" />
<insite:Modal runat="server" ID="EditorWindow" Title="Edit Experience" Width="550px" MinHeight="300px" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        var candidateExperienceGrid = {
            showCreator: function (contactId) {
                var wnd = modalManager.load('<%= CreatorWindow.ClientID %>', '/ui/admin/jobs/candidates/add-experience?candidate=' + String(contactId));
                $(wnd).one('closed.modal.insite', candidateExperienceGrid.onWindowClose);
            },
            showEditor: function (candidateExperienceIdentifier) {
                var wnd = modalManager.load('<%= EditorWindow.ClientID %>', '/ui/admin/jobs/candidates/edit-experience?experience=' + String(candidateExperienceIdentifier));
                $(wnd).one('closed.modal.insite', candidateExperienceGrid.onWindowClose);
            },
            onWindowClose: function (e, s, a) {
                if (a === 'refresh')
                    __doPostBack('<%= RefreshGridButton.UniqueID %>', '');
            }
        };

    </script>
</insite:PageFooterContent>

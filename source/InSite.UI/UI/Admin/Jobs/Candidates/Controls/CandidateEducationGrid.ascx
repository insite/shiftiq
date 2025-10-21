<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CandidateEducationGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.CandidateEducationGrid" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div>
            <insite:Grid runat="server" ID="Grid" DataKeyNames="EducationIdentifier">
                <Columns>

                    <asp:TemplateField HeaderText="Date From">
                        <ItemTemplate>
                            <%# GetDateString((DateTime?)Eval("EducationDateFrom")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date To">
                        <ItemTemplate>
                            <%# GetDateString((DateTime?)Eval("EducationDateTo")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Status" DataField="EducationStatus"></asp:BoundField>
                    <asp:BoundField HeaderText="Institution" DataField="EducationInstitution"></asp:BoundField>
                    <asp:BoundField HeaderText="Course" DataField="EducationName"></asp:BoundField>
                    <asp:BoundField HeaderText="Qualification" DataField="EducationQualification"></asp:BoundField>
                    <asp:BoundField HeaderText="Country" DataField="EducationCountry"></asp:BoundField>
                    <asp:BoundField HeaderText="City" DataField="EducationCity"></asp:BoundField>
                
                    <insite:TemplateField ItemStyle-Width="40px" FieldName="EditColumn" ItemStyle-Wrap="False">
                        <ItemTemplate>
                            <insite:IconButton runat="server" ID="EditCommand" Name="pencil" OnClientClick='<%# GetModalUrl((Guid?)Eval("EducationIdentifier")) %>' ToolTip="Edit" />
                            <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this education?" />
                        </ItemTemplate>
                    </insite:TemplateField>

                </Columns>
            </insite:Grid>

            <asp:Button runat="server" ID="RefreshGridButton" Style="display: none;" />
        </div>

        <div class="mt-3">
            <insite:Button runat="server" ID="AddNewLink" Text="Add New" Visible="false" ButtonStyle="Default" Icon="fas fa-plus-circle" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:Modal runat="server" ID="CreatorWindow" Title="Add New Education" Width="550px" MinHeight="300px" />
<insite:Modal runat="server" ID="EditorWindow" Title="Edit Education" Width="550px" MinHeight="300px" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        var candidateEducationGrid = {
            showCreator: function (contactKey) {
                var wnd = modalManager.load('<%= CreatorWindow.ClientID %>', '/ui/admin/jobs/candidates/add-education?candidate=' + String(contactKey));
                $(wnd).one('closed.modal.insite', candidateEducationGrid.onWindowClose);
            },
            showEditor: function (candidateEducationKey) {
                var wnd = modalManager.load('<%= EditorWindow.ClientID %>', '/ui/admin/jobs/candidates/edit-education?education=' + String(candidateEducationKey));
                $(wnd).one('closed.modal.insite', candidateEducationGrid.onWindowClose);
            },
            onWindowClose: function (e, s, a) {
                if (a === 'refresh')
                    __doPostBack('<%= RefreshGridButton.UniqueID %>', '');
            }
        };

    </script>
</insite:PageFooterContent>
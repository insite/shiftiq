<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Assessments.QuizAttempts.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="AttemptIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Attempt"
                    NavigateUrl='<%# GetViewUrl() %>' 
                />
                <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete Attempt"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to delete this attempt?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Quiz Type" DataField="QuizType" />
        <asp:BoundField HeaderText="Quiz Name" DataField="QuizName" />
        <asp:BoundField HeaderText="Learner Name" DataField="LearnerUser.UserFullName" />
        <asp:BoundField HeaderText="Learner Email" DataField="LearnerUser.UserEmail" />

        <asp:TemplateField HeaderText="Attempt Started">
            <ItemTemplate>
                <%# LocalizeTime(Eval("AttemptStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Attempt Completed">
            <ItemTemplate>
                <%# LocalizeTime(Eval("AttemptCompleted")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
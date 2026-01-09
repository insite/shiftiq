<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Submissions.Controls.SearchResults" %>

<asp:Panel runat="server" ID="MainPanel">

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="ResponseSessionIdentifier">
    <Columns>
            
        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Submission Session"
                    NavigateUrl='<%# GetScreenUrl("/ui/admin/workflow/forms/submissions/outline") %>' 
                    Visible='<%# !(bool)Eval("SurveyIsConfidential") %>' />
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit Submission Session" Target="_blank"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/portal/workflow/forms/submit/answer?session={0}") %>' 
                    Visible='<%# CanChangeResponse %>' />
                <insite:IconLink runat="server" Name="lock" ToolTip="Lock Submission Session"
                    NavigateUrl='<%# GetScreenUrl("/ui/admin/workflow/forms/submissions/lock") %>' 
                    Visible='<%# Eval("ResponseSessionCompleted") != null && !(bool)Eval("ResponseIsLocked") %>'/>
                <insite:IconLink runat="server" Name="lock-open" ToolTip="Unlock Submission Session"
                    NavigateUrl='<%# GetScreenUrl("/ui/admin/workflow/forms/submissions/unlock") %>' 
                    Visible='<%# Eval("ResponseSessionCompleted") != null && (bool)Eval("ResponseIsLocked") %>'/>
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete Submission Session"
                    NavigateUrl='<%# GetScreenUrl("/ui/admin/workflow/forms/submissions/delete-one") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Started" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalDate(Eval("ResponseSessionStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Completed" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalDate(Eval("ResponseSessionCompleted")) %>
                <%# Eval("ResponseIsLockedHtml") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Form">
            <ItemTemplate>
                <a href="/ui/admin/workflow/forms/outline?<%# Eval("SurveyFormIdentifier", "form={0}") %>">
                    <%# Eval("SurveyName") %>
                </a>
                <div class="form-text">
                    Form #<%# Eval("SurveyNumber") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Respondent" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("RespondentHtml") %>
                <%# Eval("FirstSelectionHtml") %>
                <%# Eval("FirstCommentHtml") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group">
            <ItemTemplate>
                <%# Eval("GroupName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Period">
            <ItemTemplate>
                <%# Eval("PeriodName") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

</asp:Panel>
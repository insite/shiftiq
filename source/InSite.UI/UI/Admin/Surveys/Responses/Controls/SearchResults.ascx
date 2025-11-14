<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Surveys.Responses.SearchResults" %>

<asp:Panel runat="server" ID="MainPanel">

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="ResponseSessionIdentifier">
    <Columns>
            
        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/outline?session={0}") %>' 
                    Visible='<%# !(bool)Eval("SurveyIsConfidential") %>' />
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit Response Session" Target="_blank"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/survey/respond/answer?session={0}") %>' 
                    Visible='<%# CanChangeResponse %>' />
                <insite:IconLink runat="server" Name="lock" ToolTip="Lock Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/lock?session={0}") %>' 
                    Visible='<%# !(bool)Eval("ResponseIsLocked") %>'/>
                <insite:IconLink runat="server" Name="lock-open" ToolTip="Unlock Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/unlock?session={0}") %>' 
                    Visible='<%# Eval("ResponseIsLocked") %>'/>
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/admin/surveys/responses/delete-session?session={0}") %>' />
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
            
        <asp:TemplateField HeaderText="Survey">
            <ItemTemplate>
                <a href="/ui/admin/surveys/forms/outline?<%# Eval("SurveyFormIdentifier", "survey={0}") %>">
                    <%# Eval("SurveyName") %>
                </a>
                <div class="form-text">
                    Survey Form #<%# Eval("SurveyNumber") %>
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
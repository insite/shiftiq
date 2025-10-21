<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportResponseSearchResults.ascx.cs" Inherits="InSite.UI.Admin.Surveys.Forms.Controls.ReportResponseSearchResults" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:BoundField HeaderText="Started" DataField="ResponseSessionStarted" DataFormatString="{0:MMM d, yyyy}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HtmlEncode="false" />
        <asp:BoundField HeaderText="Completed" DataField="ResponseSessionCompleted" DataFormatString="{0:MMM d, yyyy}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HtmlEncode="false" />

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

        <asp:TemplateField HeaderText="Respondent" HeaderStyle-Wrap="false" ItemStyle-Wrap="False">
            <itemtemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("RespondentUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("RespondentName") %>'
                    Visible='<%# !(bool)Eval("SurveyIsConfidential") && Eval("RespondentName") != null %>' />
                <div runat="server" class="form-text" visible='<%# !(bool)Eval("SurveyIsConfidential") && Eval("RespondentName") != null %>'>
                    <%# Eval("RespondentName") != null ? "<a href='mailto:" + Eval("RespondentEmail") + "'>" + Eval("RespondentEmail") + "</a>" : string.Empty %>
                </div>
                <asp:Literal runat="server" Text="*****" Visible='<%# (bool)Eval("SurveyIsConfidential") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false">
            <itemtemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/outline?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>' />
                <insite:IconLink runat="server" Name="lock" ToolTip="Lock Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/lock?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    Visible='<%# !(bool)Eval("ResponseIsLocked") %>' />
                <insite:IconLink runat="server" Name="lock-open" ToolTip="Unlock Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/unlock?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    Visible='<%# Eval("ResponseIsLocked") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete Response Session"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/admin/surveys/responses/delete-session?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>' />
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

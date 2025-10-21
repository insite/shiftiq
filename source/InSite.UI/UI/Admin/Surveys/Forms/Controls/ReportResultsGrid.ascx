<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportResultsGrid.ascx.cs" Inherits="InSite.Admin.Surveys.Forms.Controls.ReportResultsGrid" %>

<%@ Import Namespace="Shift.Common" %>

<div style="margin-bottom: 15px;">
    <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
    <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript"> 
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= FilterTextBox.ClientID %>')
                        .off('keydown', onKeyDown)
                        .on('keydown', onKeyDown);
                });

                function onKeyDown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        $('#<%= FilterButton.ClientID %>')[0].click();
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="ResponseSessionIdentifier">
    <Columns>

        <asp:BoundField HeaderText="Started" DataField="ResponseSessionStarted" DataFormatString="{0:MMM d, yyyy}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HtmlEncode="false" />
        <asp:BoundField HeaderText="Completed" DataField="ResponseSessionCompleted" DataFormatString="{0:MMM d, yyyy}" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HtmlEncode="false" />

        <asp:TemplateField HeaderText="Contact" HeaderStyle-Wrap="false" ItemStyle-Wrap="False">
            <itemtemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("Respondent.UserFullName") %>'
                    NavigateUrl='<%# Eval("RespondentUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Visible='<%# !(bool)Eval("SurveyForm.EnableUserConfidentiality") && Eval("Respondent") != null %>' />
                <div runat="server" class="form-text" visible='<%# !(bool)Eval("SurveyForm.EnableUserConfidentiality") && Eval("Respondent") != null %>'>
                    <%# Eval("Respondent") != null ? "<a href='mailto:" + Eval("Respondent.UserEmail") + "'>" + Eval("Respondent.UserEmail") + "</a>" : string.Empty %>
                </div>
                <asp:Literal runat="server" Text="*****" Visible='<%# (bool)Eval("SurveyForm.EnableUserConfidentiality") %>' />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false">
            <itemtemplate>
                <insite:IconLink runat="server" Name="search"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/outline?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    ToolTip="View Response Session" />
                <insite:IconLink runat="server" Name="lock"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/lock?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    ToolTip="Lock Response Session"
                    Visible='<%# !(bool)Eval("ResponseIsLocked") %>' />
                <insite:IconLink runat="server" Name="lock-open"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/ui/admin/surveys/responses/unlock?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    ToolTip="Unlock Response Session"
                    Visible='<%# Eval("ResponseIsLocked") %>' />
                <insite:IconLink runat="server" Name="trash-alt"
                    NavigateUrl='<%# Eval("ResponseSessionIdentifier", "/admin/surveys/responses/delete-session?session={0}") %>'
                    ReturnUrl='<%# "survey&panel=responses&grid-page=" + Grid.PageIndex %>'
                    ToolTip="Delete Response Session" />
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

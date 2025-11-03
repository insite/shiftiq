<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Surveys.Forms.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="chart-bar" ToolTip="Report"
                    NavigateUrl='<%# Eval("SurveyFormIdentifier", "/admin/surveys/forms/report?survey={0}") %>' />
                <insite:IconLink runat="server" Name="tasks" ToolTip="Responses"
                    NavigateUrl='<%# Eval("SurveyFormIdentifier", "/ui/admin/surveys/responses/search?survey={0}") %>' />
                <%# GetInvitationLink(Container.DataItem as InSite.Application.Surveys.Read.QSurveyForm) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="External Title / Internal Name">
            <ItemTemplate>
                <a href='/ui/admin/surveys/forms/outline?survey=<%# Eval("SurveyFormIdentifier") %>'>
                    <%# GetTitle((Guid)Eval("SurveyFormIdentifier")) %>
                    <i runat="server" visible='<%# Eval("SurveyFormLocked") != null %>' class="fas fa-lock" title="Locked" style="color:red;"></i>
                </a>
                <div class="form-text">
                    <%# Eval("SurveyFormName") %>
                </div>
                <div class="form-text text-warning">
                    <%# Eval("SurveyFormHook") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Status" DataField="SurveyFormStatus" />

        <asp:TemplateField HeaderText="Details">
            <ItemTemplate>
                <div>
                    <%# Eval("PageCount") %> Pages
                </div>
                <div>
                   <%# Eval("QuestionCount") %>  Questions
                </div>
                <div>
                    <%# Eval("BranchCount") %> Branches
                </div>
                <div>
                   <%# Eval("ConditionCount") %>  Conditions
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Modified">
            <ItemTemplate>
                <%# GetDataTimeHtml((DateTimeOffset?)Eval("LastChangeTime"), (Guid)Eval("LastChangeUser")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Response Summary" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetSummaryHtml((Guid)Eval("SurveyFormIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .table-summary {
            width: 100%;
        }

        .table-summary tr td:first-child {
            padding-right: 5px;
        }

        .table-summary tr td {
            padding: 2px;
        }

        .table-summary tr td:last-child {
            width: 100%;
        }
    </style>
</insite:PageHeadContent>
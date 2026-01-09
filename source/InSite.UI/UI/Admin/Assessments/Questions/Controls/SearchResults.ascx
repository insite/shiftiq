<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Text">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&question=<%# Eval("QuestionIdentifier") %>"><%# Shift.Common.Markdown.ToText((string)Eval("QuestionText")) %></a>
                <div class="form-text">
                    Question <%# (int)Eval("BankIndex") + 1 %> in Bank <%# Eval("BankName") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Publication Status">
            <ItemTemplate>
                <%# GetPublicationStatus() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Code" DataField="QuestionCode" />

        <asp:TemplateField HeaderText="Difficulty">
            <ItemTemplate>
                <%# GetQuestionDifficulty() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Reference" DataField="QuestionReference" />
        <asp:BoundField HeaderText="Tag" DataField="QuestionTag" />
        <asp:BoundField HeaderText="Taxonomy" DataField="QuestionTaxonomy" />
        <asp:BoundField HeaderText="Rubric" DataField="Rubric" />
      
        <asp:TemplateField HeaderText="QuestionType">
            <ItemTemplate>
                <%# GetQuestionTypeDescription() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Competency">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("QuestionCompetencyIdentifier","/ui/admin/standards/edit?id={0}") %>'
                    Text='<%# Eval("QuestionCompetencyTitle") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Question Flag">
            <ItemTemplate>
                <%# GetQuestionFlag() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Reporting Tags">
            <ItemTemplate>
                <%# GetReportingTags() %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
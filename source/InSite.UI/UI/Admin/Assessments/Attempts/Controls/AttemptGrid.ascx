<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttemptGrid.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.AttemptGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("LearnerUserIdentifier") %>">
                    <%# Eval("LearnerName") %>
                </a>
                <small class="form-text"><%# Eval("LearnerCode") %></small>
                <span runat="server" visible='<%# Eval("SebVersion") != null %>' class='badge bg-info'><%# Eval("SebVersion", "SEB v{0}") %></span>
            </ItemTemplate>
        </asp:TemplateField>
                
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href="mailto:<%# Eval("LearnerEmail") %>">
                    <%# Eval("LearnerEmail") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tag">
            <ItemTemplate>
                <%# Eval("AttemptTag") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completed">
            <ItemTemplate>
                <%# GetCompletedHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Score" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# GetScoreHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Grade">
            <ItemTemplate>
                <%# GetGradeHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <ItemTemplate>
                <a title='View Exam Attempt' href='/ui/admin/assessments/attempts/view?attempt=<%# Eval("AttemptIdentifier") %>'><i class='far fa-search'></i></a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
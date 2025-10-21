<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Type" DataField="QuizType" />

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href="<%# GetEditUrl() %>"><%# Eval("QuizName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Time Limit" DataField="TimeLimit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Attempt Limit" DataField="AttemptLimit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />

    </Columns>
</insite:Grid>

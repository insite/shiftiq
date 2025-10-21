<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Learning.Categories.Controls.SearchResults" %>

<div class="search-results">
    <insite:Grid runat="server" ID="Grid">
        <Columns>

            <asp:BoundField HeaderText="Category Folder" DataField="ItemFolder" />

            <asp:TemplateField HeaderText="Category Name" ItemStyle-Wrap="false"> 
                <ItemTemplate>
                    <a href='<%# GetEditUrl() %>'><%# Eval("ItemName") %></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Achievements" DataField="AchievementCount"
                HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

            <asp:BoundField HeaderText="Courses" DataField="CourseCount"
                HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

            <asp:BoundField HeaderText="Programs" DataField="ProgramCount"
                HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

        </Columns>
    </insite:Grid>
</div>
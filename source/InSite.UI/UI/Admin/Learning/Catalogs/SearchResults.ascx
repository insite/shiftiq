<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Learning.Catalogs.SearchResults" %>

<div class="search-results">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="CatalogIdentifier">
        <Columns>

            <asp:TemplateField HeaderText="Catalog Name"> 
                <ItemTemplate>
                    <a href='/ui/admin/learning/catalogs/edit?<%# Eval("CatalogIdentifier", "catalog={0}") %>'><%# Eval("CatalogName") %></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# GetStatusHtml(Container.DataItem) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Courses" DataField="CourseCount"
                HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

            <asp:BoundField HeaderText="Programs" DataField="ProgramCount"
                HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

        </Columns>
    </insite:Grid>
</div>
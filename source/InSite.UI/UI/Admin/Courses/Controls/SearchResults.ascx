<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Courses.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="CourseIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Course Name">
            <ItemTemplate>
                <a href='<%# Eval("CourseIdentifier", "/ui/admin/courses/manage?course={0}") %>'>
                    <%# Eval("CourseName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Code">
            <ItemTemplate>
                <%# Eval("CourseCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tag">
            <ItemTemplate>
                <%# Eval("CourseLabel") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Units" DataField="UnitCount" HeaderStyle-Wrap="false"
                        ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px" />

        <asp:BoundField HeaderText="Modules" DataField="ModuleCount" HeaderStyle-Wrap="false"
                        ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px" />

        <asp:BoundField HeaderText="Activities" DataField="ActivityCount" HeaderStyle-Wrap="false"
                        ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px" />

        <asp:TemplateField HeaderText="Catalog">
            <ItemTemplate>
                <%# Eval("CatalogName") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
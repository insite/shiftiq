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

        <asp:BoundField HeaderText="Publication Status" DataField="PublicationStatus" />

        <asp:TemplateField HeaderText="Publication Date">
            <ItemTemplate>
                <insite:Container runat="server" Visible='<%# Eval("PublicationDate") != null %>'>
                    <%# LocalizeTime(Eval("PublicationDate")) %>
                    <div class="text-muted fs-sm">by <%# Eval("PublicationAuthor") %></div>
                </insite:Container>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Course Enrollments" HeaderStyle-Width="185px">
            <ItemTemplate>
                <table class="table-enrollments"><tbody>
                    <tr>
                        <td><%# Eval("EnrollmentStarted") %></td>
                        <td>Started</td>
                    </tr>
                    <tr>
                        <td><%# Eval("EnrollmentCompleted") %></td>
                        <td>Completed</td>
                    </tr>
                </tbody></table>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Code" DataField="CourseCode" />
        <asp:BoundField HeaderText="Tag" DataField="CourseLabel" />

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

        <asp:TemplateField HeaderText="Gradebook">
            <ItemTemplate>
                <a href='<%# Eval("GradebookIdentifier", "/ui/admin/records/gradebooks/outline?id={0}") %>'>
                    <%# Eval("GradebookTitle") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        table.table-enrollments {
            width: 100%;
        }

            table.table-enrollments tr td:nth-child(1) {
                padding-right: 0.5rem;
                text-align: right;
            }

            table.table-enrollments tr td:nth-child(n+2) {
                text-align: left;
                width: 90px;
            }
    </style>
</insite:PageHeadContent>
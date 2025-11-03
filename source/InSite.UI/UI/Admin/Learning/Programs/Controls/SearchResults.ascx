<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.SearchResults" %>

<div class="search-results">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="ProgramIdentifier">
        <Columns>

            <asp:TemplateField HeaderText="Name" ItemStyle-Wrap="false"> 
                <ItemTemplate>
                    <a href='<%# InSite.Admin.Records.Programs.Outline.GetNavigateUrl((Guid)Eval("ProgramIdentifier")) %>'><i class="fas fa-pencil me-2"></i></a>
                    <a href='<%# InSite.Admin.Records.Programs.Outline.GetNavigateUrl((Guid)Eval("ProgramIdentifier")) %>'><%# Eval("ProgramName") %></a>
                    <%# Eval("CatalogName") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Code" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# Eval("ProgramCode") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Description"> 
                <ItemTemplate>
                    <span style="white-space:pre-wrap;"><%# Eval("ProgramDescription") %></span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Department" DataField="GroupName" ItemStyle-Wrap="false" />

            <asp:BoundField HeaderText="Categories" DataField="CategoryCount" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

            <asp:BoundField HeaderText="Enrollments" DataField="EnrollmentCount" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />
            
            <asp:BoundField HeaderText="Tasks" DataField="TaskCount" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

        </Columns>
    </insite:Grid>
</div>
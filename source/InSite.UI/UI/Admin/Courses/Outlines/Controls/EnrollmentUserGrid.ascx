<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnrollmentUserGrid.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.EnrollmentUserGrid" %>

<div class="d-flex justify-content-between align-items-top mb-3">
    <div>
        <insite:ComboBox runat="server" ID="EnrollmentStatus" Width="200px">
            <Items>
                <insite:ComboBoxOption Value="Started" Text="Started" Selected="true" />
                <insite:ComboBoxOption Value="Completed" Text="Completed" />
            </Items>
        </insite:ComboBox>
    </div>
    <div>
        <insite:DropDownButton runat="server" ID="DownloadDropDown" />
    </div>
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="LearnerIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to remove this enrollment?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                    <%# Eval("UserFullName") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href='mailto:<%# Eval("UserEmail") %>'><%# Eval("UserEmail") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Enrolled" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("EnrollmentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completed" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("CourseCompleted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Time Taken">
            <ItemTemplate>
                <%# Eval("TimeTaken") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Progress" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("CompletionCounter") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completion" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("CompletionPercent") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

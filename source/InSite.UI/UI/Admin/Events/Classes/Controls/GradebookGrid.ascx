<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradebookGrid.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.GradebookGrid" %>

<insite:Grid runat="server" ID="Grid">
    
        <Columns>

            <asp:TemplateField HeaderText="Title" ItemStyle-Wrap="False"> 
                <ItemTemplate>
                    <a href='/ui/admin/records/gradebooks/outline?<%# Eval("GradebookIdentifier", "id={0}") %>'><%# Eval("GradebookTitle") %></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Achievements"> 
                <ItemTemplate>
                    <%# GetAchievements() %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Created" ItemStyle-Wrap="False"> 
                <ItemTemplate>
                    <%# GetLocalTime(Eval("GradebookCreated")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="20" ItemStyle-CssClass="text-end"> 
                <ItemTemplate>
                    <insite:IconButton runat="server" ID="DeleteLink"
                        CommandName="Delete"
                        CommandArgument='<%# Eval("GradebookIdentifier") %>'
                        Name="trash-alt"
                        ToolTip="Delete"
                        Visible='<%# CanDelete() %>'
                        ConfirmText="Are you sure you want to delete this Gradebook from the Class?"
                    />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

</insite:Grid>

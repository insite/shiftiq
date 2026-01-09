<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramGroupGrid.ascx.cs" Inherits="InSite.UI.Admin.Learning.Programs.Controls.ProgramGroupGrid" %>

<div class="card mt-3">
    <div class="card-body">

        <h3>Groups</h3>

        <insite:Grid runat="server" ID="Grid" DataKeyNames="GroupIdentifier">
            <Columns>

                <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="False">
                    <ItemTemplate>
                        <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                            CommandName="Delete"
                            ConfirmText="Are you sure you want to remove this group from this program?" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Group" ItemStyle-Width="300px">
                    <ItemTemplate>
                        <a href="/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>">
                            <%# Eval("GroupName") %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Size" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <%# Eval("GroupSize") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Added" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("Added")) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Identity.UsersConnections.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="From User">
            <ItemTemplate>
                <%# Eval("FromUser.FullName") %>
                <div class="form-text">
                    <%# Eval("FromUser.Email") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="To User">
            <ItemTemplate>
                <%# Eval("ToUser.FullName") %>
                <div class="form-text">
                    <%# Eval("ToUser.Email") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Manager">
            <ItemTemplate>
                <%# Convert.ToBoolean(Eval("IsManager")) ? "<span class='text-success'>Yes</span>" : "<span class='text-danger'>No</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Supervisor">
            <ItemTemplate>
                <%# Convert.ToBoolean(Eval("IsSupervisor")) ? "<span class='text-success'>Yes</span>" : "<span class='text-danger'>No</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Validator">
            <ItemTemplate>
                <%# Convert.ToBoolean(Eval("IsValidator")) ? "<span class='text-success'>Yes</span>" : "<span class='text-danger'>No</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
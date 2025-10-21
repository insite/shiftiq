<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Authentications.ascx.cs" Inherits="InSite.UI.Portal.Home.Controls.Authentications" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Started">
            <ItemTemplate>
                <div class="fs-sm text-body-secondary">
                    <%# Eval("Started") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Host">
            <ItemTemplate>
                <%# Eval("Host") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("Status") %>
                <div class="form-text"><%# Eval("StatusInfo") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Browser">
            <ItemTemplate>
                <%# Eval("Browser") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Language">
            <ItemTemplate>
                <%# Eval("Language") %>
            </ItemTemplate>
        </asp:TemplateField>


    </Columns>
</insite:Grid>
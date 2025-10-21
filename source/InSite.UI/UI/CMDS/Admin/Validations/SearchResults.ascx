<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Validations.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:TemplateField HeaderText="Standard">
            <ItemTemplate>
                <%# Eval("StandardName") %>
                <div class="form-text"><%# Eval("StandardType") %> <%# Eval("StandardCode") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("UserFullName") %>
                <div class="form-text"><%# Eval("UserEmail") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("ValidationStatus") %>
                <div>
                    <%# GetStatusHtml((bool)Eval("IsValidated")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expired" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Expired") as DateTimeOffset?) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Validated" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime(Eval("ValidationDate") as DateTimeOffset?) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Validator" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("ValidatorUserFullName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit Validation'
                    NavigateUrl='<%# Eval("ValidationIdentifier", "/ui/cmds/admin/validations/edit?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
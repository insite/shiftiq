<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Publications.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="PageIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="47" ItemStyle-HorizontalAlign="left">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="trash-alt" CssClass="text-danger" ToolTip="Delete" NavigateUrl='<%# GetRedirectUrl("/ui/admin/assessments/publications/delete?page=" + Eval("PageIdentifier")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Page Title">
            <ItemTemplate>
                <a href='<%# Eval("PageIdentifier", "/ui/admin/assessments/publications/edit?page={0}") %>'>
                    <%# Eval("PageTitle") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Visibility" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("PageVisibilityHtml") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Form Name">
            <ItemTemplate>
                <a href='<%# GetFormLink(Container.DataItem) %>'>
                    <%# Eval("FormName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Asset #" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px" >
            <ItemTemplate>
                <%# Eval("FormAsset") %>.<%# Eval("FormAssetVersion") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Form Code" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("FormCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Privacy">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="GroupRepeater">
                    <ItemTemplate>
                        <div class="mb-1">
                            <%# Eval("GroupName") %>
                            <span class="badge bg-primary ms-2"><%# Eval("GroupType") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

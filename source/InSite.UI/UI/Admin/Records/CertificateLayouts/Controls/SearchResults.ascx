<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Records.AchievementLayouts.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="CertificateLayoutIdentifier">
    <Columns>
            
        <asp:TemplateField HeaderText="Code" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a title="Edit Code" href='<%# Eval("CertificateLayoutIdentifier", "/ui/admin/records/achievement-layouts/edit?id={0}") %>'>
                    <%# Eval("CertificateLayoutCode") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Data">
            <ItemTemplate>
                <%# Eval("CertificateLayoutData") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
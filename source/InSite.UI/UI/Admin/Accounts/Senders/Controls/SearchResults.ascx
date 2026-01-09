<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Senders.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="SenderIdentifier">
    <Columns>
            
        <asp:TemplateField HeaderText="Nickname">
            <ItemTemplate>
                <a title="Edit Sender" href='<%# Eval("SenderIdentifier", "/ui/admin/accounts/senders/edit?id={0}") %>'>
                    <%# Eval("SenderNickname") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <%# Eval("SenderType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="From">
            <ItemTemplate>
                <%# Eval("SenderName") %>
                <div class="form-text">
                    <%# Eval("SystemMailbox") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Reply To">
            <ItemTemplate>
                <%# Eval("SenderEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Company">
            <ItemTemplate>
                <%# Eval("CompanyAddress").ToString().Length>1?Eval("CompanyAddress"):"" %><br />
                <%# Eval("CompanyPostalCode").ToString().Length>1?Eval("CompanyPostalCode"):"" %><br />
                <%# Eval("CompanyCity").ToString().Length>1?Eval("CompanyCity") + ", ":""  %><%# Eval("CompanyCountry").ToString().Length>1?Eval("CompanyCountry"):"" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Message Count" DataField="MessageCount" DataFormatString="{0:n0}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />

        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# Eval("SenderIdentifier", "/ui/admin/accounts/senders/edit?id={0}") %>' ToolTip="Edit" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
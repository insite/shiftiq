<%@ Control AutoEventWireup="true" CodeBehind="MailLinkGrid.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.MailLinkGrid" Language="C#" %>

<asp:Literal runat="server" ID="NoRecordMessage" Text="No trackable links." />

<insite:Grid runat="server" ID="Grid" DataKeyNames="LinkIdentifier">
    <Columns>
        <insite:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false" FieldName="ActionColumn">
            <ItemTemplate>
                <asp:HyperLink runat="server" ID="TestLink" CssClass="icon" Style="text-decoration: none;" Target="_blank">
                    <i title="Open this link in a new browser tab" class="icon far fa-external-link"></i>
                </asp:HyperLink>
                <insite:IconButton runat="server"
                    Name="power-off"
                    CommandName="Delete"
                    ToolTip="Reset the counters on this link"
                    ConfirmText="Are you sure you want to reset this link?" />
            </ItemTemplate>
        </insite:TemplateField>
        <asp:BoundField HeaderText="Url" DataField="LinkUrl" />
        <asp:BoundField HeaderText="Text" DataField="LinkText" />
        <asp:BoundField HeaderText="Clicks" DataField="ClickCount" DataFormatString="{0:n0}" />
        <asp:BoundField HeaderText="Users" DataField="UserCount" DataFormatString="{0:n0}" />
    </Columns>

</insite:Grid>
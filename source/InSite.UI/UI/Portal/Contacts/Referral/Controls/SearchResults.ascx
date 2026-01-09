<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" Translation="Header">
        <Columns>
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Text='<%# Eval("FullName") %>' ToolTip="View Person"
                        NavigateUrl='<%# Eval("UserIdentifier", "/ui/portal/contacts/referral/outline?learner={0}") %>' />
                    <div class="fs-sm">
                        <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <insite:BoundField FieldName="PersonCode" DataField="PersonCode" HeaderStyle-Wrap="false" />

            <insite:TemplateField FieldName="Cases" HeaderText="Cases" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Literal ID="CaseTitles" runat="server"></asp:Literal>
                </ItemTemplate>
            </insite:TemplateField>

            <asp:TemplateField HeaderText="Date Referred">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("Referred")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <insite:TemplateField FieldName="Documents" HeaderText="Documents" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <i runat="server" id="HasAttachment" class="far fa-paperclip" title="Has Attachments"></i>
                </ItemTemplate>
            </insite:TemplateField>

            <asp:TemplateField HeaderText="Last Authenticated">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("LastAuthenticated")) %>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>
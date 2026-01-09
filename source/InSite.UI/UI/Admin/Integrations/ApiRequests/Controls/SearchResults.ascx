<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Integrations.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="RequestIdentifier">
    <Columns>
            
        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <ItemTemplate>
                    <a href="/ui/admin/integrations/api-requests/outline?request=<%# Eval("RequestIdentifier") %>">
                        <i class="far fa-search"></i>
                    </a>
                </ItemTemplate>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Requested" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime((DateTimeOffset)Eval("RequestStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Requested By" HeaderStyle-Wrap="false" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("ContactName") %>
                <div class="form-text"><%# Eval("OrganizationName") %></div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Request URL">
            <ItemTemplate>
                <%# GetRelativeURL((string)Eval("RequestUri")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Method" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("RequestMethod") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Response Status" HeaderStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("ResponseStatusNumber") %>
                <%# Eval("ResponseStatusName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Response Time" HeaderStyle-Wrap="False" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("ResponseTime", "{0:n0} ms") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Persons.PersonSearchResults" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonToolTipWithLinks.ascx" TagName="PersonToolTipWithLinks" TagPrefix="uc" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
		        <a href='<%# "/ui/cmds/admin/users/edit?userID=" + Eval("UserIdentifier") %>'>
		            <%# Eval("Name") %>
		        </a>
                <div class="form-text">
                    <%# Eval("Organization") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Literal runat="server" ID="Email" />
			</ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Signed In" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("LastAuthenticated", "{0:MMM d, yyyy}") %>
			</ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Profiles" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# GetProfilesCount(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <uc:PersonToolTipWithLinks runat="server" 
                    UserIdentifier='<%# Eval("UserIdentifier") %>'
                    UserFullName='<%# Eval("Name") %>' />
			</ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

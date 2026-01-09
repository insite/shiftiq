<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Persons.PersonGrid" %>

<%@ Register Src="PersonToolTipWithLinks.ascx" TagName="PersonToolTipWithLinks" TagPrefix="uc" %>

<asp:Panel ID="FilterPanel" runat="server" Visible="false" CssClass="mb-3">
    <asp:CheckBoxList ID="RoleTypeSelector" runat="server" RepeatDirection="Horizontal">
	    <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
        <asp:ListItem Value="Organization" Text="Organization Employment" />
	    <asp:ListItem Value="Administration" Text="Data Access" />
    </asp:CheckBoxList>
</asp:Panel>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
	<Columns>

		<insite:TemplateField ItemStyle-Width="20" FieldName="Delete">
			<ItemTemplate>
                <cmds:IconButton runat="server"
                    IsFontIcon="true" CssClass="trash-alt"
                    ToolTip="Remove"
                    ConfirmText="Are you sure you want to remove this record?"
                    CommandName="Delete"
                />
			</ItemTemplate>
		</insite:TemplateField>

		<insite:TemplateField HeaderText="Name" FieldName="Name">
			<ItemTemplate>
				<a href='<%# "/ui/cmds/admin/users/edit?userID=" + Eval("UserIdentifier") %>'>
					<%# Eval("FullName") %>
				</a>
			</ItemTemplate>
		</insite:TemplateField>

		<insite:BoundField HeaderText="Name" DataField="FullName" FieldName="NameWithoutLink" />
		<insite:BoundField HeaderText="City" DataField="AddressCity" FieldName="City"/>
		<insite:BoundField HeaderText="Province" DataField="AddressProvince" FieldName="Province"/>

		<insite:TemplateField HeaderText="Organization" FieldName="Organization">
			<ItemTemplate>
				<%# GetCompanyName((Guid)Eval("UserIdentifier")) %>
			</ItemTemplate>
		</insite:TemplateField>

		<insite:BoundField HeaderText="Email" DataField="Email" FieldName="EmailWork"/>

		<insite:TemplateField FieldName="ToolTipWithLinks" HeaderStyle-Width="300px">
			<ItemTemplate>
				<uc:PersonToolTipWithLinks runat="server" 
                    UserIdentifier='<%# (Guid)Eval("UserIdentifier") %>'
                    UserFullName='<%# (String)Eval("FullName") %>' />
			</ItemTemplate>
		</insite:TemplateField>

	</Columns>
</insite:Grid>
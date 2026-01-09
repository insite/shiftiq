<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Reports.Impersonations.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
		<asp:TemplateField HeaderText="Impersonatee">
			<ItemTemplate>
                <%# Eval("ImpersonatedContactLink") %>
			</ItemTemplate>
		</asp:TemplateField>

		<asp:TemplateField HeaderText="Impersonator">
			<ItemTemplate>
				<%# Eval("ImpersonatorContactLink") %>
			</ItemTemplate>
		</asp:TemplateField>

        <asp:TemplateField HeaderText="Started">
            <ItemTemplate>
                <%# Eval("Started") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Stopped">
            <ItemTemplate>
                <%# Eval("Stopped") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
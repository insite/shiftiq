<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="OrganizationIdentifier" Translation="Header">
        <Columns>
            <asp:TemplateField HeaderText="Title">
                <ItemTemplate>
                    <a href='<%# GetOutlineUrl() %>' title="Content Test"><%# Eval("ContentTitle") %></i></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Tag" DataField="StandardLabel" />
            <asp:BoundField HeaderText="Code" DataField="Code" />
            <asp:BoundField HeaderText="Asset #" DataField="AssetNumber" />

        </Columns>
    </insite:Grid>
</div>
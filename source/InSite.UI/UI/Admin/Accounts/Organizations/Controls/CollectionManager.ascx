<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionManager.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.CollectionManager" %>

<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/CollectionItemRepeater.ascx" TagName="CollectionItemRepeater" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/CollectionFolderRepeater.ascx" TagName="CollectionFolderRepeater" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/CollectionPersonRepeater.ascx" TagName="CollectionPersonRepeater" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                Collection
            </label>
            <insite:FindCollection runat="server" ID="CollectionSelector" />
        </div>

        <insite:Container runat="server" ID="ItemsContainer" Visible="false">
            <div class="form-group mb-3">
                <label class="form-label">
                    Items
                    <insite:IconButton runat="server" ID="AddItemButton" CssClass="align-middle"
                        ToolTip="Add Item" Name="plus-circle" />
                </label>
                <uc:CollectionItemRepeater runat="server" ID="ItemRepeater" ValidationGroup="Organization" />
            </div>
        </insite:Container>

        <insite:Container runat="server" ID="FoldersContainer" Visible="false">
            <div class="form-group mb-3">
                <label class="form-label">
                    Folders
                    <insite:IconButton Name="plus-circle" runat="server" ID="AddFolderButton" CssClass="align-middle"
                        ToolTip="Add Folder" />
                </label>
                <uc:CollectionFolderRepeater runat="server" ID="FolderRepeater" ValidationGroup="Organization" />
            </div>
        </insite:Container>

        <insite:Container runat="server" ID="PersonsContainer" Visible="false">
            <div class="form-group mb-3">
                <label class="form-label">
                    Persons
                    <insite:IconButton Name="plus-circle" runat="server" ID="AddPersonButton" CssClass="align-middle"
                        ToolTip="Add Person" />
                </label>
                <uc:CollectionPersonRepeater runat="server" ID="PersonRepeater" ValidationGroup="Organization" />
            </div>
        </insite:Container>
    </ContentTemplate>
</insite:UpdatePanel>

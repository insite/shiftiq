<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressSection.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.AddressSection" %>

<div class="row outline">
    <div class="col-2">
        <insite:Nav runat="server" ID="AddressesNav" 
            ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="AddressesNavContent">
        </insite:Nav>
    </div>
    <div class="col-4">
        <insite:NavContent runat="server" ID="AddressesNavContent" />
    </div>
</div>
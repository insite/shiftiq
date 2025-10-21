<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressList.ascx.cs" Inherits="InSite.Admin.Contacts.Addresses.Controls.AddressList" %>

<div class="clearfix" ></div>
<div class="row settings">
    <div class="col-md-2 col-sm-3">
        <insite:Nav runat="server" ID="AddressesNav" 
            ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="AddressesNavContent">
        </insite:Nav>
    </div>
    <div class="col-md-10 col-sm-9">
        <insite:NavContent runat="server" ID="AddressesNavContent" />
    </div>
</div>
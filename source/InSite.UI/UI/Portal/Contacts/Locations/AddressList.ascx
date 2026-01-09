<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressList.ascx.cs" Inherits="InSite.UI.Individual.Controls.AddressList" %>

<div class="row">
    <div class="col-md-12">

        <div class="row">
            <div class="col-md-2">
                <insite:Nav runat="server" ID="AddressesNav" 
                    ItemAlignment="Vertical" ItemType="Pills" ContentRendererID="AddressesNavContent">
                </insite:Nav>
            </div>
            <div class="col-md-10">
                <insite:NavContent runat="server" ID="AddressesNavContent" />
            </div>
        </div>
    </div>
</div>
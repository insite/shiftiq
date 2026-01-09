<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputSender.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputSender" %>

<div class="form-group mb-3">

    <div runat="server" id="SenderStatusOutput" class="float-end"></div>
    
    <label class="form-label">
        Sender
        <insite:RequiredValidator runat="server" ID="SenderFormValidator" ControlToValidate="SenderCombo" FieldName="Sender" />
    </label>
    
    <insite:SenderComboBox runat="server" ID="SenderCombo" />
    
    <div class="alert alert-info mt-3" runat="server" id="SenderDescription">
        
    </div>

    <div runat="server" id="SenderInstruction" class="form-text"></div>

</div>
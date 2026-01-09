<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadFieldRepeater.ascx.cs" Inherits="InSite.UI.Admin.Standards.Standards.Controls.UploadFieldRepeater" %>

<asp:Repeater runat="server" ID="FieldRepeater">
    <ItemTemplate>
        <div class="form-group mb-3 row">
            <label class="col-md-4 col-sm-3 control-label bold-xs"><%# Eval("Title") %></label>
            <div class="col-lg-1 col-md-2 col-xs-1 control-validator">
                <insite:RequiredValidator runat="server" ID="Validator" ControlToValidate="Selector" Display="Dynamic" />
            </div>
            <div class="col-lg-8 col-md-6 col-sm-8 col-xs-11">
                <insite:ComboBox runat="server" ID="Selector" AllowBlank="true" CssClass="w-100" />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

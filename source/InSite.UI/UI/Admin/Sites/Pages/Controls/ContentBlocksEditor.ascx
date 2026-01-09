<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentBlocksEditor.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocksEditor" %>

<div class="row">
    <div class="col-md-4">

        <div class="form-group mb-3">
            <label class="form-label">
                Block Type
                <insite:RequiredValidator runat="server" ID="ControlRequiredValidator" ControlToValidate="ControlSelector" FieldName="Block Type" />
            </label>
            <div>
                <insite:ComboBox runat="server" ID="ControlSelector" Width="100%" Enabled="false" />
            </div>
        </div>

    </div>
    <div class="col-md-4">

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink runat="server" ID="DeleteButton" ToolTip="Delete block" Name="trash-alt" />
            </div>
            <label class="form-label">
                Block Title
            </label>
            <div>
                <insite:TextBox runat="server" ID="Title" MaxLength="128" Width="100%" />
            </div>
        </div>

    </div>
    <div class="col-md-4">
        <a class="btn-scroll-bottom show" href="#bottom" data-scroll data-fixed-element><span class="btn-scroll-bottom-tooltip text-body-secondary fs-sm me-2">Bottom</span><i class="btn-scroll-bottom-icon far fa-arrow-down"></i></a>
    </div>
</div>

<div runat="server" id="ContentRow" class="row mt-3">
    <div class="col-lg-12">
        
        <h3>Block Content</h3>

        <asp:Repeater runat="server" ID="ContentFieldRepeater">
            <ItemTemplate>
                <div class="form-group mb-3">
                    <label class="form-label"><asp:Literal runat="server" ID="FieldName" Text='<%# Eval("Name") %>' /></label>
                    <div>
                        <insite:DynamicControl runat="server" ID="Container" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <div class="form-group mb-3">
            <label class="form-label ">Hook / Integration Code</label>
            <div>
                <insite:TextBox runat="server" ID="Hook" MaxLength="100" Width="100%" />
            </div>
        </div>

    </div>
</div>


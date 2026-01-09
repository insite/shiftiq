<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentBlocksViewer.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocksViewer" %>

<div class="row settings">
    <div class="col-md-4">

        <div class="form-group mb-3">
            <label class="form-label">
                <strong>Block Type</strong>
            </label>
            <div>
                <asp:Literal ID="ControlSelector" runat="server"></asp:Literal>
            </div>
        </div>

    </div>
    <div class="col-md-4">

        <div class="form-group mb-3">
            <label class="form-label">
                <strong>Block Title</strong>
            </label>
            <div>
                <asp:Literal ID="Title" runat="server"></asp:Literal>
            </div>
        </div>

    </div>
    <div class="col-md-4">
        <div class="float-end">
            <div class="content-cmds">
                <insite:Button runat="server" id="EditLink" ToolTip="Revise" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
            </div>
        </div>
    </div>
</div>

<div runat="server" id="ContentRow" class="row settings">
    <div class="col-md-12 pt-4">
        
        <h3>Block Content</h3>

        <asp:Repeater runat="server" ID="ContentFieldRepeater">
            <ItemTemplate>
                <div class="form-group pb-2">
                    <label class="form-label"><asp:Literal runat="server" ID="FieldName" Text='<%# Eval("Name") %>' /></label>
                    <div>
                        <insite:DynamicControl runat="server" ID="Container" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="form-group mb-3">
            <label class="form-label">
                <strong>Hook / Integration Code</strong>
            </label>
            <div>
                <asp:Literal ID="Hook" runat="server"></asp:Literal>
            </div>
        </div>

    </div>
</div>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Standards.Collections.Controls.Detail" %>

<div class="row">
    <div class="col-md-6">
        <div class="settings">

            <div class="form-group mb-3">
                <div class="float-end">
                    <insite:IconLink Name="pencil" runat="server" ID="TitleLink" Style="padding: 8px" ToolTip="Change Title" />
                </div>
                <asp:Label runat="server" ID="TitleLabel" AssociatedControlID="Title" Text="Title" CssClass="form-label" />
                <div>
                    <asp:Literal runat="server" ID="Title" />
                </div>
            </div>

            <div class="form-group mb-3">
                <div class="float-end">
                    <insite:IconLink Name="pencil" runat="server" ID="TagLink" Style="padding: 8px" ToolTip="Change Tag" />
                </div>
                <asp:Label runat="server" ID="LabelLabel" AssociatedControlID="Label" Text="Tag" CssClass="form-label" />
                <div>
                    <asp:Literal runat="server" ID="Label" />
                </div>
            </div>

        </div>
    </div>
</div>

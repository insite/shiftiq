<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailInfo.ascx.cs" Inherits="InSite.Admin.Standards.Collections.Controls.DetailInfo" %>

<div class="row">

    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Title
            </label>
            <div>
                <asp:Literal runat="server" ID="Title" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Tag
            </label>
            <div>
                <asp:Literal runat="server" ID="Label" />
            </div>
        </div>

    </div>
</div>

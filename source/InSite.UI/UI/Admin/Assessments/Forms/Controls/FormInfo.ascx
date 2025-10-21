<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormInfo.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormInfo" %>

<%@ Register Src="~/UI/Admin/Assessments/Specifications/Controls/SpecificationInfo.ascx" TagName="SpecificationDetails" TagPrefix="uc" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div class="row">

    <div class="col-lg-6">
        <h3>Form</h3>
        <div runat="server" id="FormStandardField" class="form-group mb-3">
            <label class="form-label">Standard</label>
            <div>
                <assessments:AssetTitleDisplay runat="server" ID="FormStandard" />
            </div>
        </div>

        <div runat="server" id="FormNameField" class="form-group mb-3">
            <label class="form-label">Form Name</label>
            <div>
                <asp:Literal runat="server" ID="Name" />
            </div>
        </div>

        <div runat="server" id="FormAssetField" class="form-group mb-3">
            <label class="form-label">Asset Number and Version</label>
            <div>
                <asp:Literal runat="server" ID="Version" />
            </div>
        </div>

        <div runat="server" id="CodeField" class="form-group mb-3">
            <label class="form-label">Code</label>
            <div>
                <asp:Literal runat="server" ID="Code" />
            </div>
        </div>

        <div runat="server" id="SourceField" class="form-group mb-3">
            <label class="form-label">Source</label>
            <div>
                <asp:Literal runat="server" ID="Source" />
            </div>
        </div>

        <div runat="server" id="OriginField" class="form-group mb-3">
            <label class="form-label">Origin</label>
            <div>
                <asp:Literal runat="server" ID="Origin" />
            </div>
        </div>

        <div runat="server" id="HookField" class="form-group mb-3">
            <label class="form-label">Hook / Integration Code</label>
            <div>
                <asp:Literal runat="server" ID="Hook" />
            </div>
        </div>
    </div>

    <div id="SpecDiv" runat="server" class="col-lg-6">
        <h3>Specification</h3>
        <uc:SpecificationDetails ID="SpecificationDetails" runat="server" />
    </div>

</div>

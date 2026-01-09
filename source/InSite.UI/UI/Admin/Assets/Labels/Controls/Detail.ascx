<%@ Control AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Utilities.Labels.Controls.Detail" Language="C#" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/TranslationControl.ascx" TagName="TranslationControl" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-12">

        <h2 class="mt-0">
            Placeholder Name
            <insite:RequiredValidator runat="server" ControlToValidate="LabelName" FieldName="Placeholder Name" ValidationGroup="Label" />
        </h2>
        <div class="form-group mb-3">
            <div>
                <insite:TextBox runat="server" ID="LabelName" MaxLength="128" />
            </div>
        </div>

    </div>
</div>

<uc:TranslationControl runat="server" ID="LabelTranslation" />

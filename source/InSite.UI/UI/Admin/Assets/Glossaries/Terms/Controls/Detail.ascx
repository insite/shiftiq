<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Controls.Detail" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor/Field.ascx" TagPrefix="uc" TagName="ContentField" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .form-group label {
            font-weight: bold;
        }
    </style>
</insite:PageHeadContent>

<div class="row">
    <div class="col-md-6">
        <div class="form-group mb-3">
            <label class="form-label">
                Name
                <insite:RequiredValidator runat="server" ControlToValidate="TermName" ValidationGroup="GlossaryTerm" />
            </label>
            <insite:TextBox runat="server" ID="TermName" Width="100%" />
        </div>
        <div class="form-group mb-3">
            <label class="form-label">
                Title
            </label>
            <div>
                <uc:ContentField runat="server" ID="TermTitle" />
            </div>
        </div>
        <div class="form-group mb-3">
            <label class="form-label">
                Definition
                <insite:CustomValidator runat="server" ID="DefinitionRequiredValidator" Display="Dynamic" ErrorMessage="Required field: Definition" ValidationGroup="GlossaryTerm" />
            </label>
            <div>
                <uc:ContentField runat="server" ID="TermDefinition" />
            </div>
        </div>
    </div>
</div>

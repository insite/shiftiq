<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineContentField.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.OutlineContentField" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>

<div class="row settings">
    <div class="col-md-12">                           

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:Button runat="server" id="ChangeLink" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
            </div>
			<div>
                <uc:MultilingualStringInfo runat="server" ID="Output" />
            </div>
            <div runat="server" id="FieldDescription" class="form-text">
            </div>
        </div>

    </div>
</div>

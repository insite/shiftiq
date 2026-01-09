<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentOutput.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentOutput" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>

<div class="content-cmds">
    <insite:Button runat="server" id="EditLink" ToolTip="Revise" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
</div>
<div class="content-string">
    <uc:MultilingualStringInfo runat="server" ID="DataOutput" />
</div>

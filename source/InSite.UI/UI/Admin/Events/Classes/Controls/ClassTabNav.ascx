<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassTabNav.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassTabNav" %>

<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassSetup.ascx" TagName="ClassSetup" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassSettings.ascx" TagName="ClassSettings" TagPrefix="uc" %>

<insite:Nav runat="server">               
    <insite:NavItem runat="server" ID="ClassSetupTab" Title="Class Setup">
        <uc:ClassSetup runat="server" ID="ClassSetupControl" />
    </insite:NavItem>
    <insite:NavItem runat="server" ID="ClassSettingsTab" Title="Class Settings">
        <div class="row">
            <div class="col-xxl-6">
                <uc:ClassSettings runat="server" ID="ClassSettingsControl" />
            </div>
        </div>
    </insite:NavItem>
</insite:Nav>

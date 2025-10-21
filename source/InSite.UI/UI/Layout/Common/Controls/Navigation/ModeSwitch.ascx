<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModeSwitch.ascx.cs" Inherits="InSite.UI.Layout.Admin.ModeSwitch" %>

<div class="form-check form-switch mode-switch ms-4" data-bs-toggle="mode">
    <input runat="server" id="themeMode" class="form-check-input" type="checkbox"
        onserverchange="ThemeMode_ServerChange" />
    <label class="form-check-label" for="theme-mode">
        <i runat="server" id="offIcon" class="fa-light fa-sun-bright fs-lg"></i>
    </label>
    <label class="form-check-label" for="theme-mode">
        <i runat="server" id="onIcon" class="fa-light fa-moon fs-lg"></i>
    </label>
</div>

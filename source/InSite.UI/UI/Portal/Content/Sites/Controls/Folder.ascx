<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Folder.ascx.cs" Inherits="InSite.UI.Portal.Sites.PageFolder" %>

<%@ Register Src="../../../Controls/LaunchCardRepeater.ascx" TagPrefix="uc" TagName="LaunchCardRepeater" %>

<div class="mb-4" runat="server" id="ListBody"></div>

<uc:LaunchCardRepeater runat="server" ID="Repeater" />
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownloadColumnTabs.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchDownloadColumnTabs" %>

<%@ Register TagPrefix="uc" TagName="ColumnRepeater" Src="~/UI/Layout/Common/Controls/SearchDownloadColumnRepeater.ascx" %>

<insite:Nav runat="server" ID="GroupTabs" CssClass="mt-3 mb-3" />
<uc:ColumnRepeater runat="server" ID="ColumnRepeater" />

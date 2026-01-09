<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuildDataSet.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.BuildDataSet" %>

<insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="DataSetSelect" RenderMode="Exclamation" Display="None" />

<insite:ComboBox runat="server" ID="DataSetSelect" AllowBlank="false" />

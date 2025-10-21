<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupInfo.ascx.cs" Inherits="InSite.Admin.Contacts.Groups.Controls.GroupInfo" %>

<dl>
  <dt>Group Name</dt>
  <dd>
      <asp:Literal runat="server" ID="GroupName" />
      <a runat="server" id="GroupLink"></a>
  </dd>
  <dt>Group Tag</dt>
  <dd><asp:Literal runat="server" ID="GroupLabel" /></dd>
  <dt>Group Type</dt>
  <dd><asp:Literal runat="server" ID="GroupType" /></dd>
    <dt>Group Code</dt>
  <dd><asp:Literal runat="server" ID="GroupCode" /></dd>
</dl>
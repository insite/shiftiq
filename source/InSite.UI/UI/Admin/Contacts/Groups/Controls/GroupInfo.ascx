<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupInfo.ascx.cs" Inherits="InSite.Admin.Contacts.Groups.Controls.GroupInfo" %>

<dl>
  
    <dt><asp:Literal runat="server" ID="GroupType" /></dt>
  
    <dd>
      <asp:Literal runat="server" ID="GroupName" />
        <div>
          <asp:Literal runat="server" ID="GroupTag" />
          <asp:Literal runat="server" ID="GroupCode" />
        </div>
    </dd>

    <dt runat="server" id="GroupReferencesTerm">References</dt>

    <dd runat="server" id="GroupReferencesDefn"></dd>

</dl>

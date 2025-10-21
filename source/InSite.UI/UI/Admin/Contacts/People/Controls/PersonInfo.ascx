<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonInfo.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.PersonInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Name
    </label>
    <div>
        <asp:Literal runat="server" ID="UserName" />
        <a runat="server" id="UserLink"></a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Email
    </label>
    <div>
        <asp:Literal runat="server" ID="Email" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
    </label>
    <div>
        <asp:Literal runat="server" ID="ContactCode" />
    </div>
</div>

<div runat="server" id="BirthDiv" class="form-group mb-3">
    <label class="form-label">
        Birthdate
    </label>
    <div>
        <asp:Literal runat="server" ID="Birthdate" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Employer
    </label>
    <div>
        <asp:Literal runat="server" ID="Employer" />
    </div>
</div>
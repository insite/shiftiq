<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramDetails.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.ProgramDetails" %>

<dl class="row">
    <dt class="col-sm-3">Code:</dt>
    <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgramCode" /></dd>

    <dt class="col-sm-3">Name:</dt>
    <dd class="col-sm-9">
        <a runat="server" id="ProgramLink">
            <asp:Literal runat="server" ID="ProgramName" />
        </a>    
    </dd>
</dl>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankInfo.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.BankInfo" %>

<div id="StandardDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Standard</label>
    <div>
        <asp:Literal runat="server" ID="BankStandard" />
    </div>
</div>

<div id="LevelDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Level</label>
    <div>
        <asp:Literal runat="server" ID="BankLevel" />
    </div>
</div>

<div id="NameDiv" runat="server" class="form-group mb-3">
    <label class="form-label">
        Internal Name
    </label>
    <div>
        <asp:Literal runat="server" ID="BankName" />
    </div>
</div>

<div id="EditionDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Edition</label>
    <div>
        <asp:Literal runat="server" ID="BankEdition" />
    </div>
</div>

<div runat="server" id="StatusDiv" class="form-group mb-3">
    <label class="form-label">Current Status</label>
    <div>
        <asp:Literal runat="server" ID="BankStatus" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Asset Number</label>
    <div>
        <asp:Literal runat="server" ID="AssetNumber" />
    </div>
</div>

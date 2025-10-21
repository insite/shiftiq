<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogEntryDetail.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Controls.LogEntryDetail" %>

<div class="form-group mb-3">
    <label class="form-label">Entry Number</label>
    <div>
        <asp:Literal runat="server" ID="EntryNumber" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Entry Created</label>
    <div>
        <asp:Literal runat="server" ID="EntryCreated" />
    </div>
</div>

<asp:Repeater runat="server" ID="FieldRepeater">
    <ItemTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                <%# Eval("LabelText") %>
            </label>
            <div>
                <insite:DynamicControl runat="server" ID="FieldValue" />
            </div>
            <div class="form-text">
                <%# Eval("HelpText") %>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

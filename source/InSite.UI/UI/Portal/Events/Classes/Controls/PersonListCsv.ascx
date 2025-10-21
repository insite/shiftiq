<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonListCsv.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.PersonListCsv" %>

<div class="row pb-2">
    <div class="col-md-12">
        <div>
            <b>Fields:</b> [First Name],[Last Name],[Email],[Birthday],[Learner ID Number],[Phone]
        </div>
        <div>
            <b>Birthday format:</b> YYYY-MM-DD
        </div>
    </div>
</div>

<div class="row pb-2">
    <div class="col-md-12">
        <asp:CustomValidator runat="server" ID="FinalValidator" Display="none" ValidationGroup="PersonCsv" />

        <insite:TextBox runat="server" ID="CsvText" Rows="15" TextMode="MultiLine" />
    </div>
</div>
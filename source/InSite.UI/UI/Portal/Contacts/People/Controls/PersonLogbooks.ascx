<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonLogbooks.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonLogbooks" %>

<div class="card">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoLogbooks" Text="No logbooks to display." />

        <asp:Repeater runat="server" ID="LogbookRepeater">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Logbook</th>
                            <th runat="server" visible='<%# IsHoursColumnVisible %>' style="text-align:center;">Hours</th>
                            <th style="text-align:center;">Entries</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("JournalSetupName") %>
                    </td>
                    <td runat="server" visible='<%# IsHoursColumnVisible %>' style="text-align:center;">
                        <%# Eval("HourSum", "{0:n2}") %>
                    </td>
                    <td style="width:200px;text-align:center;">
                        <%# Eval("EntryCount") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>

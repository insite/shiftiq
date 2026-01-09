<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogbookList.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.LogbookList" %>

<div runat="server" id="NoLogbooks" class="alert alert-warning" role="alert">
    There are no logbooks
</div>

<asp:Panel runat="server" ID="LogbookPanel">
    <div class="row mb-3">
        <div class="col-lg-6">
            <insite:TextBox runat="server" ID="FilterText" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
            <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />

            <insite:PageFooterContent runat="server">
                <script type="text/javascript"> 
                    (function () {
                        Sys.Application.add_load(function () {
                            $('#<%= FilterText.ClientID %>')
                                .off('keydown', onKeyDown)
                                .on('keydown', onKeyDown);
                        });

                        function onKeyDown(e) {
                            if (e.which === 13) {
                                e.preventDefault();
                                $('#<%= FilterButton.ClientID %>')[0].click();
                            }
                        }
                    })();
                </script>
            </insite:PageFooterContent>
        </div>
    </div>
        
    <asp:Repeater runat="server" ID="LogbookRepeater">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Logbook</th>
                        <th style="text-align:center;">Entries</th>
                        <th></th>
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
                    <a runat="server" visible='<%# Eval("IsAdmin") %>' href='<%# Eval("JournalSetupIdentifier", "/ui/admin/records/logbooks/outline?id={0}") %>'>
                        <%# Eval("JournalSetupName") %>
                    </a>
                    <asp:Literal runat="server" Visible='<%# Eval("IsValidator") %>' Text='<%# Eval("JournalSetupName") %>' />
                </td>
                <td style="width:200px;text-align:center;">
                    <%# Eval("EntryCount") %>
                </td>
                <td style="width:40px;text-align:right;">
                    <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Logbook"
                        Visible='<%# (bool)Eval("IsAdmin") && Eval("JournalIdentifier") != null %>'
                        NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/outline-journal?journalSetup={0}&user={1}", Eval("JournalSetupIdentifier"), Eval("UserIdentifier")) %>'
                    />
                    <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Logbook"
                        Visible='<%# (bool)Eval("IsValidator") && Eval("JournalIdentifier") != null %>'
                        NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/validators/outline-journal?journalSetup={0}&user={1}", Eval("JournalSetupIdentifier"), Eval("UserIdentifier")) %>'
                    />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

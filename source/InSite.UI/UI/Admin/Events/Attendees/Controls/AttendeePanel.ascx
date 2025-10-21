<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendeePanel.ascx.cs" Inherits="InSite.Admin.Events.Attendees.Controls.AttendeePanel" %>
<%@ Register Src="PersonAttendeePanel.ascx" TagName="PersonAttendeePanel" TagPrefix="uc" %>

<div runat="server" id="ButtonPanel" class="float-end">
    <asp:HyperLink runat="server" ID="AddButton" CssClass="btn btn-default" Text="<i class='fas fa-plus-circle me-2'></i> Add Contact" />
</div>

<div runat="server" id="FilterPanel" class="mb-3">
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

<asp:Repeater runat="server" ID="PersonAttendeeRepeater">
    <ItemTemplate>
        <div runat="server" id="Wrapper" class="mb-4">
            <h3>
                <%# (string)Eval("AttendeeRole") ?? "No Role" %>
                <span runat="server" id="Subtitle" class="form-text"></span>
            </h3>
            <div>
                <uc:PersonAttendeePanel runat="server" ID="PersonAttendeePanel" />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

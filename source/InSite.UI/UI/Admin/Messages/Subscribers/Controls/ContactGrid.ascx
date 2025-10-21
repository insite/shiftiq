<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactGrid.ascx.cs" Inherits="InSite.Admin.Messages.Contacts.Controls.ContactGrid" %>
<%@ Register Src="ContactGridGroup.ascx" TagName="ContactGridGroup" TagPrefix="uc" %>
<%@ Register Src="ContactGridPerson.ascx" TagName="ContactGridPerson" TagPrefix="uc" %>

<div runat="server" id="FilterSection">

    <div class="row mb-2">
        <asp:Panel runat="server" ID="ButtonPanel" class="col-lg-6">
            <insite:Button runat="server" ID="AddButton" ButtonStyle="Default" Text="Add Subscriber" Icon="fas fa-plus-circle" CssClass="mb-3" />
            <insite:Button runat="server" ID="DeleteButton" ButtonStyle="Danger" Text="Delete Subscribers" Icon="far fa-trash-alt" CssClass="mb-3" />
        </asp:Panel>
        <asp:Panel runat="server" ID="SearchPanel" DefaultButton="SearchInput" CssClass="col-lg-6">
            <insite:InputSearch runat="server" ID="SearchInput" />
        </asp:Panel>
    </div>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript"> 
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= SearchInput.ClientID %>_box')
                    .off('keydown', onKeyDown)
                    .on('keydown', onKeyDown);
            });

            function onKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= SearchInput.ClientID %>_button')[0].click();
                }
            }
        })();
    </script>
</insite:PageFooterContent>

<div runat="server" id="PersonCard" class="card mb-3">
    <div class="card-body">
        <h3 class="card-title mb-3">People</h3>

        <div class="overflow-auto">
            <uc:ContactGridPerson runat="server" ID="PersonGrid" />
        </div>
    </div>
</div>

<div runat="server" id="GroupCard" class="card mb-3">
    <div class="card-body overflow-auto">
        <h3 class="card-title mb-3">Groups</h3>

        <div class="overflow-auto">
            <uc:ContactGridGroup runat="server" ID="GroupGrid" />
        </div>
    </div>
</div>
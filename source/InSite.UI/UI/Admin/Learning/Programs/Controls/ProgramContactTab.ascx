<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramContactTab.ascx.cs" Inherits="InSite.UI.Admin.Learning.Programs.Controls.ProgramContactTab" %>

<%@ Register TagPrefix="uc" TagName="ProgramPersonGrid" Src="./ProgramPersonGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProgramGroupGrid" Src="./ProgramGroupGrid.ascx" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <div runat="server" id="FilterSection" class="row">
            <div class="col-lg-6">
                <insite:Button runat="server" ID="AddButton" Text="Add Learners" Icon="fas fa-plus-circle" ButtonStyle="Success" />
            </div>
            <asp:Panel runat="server" DefaultButton="SearchInput" CssClass="col-lg-6">
                <insite:InputSearch runat="server" ID="SearchInput" />
            </asp:Panel>
        </div>

        <div runat="server" id="NoLearners" class="alert d-flex alert-warning mt-3" role="alert">
            <i class="fa-solid fa-exclamation-triangle fs-xl pe-1 me-2"></i>
            <div>No learners enrolled</div>
        </div>

        <uc:ProgramPersonGrid runat="server" ID="PersonGrid" />

        <uc:ProgramGroupGrid runat="server" ID="GroupGrid" />

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
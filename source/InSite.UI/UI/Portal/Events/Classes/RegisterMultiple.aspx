<%@ Page Language="C#" CodeBehind="RegisterMultiple.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.RegisterMultiple" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/PersonList.ascx" TagName="PersonList" TagPrefix="uc" %>
<%@ Register Src="Controls/PersonListCsv.ascx" TagName="PersonListCsv" TagPrefix="uc" %>
<%@ Register Src="Controls/EmployerDetail.ascx" TagName="EmployerDetail" TagPrefix="uc" %>
<%@ Register Src="Controls/SeatDetail.ascx" TagName="SeatDetail" TagPrefix="uc" %>
<%@ Register Src="Controls/ConfirmPayment.ascx" TagName="ConfirmPayment" TagPrefix="uc" %>
<%@ Register Src="Controls/PaymentDetail.ascx" TagName="PaymentDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .payment-overlay {
            width: 100%;
            height: 100%;
            position: absolute;
            top: 0;
            left: 0;
            background: rgba( 205, 205, 205, 0.4 )  url('/images/animations/loader.gif') 50% 50% no-repeat;
            z-index: 9000;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div id="PaymentOverlay"></div>

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:ValidationSummary runat="server" ValidationGroup="PersonList" />
    <insite:ValidationSummary runat="server" ValidationGroup="PersonCsv" />
    <insite:ValidationSummary runat="server" />

    <asp:CustomValidator runat="server" ID="AvailabilityValidator" />

    <asp:Repeater runat="server" ID="TopProgressBar">
        <HeaderTemplate><ul class='dot-indicator'></HeaderTemplate>
        <FooterTemplate></ul></FooterTemplate>
        <ItemTemplate>
            <li data-num='<%# Eval("Number") %>' class='<%# (bool)Eval("IsCompleted") ? "step-complete" : "" %>'>
                <%# Eval("Title") %>
            </li>
        </ItemTemplate>
    </asp:Repeater>

    <insite:Accordion runat="server" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="PersonsPanel" Icon="far fa-users" Title="People">
            <insite:Nav runat="server">
                <insite:NavItem runat="server" ID="PersonListTab" Title="List">
                    <uc:PersonList runat="server" ID="PersonList" />

                    <div class="pt-3">
                        <insite:NextButton runat="server" ID="ApplyPersonListButton" ValidationGroup="PersonList" />
                        <insite:CancelButton runat="server" ID="CancelButton1" ConfirmText="Are you sure you want to close page without saving any information?" />
                    </div>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="PersonListCsvTab" Title="CSV">
                    <uc:PersonListCsv runat="server" ID="PersonListCsv" />

                    <div class="pt-3">
                        <insite:NextButton runat="server" ID="ApplyPersonCsvButton" ValidationGroup="PersonCsv" />
                        <insite:CancelButton runat="server" ID="CancelButton12" ConfirmText="Are you sure you want to close page without saving any information?" />
                    </div>
                </insite:NavItem>

            </insite:Nav>
        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="EmployerPanel" Icon="far fa-user-tie" Title="Employer" Visible="false">
            <uc:EmployerDetail runat="server" ID="EmployerDetail" />

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton2" />
                <insite:CancelButton runat="server" ID="CancelButton2" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>
        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="SeatPanel" Icon="far fa-money-check-alt" Title="Class Registration" Visible="false">
            <uc:SeatDetail runat="server" ID="SeatDetail" />

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton2" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton3" Enabled="false" />
                <insite:CancelButton runat="server" ID="CancelButton3" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="PaymentSection" Icon="far fa-credit-card-front" Title="Payment" Visible="false">
            <insite:Alert runat="server" ID="PaymentNotAvailable" />
            <insite:Alert runat="server" ID="PaymentAlert" />

            <uc:PaymentDetail runat="server" ID="PaymentDetail" />

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton3" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton4" ValidationGroup="Payment" />
                <insite:CancelButton runat="server" ID="CancelButton4" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="ConfirmSection" Icon="far fa-file-check" Title="Confirm Details & Payment" Visible="false">
            <uc:ConfirmPayment runat="server" ID="ConfirmPayment" />

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton4" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:Button runat="server" ID="PreSaveButton" Text="Confirm Registration" ButtonStyle="Success" Icon="fas fa-cloud-upload" OnClientClick="onSave(); return false;" CausesValidation="false" />
                <insite:CancelButton runat="server" ID="CancelButton5" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
    </insite:Accordion>

<asp:Button runat="server" ID="SaveButton" style="display:none;" />

<script>
    function onSave() {
        var $btn = $("#<%= PreSaveButton.ClientID %>");
        if ($btn.prop('disabled'))
            return;

        Page_ClientValidate('');
        if (!Page_IsValid)
            return;

        $btn.prop('disabled', true);

        $("#PaymentOverlay").addClass("payment-overlay");

        __doPostBack("<%= SaveButton.UniqueID %>", "");
    }

    function onPaymentNextClicked() {
        Page_ClientValidate('Payment');
        if (!Page_IsValid)
            return;

        __doPostBack("<%= NextButton4.UniqueID %>", "");
    }
</script>

</asp:Content>
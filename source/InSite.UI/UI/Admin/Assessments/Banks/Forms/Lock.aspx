<%@ Page Language="C#" CodeBehind="Lock.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.Lock" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/BankInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Bank" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-balance-scale"></i>
            <asp:Literal ID="CardHeader" runat="server"></asp:Literal>
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Bank</h3>
                        <uc:Details runat="server" ID="BankDetails" />
                    </div>

                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">

            <insite:Button runat="server" ID="LockButton" Text="Lock" Icon="fas fa-lock" ButtonStyle="Danger" />
            <insite:Button runat="server" ID="UnlockButton" Text="Unlock" Icon="far fa-lock-open" ButtonStyle="Success" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />

        </div>
    </div>

</asp:Content>

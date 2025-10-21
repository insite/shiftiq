<%@ Page Language="C#" CodeBehind="RegisterSuccess.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.RegisterSuccess" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="card card-hover shadow mb-3">
        <div class="card-body">
            <h2><insite:Literal runat="server" Text="Thank you!" /></h2>

            <h2><insite:Literal runat="server" Text="Registration Complete" /></h2>

            <p runat="server" id="default_message">
                <insite:Literal runat="server" Text="You registration is complete. We look forward to seeing you!" />
            </p>

            <div>
                <asp:Literal runat="server" ID="RegistrationCompletion" />
            </div>

            <p>
                <insite:Literal runat="server" Text="Payment Received. Thank you." /> <br />
                <insite:Button runat="server" ID="PrintReceiptButton" Text="Print Receipt" Icon="fas fa-print" ButtonStyle="Default" />
            </p>

            <div class="pt-3">
                <insite:Button runat="server" ID="ReturnButton" ButtonStyle="Primary" Text="Return to Class Search Page" Icon="fas fa-calendar-alt" />
            </div>
        </div>
    </div>

</asp:Content>
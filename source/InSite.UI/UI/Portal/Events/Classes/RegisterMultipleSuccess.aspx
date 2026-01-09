<%@ Page Language="C#" CodeBehind="RegisterMultipleSuccess.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.RegisterMultipleSuccess" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="card card-hover shadow mb-3">
        <div class="card-body">
            <h2><insite:Literal runat="server" Text="Thank you!" /></h2>

            <h2><insite:Literal runat="server" Text="Registration Complete" /></h2>

            <p runat="server" id="DefaultMessage">
                <insite:Literal runat="server" Text="You registration is complete. We look forward to seeing you!" />
            </p>

            <div>
                <asp:Literal runat="server" ID="RegistrationCompletion" />
            </div>

            <table class="table table-striped">
                <asp:Repeater runat="server" ID="RegistrationRepeater">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("Candidate.UserFullName") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>

            <p>
                <insite:Literal runat="server" Text="Payment Received. Thank you." /> <br />
                <insite:Button runat="server" ID="PrintReceiptButton" Text="Print Receipt" Icon="fas fa-print" ButtonStyle="Default" />
            </p>

            <div class="pt-3">
                <insite:Button runat="server" ID="ReturnButton" ButtonStyle="Primary" Text="Return to Class Outline Page" Icon="fas fa-calendar-alt" />
            </div>
        </div>
    </div>

</asp:Content>
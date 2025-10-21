<%@ Page Language="C#" CodeBehind="Send.aspx.cs" Inherits="InSite.Cmds.Admin.People.Forms.Send" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="SendStatus" />

        <h2 class="h4 mb-3"><i class="far fa-paper-plane me-2"></i>Email</h2>

        <div class="card border-0 shadow-lg">

            <div class="card-body">

                <div class="mb-3 row">
                    <label class="col-md-2 col-form-label" for="<%= To.ClientID %>">To:</label>
                    <div class="col-md-10">
                        <insite:TextBox ID="To" runat="server" Name="To" Enabled="false" />
                    </div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-2 col-form-label" for="<%= Cc.ClientID %>">Cc:</label>
                    <div class="col-md-10">
                        <insite:TextBox ID="Cc" runat="server" Name="Cc" />
                    </div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-2 col-form-label" for="<%= Subject.ClientID %>">
                        Subject:
                        <insite:RequiredValidator runat="server" ControlToValidate="Subject" ValidationGroup="SendEmail" />
                    </label>
                    <div class="col-md-10">
                        <insite:TextBox ID="Subject" runat="server" Name="Subject" />
                    </div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-2 col-form-label" for="<%= MessageBody.ClientID %>">
                        Body:
                    </label>
                    <div class="col-md-10">
                        <insite:TextBox runat="server" ID="MessageBody" TextMode="MultiLine" Rows="21" />
                    </div>
                </div>

            </div>
        </div>
    
    <div class="row mt-4">
        <div class="col-lg-12">
            <asp:LinkButton runat="server" ID="SendButton" CssClass="btn btn-sm btn-success" ValidationGroup="SendEmail"><i class='far fa-paper-plane me-2'></i>Send</asp:LinkButton>
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>

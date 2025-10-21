<%@ Page Language="C#" CodeBehind="ChangeSender.aspx.cs" Inherits="InSite.Admin.Messages.Messages.ChangeSender" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="InputSender" Src="./Controls/FieldInputSender.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Message" />

    <section class="mb-3">
        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>Message Sender</h3>

                        <div class="form-group mb-3">
                            <uc:InputSender runat="server" ID="SenderInput" ValidationGroup="Message" />
                        </div>
                        
                        <div class="form-group mb-3">
                            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Message" />
                            <insite:CancelButton runat="server" ID="CancelButton" />
                        </div>

                        <asp:Panel runat="server" ID="SenderWarningPanel" class="alert alert-warning" Visible="false">
                            <i class="fas fa-exclamation-triangle"></i> <strong>Please Note!</strong>
                            The email address for the sender of this message is <b><asp:Literal runat="server" ID="WarningEmailLiteral" /></b>.
                            Please ensure your mail server administrator (for the domain <b><asp:Literal runat="server" ID="WarningDomainLiteral" /></b>)
                            is aware of the fact that you are sending email messages from this system.
                            Otherwise, recipients' mail servers might refuse to accept this email message.
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>

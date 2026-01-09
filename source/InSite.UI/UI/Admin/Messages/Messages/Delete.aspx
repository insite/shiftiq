<%@ Page MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Messages.Messages.DeleteForm" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Message" />

    <div class="row settings mb-5">

        <div class="col-lg-7">
                                    
            <div class="settings">
                <h3>Message</h3>

                <div class="row">

                    <div class="col-lg-12">

                        <dl class="row">
                            <dt class="col-sm-3">Sender:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="Sender" /></dd>

                            <dt class="col-sm-3">Subject:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="Subject" /></dd>

                            <dt class="col-sm-3">Message Status:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="MessageStatus" /></dd>

                            <dt class="col-sm-3">Message Type:</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="MessageType" /></dd>

                            <dt class="col-sm-3" runat="server" id="NameFieldHeader">Message Name:</dt>
                            <dd class="col-sm-9" runat="server" id="NameFieldValue"><asp:Literal runat="server" ID="Name" /></dd>

                            <dt class="col-sm-3" runat="server" id="NotificationFieldHeader">Application Change Type:</dt>
                            <dd class="col-sm-9" runat="server" id="NotificationFieldValue"><asp:Literal runat="server" ID="NotificationName" /></dd>

                            <dt class="col-sm-3" runat="server" id="SurveyFieldHeader">Form:</dt>
                            <dd class="col-sm-9" runat="server" id="SurveyFieldValue"><asp:Literal runat="server" ID="SurveyName" /></dd>
                        </dl>

                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-12">

                        <div runat="server" id="ConfirmAlert" class="alert alert-danger">
                            <i class="fas fa-stop-circle"></i> <strong>Please Confirm:</strong>
                            Are you sure you want to delete this message?
                        </div>

                        <insite:DeleteButton runat="server" ID="DeleteButton" />
                        <insite:CancelButton runat="server" ID="CancelButton" />

                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-5">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Important Note</strong>:
                This is a permanent change that cannot be undone. 
                Your message will be removed from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed:
            </div>

            <table class="table table-striped table-bordered table-metrics">
                <asp:Repeater runat="server" ID="ReferenceRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Name") %></td>
                            <td><%# Eval("Value") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>

        </div>

    </div>

</asp:Content>
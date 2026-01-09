<%@ Page Language="C#" CodeBehind="Send.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Messages.Send" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="InSite.Admin.Messages.Messages.Forms" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        table.metadata th { font-size: 0.875em; font-weight: normal; text-align: right; color: #999; padding-top: 5px !important; }
        table.metadata tr th, table.metadata tr td { vertical-align: top; padding: 3px; }
        table.metadata tr td p,table.metadata tr td ul { margin-bottom: 0.5rem; }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="MessageTab" Title="Message" Icon="far fa-paper-plane" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Message
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="row">

                            <div class="col-lg-5 mb-3 mb-lg-0">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h3 class="card-title mb-3">
                                            Details
                                        </h3>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Message
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="MessageName" />
                                            </div>
                                        </div>

                                        <div runat="server" id="MessagePriorityField" class="form-group mb-3">
                                            <label class="form-label">
                                                Priority
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="MessagePriority" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                From
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="MessageSender" />
                                            </div>
                                        </div>

                                        <div runat="server" id="MessageRecipientField" class="form-group mb-3">
                                            <label class="form-label">
                                                To
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="MessageRecipient" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-7">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h3 class="card-title mb-3">
                                            Content
                                        </h3>

                                        <div>
                                            <asp:Literal runat="server" ID="MessageBody" />
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RecipientTab" Title="Recipients" Icon="far fa-users" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Recipients
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="MessageRecipients">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>Email</th>
                                            <th>Mail-Merge Fields</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# GetEmailAddress(Eval("Identifier")) %>
                                    </td>
                                    <td>
                                        <%# TriggerHelper.MetadataToHtml((Dictionary<string,string>)Eval("Variables")) %>
                                        <div class="form-text float-end">
                                            <%# GetEmailAddressList(Eval("Cc")) %>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="TestTab" Title="Test Drives" Icon="far fa-car" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Test Drives
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="form-text mb-3">
                            Select a date and click the <strong>Trigger</strong> button to trigger this notification for another date.
                            This is intended primarily for test drive scenarios.
                        </div>

                        <div class="d-inline-block align-middle me-3">
                            <insite:DateSelector runat="server" ID="ChangeTriggerDate" Width="300" />
                        </div>
                        
                        <cmds:CmdsButton runat="server" ID="ChangeTriggerButton" Text ="<i class='far fa-bolt'></i> Trigger" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
    
    <section runat="server" id="SubmitPanel" class="mt-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="form-text mb-3">
                    Select a <strong>delivery mode</strong> and click the <strong>Submit</strong> button.
                    Use <strong>deliver and log</strong> mode for normal delivery of this email message to each recipient.
                    If you select <strong>log but do not deliver</strong> then this email message is not delievered to any recipient but system logs are updated to make it appear as though it was. (This is intended for testing only.)
                    Select <strong>trigger only</strong> to trigger the notification without sending this email message or logging any deliveries.
                </div>
    
                <div class="form-group mb-3">
                    <asp:RadioButtonList runat="server" ID="SubmitMode">
                        <Items>
                            <asp:ListItem Value="Authentic" Text="Deliver and log" Selected="True" />
                            <asp:ListItem Value="Counterfeit" Text="Log but do not deliver" />
                            <asp:ListItem Value="Silent" Text="Trigger only (do not deliver or log)" />
                        </Items>
                    </asp:RadioButtonList>
                </div>

                <cmds:CmdsButton runat="server" ID="SubmitButton" Text="<i class='far fa-paper-plane me-1'></i> Submit"  CssClass="btn btn-success" />

            </div>
        </div>
    </section>

    <section class="mt-3">
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>

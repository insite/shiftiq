<%@ Page Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.Admin.Messages.Deliveries.Forms.View" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-mail-bulk me-1"></i>
                    Delivery
                </h4>

                <div class="row">
                    <div class="col-lg-6 mb-3 mb-lg-0">
                        <div class="card h-100">
                            <div class="card-body">

                                <h3>Delivery</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Recipient Address
                                    </label>
                                    <div runat="server" id="DeliveryRecipientAddressOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Recipient Name
                                    </label>
                                    <div runat="server" id="DeliveryRecipientNameOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Status
                                    </label>
                                    <div runat="server" id="DeliveryStatusOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Started
                                    </label>
                                    <div runat="server" id="DeliveryStartedOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Completed
                                    </label>
                                    <div runat="server" id="DeliveryCompletedOutput">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6">
                        <div class="card mb-3">
                            <div class="card-body">

                                <h3>Message</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Internal Name
                                    </label>
                                    <div runat="server" id="MessageNameOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        External Subject
                                    </label>
                                    <div runat="server" id="MessageSubjectOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Sender
                                    </label>
                                    <div runat="server" id="MessageSenderOutput">
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="card">
                            <div class="card-body">

                                <h3>Mailout</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Scheduled
                                    </label>
                                    <div runat="server" id="MailoutScheduledOutput">
                                    </div>
                                </div>

                                <div runat="server" id="MailoutCompletedField" class="form-group mb-3">
                                    <label class="form-label">
                                        Completed
                                    </label>
                                    <div runat="server" id="MailoutCompletedOutput">
                                    </div>
                                </div>

                                <div runat="server" id="MailoutCancelledField" class="form-group mb-3">
                                    <label class="form-label">
                                        Cancelled
                                    </label>
                                    <div runat="server" id="MailoutCancelledOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Identifier
                                    </label>
                                    <div runat="server" id="MailoutIdentifierOutput">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="card mt-3" style="display:none;">
                    <div class="card-body">
                        <h3>Content</h3>

                        <h6 runat="server" id="ContentSubjectOutput"></h6>

                        <div runat="server" id="ContentBody">
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:CancelButton runat="server" ID="CancelButton" Text="Close" />
    </div>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            iframe.content-frame {
                border: none;
                border-radius: 4px;
                margin: 0;
                width: 100%;
                height: 500px;
            }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var content = <%= Shift.Common.JsonHelper.SerializeJsObject(DeliveryContent) %>;
                if (!content)
                    return;

                var $container = $('div#<%= ContentBody.ClientID %>').empty();
                var $iframe = $('<iframe class="content-frame">').appendTo($container);

                var doc = $iframe.contents()[0];
                doc.open();
                doc.write(content);
                doc.close();

                var anchors = doc.getElementsByTagName('a');
                for (var i = 0; i < anchors.length; i++) {
                    anchors[i].onclick = function () { return false; };
                }

                $container.closest('div.card').show();
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
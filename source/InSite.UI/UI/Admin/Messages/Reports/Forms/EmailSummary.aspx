<%@ Page Language="C#" CodeBehind="EmailSummary.aspx.cs" Inherits="InSite.Admin.Messages.Reports.Forms.EmailSummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/EmailDeliveryGrid.ascx" TagName="DeliveryGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-mail-bulk me-1"></i>
                    Email
                </h4>

                <div class="row">
                    <div class="col-lg-6 mb-3 mb-lg-0">
                        <div class="card h-100">
                            <div class="card-body">

                                <h3>Mailout</h3>

                                <div runat="server" ID="MailoutCompletedField" class="form-group mb-3">
                                    <label class="form-label">
                                        Delivery Time
                                    </label>
                                    <div runat="server" id="DeliveryTimeOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Status Code
                                    </label>
                                    <div runat="server" id="MessageCodeOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Status Message
                                    </label>
                                    <div runat="server" id="MessageStatusOutput">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6">
                        <div class="card h-100">
                            <div class="card-body">

                                <h3>Message</h3>

                                <div runat="server" id="MessageNameField" class="form-group mb-3">
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

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Email Id
                                    </label>
                                    <div runat="server" id="MessageIdOutput">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="card mt-3" style="display:none;">
                    <div class="card-body">
                        <h3>Content</h3>

                        <div runat="server" id="EmailContentOutput">
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" ID="FailedDeliveriesSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-envelope me-1"></i>
                    Failed Deliveries
                    <span runat="server" id="FailedDeliveriesCount" class="badge rounded-pill bg-info ms-1"></span>
                </h4>

                <uc:DeliveryGrid runat="server" ID="FailedDeliveriesGrid" DeliveryStatus="Forbidden" />

            </div>
        </div>
    </section>

    <section runat="server" ID="SuccessfulDeliveriesSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-envelope-open me-1"></i>
                    Successful Deliveries
                    <span runat="server" id="SuccessfulDeliveriesCount" class="badge rounded-pill bg-info ms-1"></span>
                </h4>

                <uc:DeliveryGrid runat="server" ID="SuccessfulDeliveriesGrid" DeliveryStatus="OK" />

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
                var content = <%= Shift.Common.JsonHelper.SerializeJsObject(EmailContent) %>;
                if (!content)
                    return;

                var $container = $('div#<%= EmailContentOutput.ClientID %>').empty();
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
<%@ Page Language="C#" CodeBehind="MailoutSummary.aspx.cs" Inherits="InSite.Admin.Messages.Reports.Forms.MailoutSummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/MailoutDeliveryGrid.ascx" TagName="DeliveryGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-mail-bulk me-1"></i>
                    Mailout
                </h4>

                <div class="row">
                    <div class="col-lg-6 mb-3 mb-lg-0">
                        <div class="card h-100">
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

                            </div>
                        </div>
                    </div>
                </div>

                <div class="card mt-3">
                    <div class="card-body">
                        <h3>Content</h3>

                        <div runat="server" id="MailoutContentOutput" class="d-none"></div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="ScheduledDeliveriesSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-mail-bulk me-1"></i>
                    Scheduled Deliveries
                    <span runat="server" id="ScheduledDeliveriesCount" class="badge rounded-pill bg-info ms-1"></span>
                </h4>

                <uc:DeliveryGrid runat="server" ID="ScheduledDeliveriesGrid" DeliveryStatus="Scheduled" />

            </div>
        </div>
    </section>

    <section runat="server" id="StartedDeliveriesSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-mail-bulk me-1"></i>
                    Started Deliveries
                    <span runat="server" id="StartedDeliveriesCount" class="badge rounded-pill bg-info ms-1"></span>
                </h4>

                <uc:DeliveryGrid runat="server" ID="StartedDeliveriesGrid" DeliveryStatus="Started" />

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

                <uc:DeliveryGrid runat="server" ID="FailedDeliveriesGrid" DeliveryStatus="Failed" />

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

                <uc:DeliveryGrid runat="server" ID="SuccessfulDeliveriesGrid" DeliveryStatus="Succeeded" />

            </div>
        </div>
    </section>

    <div>
        <insite:CancelButton runat="server" ID="CancelButton" Text="Close" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            inSite.common.contentToIFrame(document.getElementById('<%= MailoutContentOutput.ClientID %>'), {
                disableAnchors: true
            });
        </script>
    </insite:PageFooterContent>

</asp:Content>
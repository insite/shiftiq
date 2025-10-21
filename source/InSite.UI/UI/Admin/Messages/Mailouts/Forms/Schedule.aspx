<%@ Page Language="C#" CodeBehind="Schedule.aspx.cs" Inherits="InSite.Admin.Messages.Mailouts.Forms.Schedule" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:UpdatePanel runat="server" ClientEvents-OnRequestStart="schedule.requestStart" ClientEvents-OnResponseEnd="schedule.requestEnd">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Schedule" />

            <div class="alert alert-warning" runat="server" ID="LargeMailoutNotice" role="alert">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This message has a very large recipient list (with over 1,000 recipients) and it might require more than an hour to complete delivery.
                The sending mail server intentionally limits the number of outgoing emails per minute to help avoid triggering the spam filters on receiving mail servers.
            </div>

            <div class="alert alert-danger" runat="server" ID="OversizeMailoutNotice" role="alert">
                <strong><i class="far fa-exclamation-circle"></i> Recipient List Exceeds Maximum Size</strong>
                <div class="mt-2">
                    The recipient list for this message exceeds the maximum allowable size of <asp:Literal runat="server" ID="MailoutDeliveryLimit" /> recipients.
                    Please divide your list into smaller groups and schedule the delivery in batches. 
                </div>
                <div class="mt-2">
                    We recommend batches of 5,000 (or less) to help avoid triggering the spam filters on 
                    recipients' mail servers.
                </div>
            </div>

            <section runat="server" id="MessageSection" class="mb-3">
                <div class="row">
                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <h4 class="card-title mb-3">
                                    <i class="far fa-paper-plane me-1"></i>
                                    Message
                                </h4>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        From
                                    </label>
                                    <div runat="server" id="MessageSenderOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Subject
                                    </label>
                                    <div runat="server" id="MessageSubjectOutput">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        To
                                    </label>
                                    <div runat="server" id="MessageRecipientsOutput">
                                    </div>
                                </div>

                                <div runat="server" id="ScheduleDateField" class="form-group mb-3">
                                    <label class="form-label">
                                        Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="ScheduleDate" FieldName="Date and Time" ValidationGroup="Schedule" />
                                    </label>
                                    <insite:DateTimeOffsetSelector ID="ScheduleDate" runat="server" />
                                    <div class="form-text">
                                        Scheduled messages are sent every five to ten minutes, so if you schedule this mailout now then you might need to wait a few minutes before delivery is complete.
                                        Messages can be scheduled for delivery a maximum of 3 days (72 hours) in the future.
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section runat="server" id="ProblemSection" class="mb-3">
                <div class="row">
                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <h4 class="card-title mb-3">
                                    <i class="far fa-exclamation-circle me-1"></i>
                                    Problem Information
                                </h4>

                                <div class="alert alert-danger" role="alert">
                                    <i class="fas fa-stop-circle"></i>
                                    <strong>This message can't be delivered yet.</strong>
                                    <div class="mt-2">Before sending this message, please address each of the issues listed below.</div>
                                </div>

                                <div runat="server" ID="SameNameProblem" visible="false" class="card mt-3">
                                    <div class="card-body">
                                        <h5>Duplicate Contacts</h5>
                                        <p>Your contact database contains <asp:Literal runat="server" ID="SameNameCount" /> contacts with the same first, middle, and last name:</p>
                                        <insite:Grid runat="server" ID="SameNameGrid">
                                            <Columns>
                                                <asp:BoundField HeaderText="Contact Name" DataField="PersonName" />
                                                <asp:BoundField HeaderText="Duplicates" DataField="SameNameCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" />
                                            </Columns>
                                        </insite:Grid>
                                    </div>
                                </div>

                                <div runat="server" ID="InvalidLinksProblem" visible="false" class="card mt-3">
                                    <div class="card-body">
                                        <h5>Invalid Links</h5>
                                        <asp:Repeater runat="server" ID="InvalidLinksRepeater">
                                            <ItemTemplate>
                                                <div><strong>Reason</strong>: <%# Eval("Reason") %></div>
                                                <div><strong>Link</strong>: <%# Eval("Url") %></div>
                                            </ItemTemplate>
                                            <SeparatorTemplate><hr /></SeparatorTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <div>
                <insite:Button runat="server" ID="ScheduleButton" ButtonStyle="Success" Visible="false"
                    CausesValidation="true" ValidationGroup="Schedule"
                    OnClientClick="return schedule.confirmSchedule();" 
                    Icon="fas fa-plus-circle" Text="Add to Schedule" />
                <insite:CloseButton runat="server" ID="CloseButton" />
            </div>

            <insite:LoadingPanel runat="server" />

        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = window.schedule = window.schedule || {};

            var $scheduleButton = $('#<%= ScheduleButton.ClientID %>');
            var $loadingPanel = $('.loading-panel');

            instance.requestStart = function () {
                $scheduleButton.remove();

                $loadingPanel.find('> div > div').append($('<span>Scheduling...</span>'));
                $loadingPanel.show();
            };

            instance.requestEnd = function () {
                $loadingPanel.find('> div > div > span').remove();
                $loadingPanel.hide();
            };

            <% if (IsScheduleActive) { %>
            $loadingPanel.find('> div > div').append($('<span>The message is currently being scheduled - please wait.</span>'));
            $loadingPanel.show();

            var timeoutId = null;

            checkStatus();

            function checkStatus() {
                if (timeoutId)
                    clearTimeout(timeoutId);

                timeoutId = setTimeout(requestStatus, 1000);
            }

            function requestStatus() {
                if ($.active !== 0) {
                    checkStatus();
                    return;
                }

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        IsPageAjax: true,
                        action: 'check-status',
                    },
                    success: function (data) {
                        if (data === 'RELOAD')
                            window.location.reload(true);
                    },
                    complete: function () {
                        checkStatus();
                    }
                });
            }

            <% } else { %>
            instance.confirmSchedule = function () {
                if (typeof Page_ClientValidate == 'function' && !Page_ClientValidate('Schedule'))
                    return false;

                var count = 0;
                if (typeof ___messageRecipientsCount === 'number')
                    count = ___messageRecipientsCount;

                var date = $('#<%= ScheduleDate.ClientID %>').val();
                var message = 'Are you sure you want to schedule the delivery of '
                    + String(count).replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' email message' + (count > 1 ? 's' : '')
                    + '?';

                var result = confirm(message);

                if (result)
                    $scheduleButton.hide();

                return result;
            };
            <% } %>
        })();

    </script>
    </insite:PageFooterContent>


</asp:Content>
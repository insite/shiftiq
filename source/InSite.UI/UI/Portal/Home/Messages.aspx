<%@ Page Language="C#" CodeBehind="Messages.aspx.cs" Inherits="InSite.UI.Portal.Home.Messages" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="row">
        <div class="col-lg-12">

            <asp:Repeater ID="MailItems" runat="server">
                <HeaderTemplate>
                    <div id='<%# MailItems.ClientID %>' class="accordion mb-5 position-relative">
                </HeaderTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
                <ItemTemplate>

                    <div class="accordion-item">
                        <h2 class="accordion-header" id='item-header-<%# Eval("Sequence") %>'>
                            <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target='#item-collapse-<%# Eval("Sequence") %>' aria-expanded="false" aria-controls='item-collapse-<%# Eval("Sequence") %>'>
                                <%# Eval("ContentSubject") %>

                                <div class="position-absolute end-0 pe-5">
                                    <%# GetDeliveryHtml() %>
                                </div>
                            </button>
                        </h2>
                        <div class="accordion-collapse collapse" id='item-collapse-<%# Eval("Sequence") %>' aria-labelledby='item-header-<%# Eval("Sequence") %>' data-bs-parent="#orders-accordion" style="">
                            <div class="accordion-body pt-4 bg-secondary rounded-top-0 rounded-3">
                                <div class="float-end pb-2">
                                    <insite:Button runat="server" ID="PrintDeliveredEmail" ButtonStyle="OutlineSecondary" Size="ExtraSmall" Icon="fas fa-download" Text="Print" CommandName="Print"  CommandArgument='<%# Eval("MailoutIdentifier") %>' />
                                </div>

                                <div class="mb-3">
                                    <strong>
                                        From: <%# Eval("SenderName") %> &lt;<%# Eval("SenderEmail") %>&gt;
                                    </strong>
                                </div>

                                <div class="mail-body-html mb-3 d-none">
                                    <%# HttpUtility.HtmlEncode((string)Eval("ContentBodyHtml")) %>
                                </div>

                                <div class="mail-body-text mb-3 d-none">
                                    <%# Eval("ContentBody") %>
                                </div>

                                <%# GetVariablesHtml() %>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                let $dummyFrameWrapper = $('<div class="position-absolute w-100 invisible" style="padding: var(--ar-accordion-body-padding-y) var(--ar-accordion-body-padding-x);">');
                let $dummyFrame = $('<iframe frameborder="0" class="w-100 border-0">')
                let $accordion = $('#<%= MailItems.ClientID %>').append($dummyFrameWrapper.append($dummyFrame));

                $('#<%= MailItems.ClientID %> .mail-body-html').each(function () {
                    const $this = $(this);

                    const value = $this.text().trim();
                    if (value.length > 0) {
                        const $frame = $('<iframe frameborder="0" class="w-100 border-0">')
                            .addClass($this.attr('class').replaceAll('d-none', ''))
                            .insertAfter($this);

                        setFrameContent($dummyFrame, value);
                        const height = calcFrameHeight($dummyFrame);

                        setFrameContent($frame, value);

                        $frame.height(height).closest('.accordion-item').on('shown.bs.collapse', onCollapse);

                        $this.parent().find('.mail-body-text').remove();
                    }

                    $this.remove();
                });

                $('#<%= MailItems.ClientID %> .mail-body-text').each(function () {
                    const $this = $(this);
                    if ($this.text().trim().length == 0)
                        $this.remove();
                    else
                        $this.removeClass('d-none');
                });

                $dummyFrameWrapper.remove();
                $dummyFrameWrapper = null;
                $dummyFrame = null;
                $accordion = null;

                function onCollapse() {
                    const $frame = $(this).find('iframe.mail-body-html');
                    const height = calcFrameHeight($frame);

                    $frame.height(height);
                }

                function setFrameContent($frame, value) {
                    const doc = $frame.contents()[0];
                    doc.open();
                    doc.write(value);
                    doc.close();
                }

                function calcFrameHeight($frame) {
                    $frame.height('');

                    const $body = getFrameBody($frame);
                    if ($body == null)
                        return;

                    $body.css('overflow', 'hidden');

                    var margin = $body.outerHeight(true) - $body.outerHeight();
                    if (isNaN(margin) || margin < 0)
                        margin = 0;

                    let height = $body.prop('scrollHeight') + margin + 16;
                    if (height < 150)
                        height = 150;

                    $body.css('overflow', '');

                    return height;
                }

                function getFrameBody($frame) {
                    try {
                        return $frame.contents().find('body');
                    } catch (ex) {

                    }

                    return null;
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
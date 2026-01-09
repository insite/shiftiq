<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportInvitationAnalysis.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportInvitationAnalysis" %>

<div class="row">
    <div class="col-md-12">
        <ul>
            <li runat="server" ID="Scheduled" Visible="false"></li>
            <li runat="server" ID="Delivered" Visible="false"></li>
            <li runat="server" ID="DeliveredMoreThanOnce" Visible="false"></li>
            <li runat="server" ID="Responded" Visible="false"></li>
        </ul>
    </div>
</div>

<asp:Button runat="server" ID="Download" style="display:none;" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        var invitationAnalytics = {
            downloadCsv: function(type) {
                if (typeof type !== 'string' && type.length == 0)
                    return;

                var $hf = $('<input type="hidden" name="__CSVTYPE">').val(type);

                $('form').append($hf);

                __doPostBack('<%= Download.UniqueID %>', '');

                $hf.remove();
            }
        };
    </script>
</insite:PageFooterContent>

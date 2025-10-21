<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnnouncementToast.ascx.cs" Inherits="InSite.Admin.Programs.Controls.AnnouncementToast" %>

<div class="position-fixed p-3" style="z-index: 11">
    <div id="announcementToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="false">
    <div class="toast-header bg-warning text-white">
    <i class="fas fa-bell-on me-2"></i>
    <span class="me-auto">Please Note!</span>
    <button type="button" class="btn-close btn-close-white ms-2 mb-1" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div runat="server" id="AnnouncementHtml" class="toast-body text-warning">

    </div>
    </div>
</div>

<script type="text/javascript">

    function showAnnouncementToast() {
        var toastLiveExample = document.getElementById('announcementToast')
        var toast = new bootstrap.Toast(toastLiveExample);
        toast.show();
    }

</script>
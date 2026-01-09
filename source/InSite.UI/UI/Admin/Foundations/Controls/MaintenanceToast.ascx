<%@ Control Language="C#" CodeBehind="MaintenanceToast.ascx.cs" Inherits="InSite.Admin.Programs.Controls.MaintenanceToast" %>

<div class="position-fixed p-3" style="z-index: 11">
    <div id="liveToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="false">
    <div class="toast-header bg-danger text-white">
    <i class="fas fa-bell-on me-2"></i>
    <span class="me-auto">Important Notice</span>
    <button type="button" class="btn-close btn-close-white ms-2 mb-1" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body text-danger">
        <asp:Literal runat="server" ID="Description" />
    </div>
    </div>
</div>

<script type="text/javascript">

    function showMaintenanceToast() {
        var toastLiveExample = document.getElementById('liveToast')
        var toast = new bootstrap.Toast(toastLiveExample);
        toast.show();
    }

</script>
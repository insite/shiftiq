<%@ Page Language="C#" CodeBehind="Print.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Print" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="PrintStatus" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-print"></i>
            Print Bank Form
        </h2>
        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Print Options</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Print Settings
                            </label>
                            <div>
                                <insite:CheckBox ID="IncludeImages" runat="server" Text="Include Images" Checked="true" />
                                <insite:CheckBox ID="IncludeAdminComments" runat="server" Text="Include Admin Comments" Checked="true" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:FormDetails ID="FormDetails" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="BuildForm" ButtonStyle="Success" Icon="fas fa-download" Text="Print Form" />
            <insite:Button runat="server" ID="BuildAddendum" ButtonStyle="Success" Icon="fas fa-download" Text="Print Addendum" />
            <insite:Button runat="server" ID="BuildFormInternal" ButtonStyle="Warning" Icon="fas fa-download" Text="Print Form (internal)" />
            <insite:Button runat="server" ID="BuildFormCompact" ButtonStyle="Info" Icon="fas fa-download" Text="Print Form (compact)" />
            <insite:CloseButton runat="server" ID="GoBackButton" />
        </div>
    </div>

    <insite:Container runat="server" ID="BuildingPdfPanel" Visible="false">
        <insite:LoadingPanel runat="server" ID="BuildingPdfLoadingPanel" Text="Building PDF" VisibleOnLoad="true" />
        <asp:Button runat="server" ID="DownloadButton" CssClass="d-none" />
        <script type="text/javascript">
            (function () {
                startTimer();

                function startTimer() {
                    setTimeout(onTimeout, 4000);
                }

                function onTimeout() {
                    $.ajax({
                        type: 'POST',
                        data: { isPageAjax: true },
                        error: function () {
                            alert('An error occurred while creating the document.');
                        },
                        success: function (data) {
                            if (data == 'DONE') {
                                var download = document.getElementById('<%= DownloadButton.ClientID %>');
                                if (download)
                                    download.click();

                                setTimeout(onDone, 500);
                            } else {
                                startTimer();
                            }
                        }
                    });
                }

                function onDone() {
                    var download = document.getElementById('<%= DownloadButton.ClientID %>');
                    if (download)
                        download.remove();

                    var panel = document.getElementById('<%= BuildingPdfLoadingPanel.ClientID %>');
                    if (panel)
                        panel.remove();
                }
            })();
        </script>
    </insite:Container>


    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                $('a[disabled]').on('click', function (e) {
                    e.preventDefault();
                });
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>

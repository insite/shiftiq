<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Reports.Dashboards.Home" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="./Controls/DashboardContainer.ascx" TagName="DashboardContainer" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        tr.grid-pager td table tbody tr td { padding-right: 10px; }
        tr.grid-pager td table tbody tr td a { border: 1px solid #e9e9f2; padding: 2px 5px; border-radius: 5px; }
        tr.grid-pager td table tbody tr td span { font-weight: bold; }
        tr.grid-pager td table tbody tr td span:before { content: " Page "; }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="BodyStatus" />

    <uc:DashboardContainer runat="server" ID="DashboardContainer" />

    <asp:Panel runat="server" ID="SpecificationPanel" CssClass="bg-secondary rounded-3 p-4">

        <div class="row">
            <div class="col-lg-4">
                <div class="card">
                    
                    <div class="card-body">
                        <h5 class="card-title">Dashboard Specification</h5>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Select and Upload File
                            </label>
                            <insite:FileUploadV1 runat="server" ID="UploadFile" 
                                AllowedExtensions=".zip" LabelText=""
                                FileUploadType="Unlimited"
                                OnClientFileUploaded="reportsDashboard.onFileUploaded"
                            />
                            <div class="mt-3">
                                <insite:UploadButton runat="server" ID="SaveButton" CssClass="disabled" />
                                <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete the selected dashboard?" />
                                <div class="float-end">
                                    <insite:DownloadButton runat="server" ID="DownloadButton" />
                                </div>
                            </div>
                        </div>
                    
                    </div>
                </div>
            </div>
        </div>
        
    </asp:Panel>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (window.reportsDashboard)
                    return;

                const instance = window.reportsDashboard = {};

                instance.onFileUploaded = function () {
                    document.getElementById('<%= SaveButton.ClientID %>').classList.remove('disabled');
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
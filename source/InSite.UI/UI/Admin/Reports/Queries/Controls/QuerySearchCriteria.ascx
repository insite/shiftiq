<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuerySearchCriteria.ascx.cs" Inherits="InSite.Admin.Reports.Queries.Controls.QuerySearchCriteria" %>

<div class="row">

    <insite:Alert runat="server" ID="UploadAlert" />

    <div class="col-6">

        <div runat="server" id="CriteriaPanel">
            <h4>Criteria</h4>
            <asp:RadioButtonList runat="server" ID="ReportList"></asp:RadioButtonList>
        </div>
        
        <div class="alert alert-light mt-4">

            <div runat="server" id="DeletePanel">
                <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download Selected Query" />
                <insite:DeleteButton runat="server" ID="DeleteButton" Text="Delete Selected Query" ConfirmText="Are you sure you want to delete the selected query?" />
            </div>

            <div runat="server" id="UploadQueryPanel" class="mt-3">
                <strong>Upload New Query</strong>
                <div class="w-75">
                    <insite:FileUploadV2 runat="server" ID="QueryFile" AllowMultiple="false" LabelText="" FileUploadType="Unlimited" />
                </div>
            </div>

        </div>

    </div>

</div>
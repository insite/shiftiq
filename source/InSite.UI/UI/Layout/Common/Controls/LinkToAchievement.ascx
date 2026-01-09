<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkToAchievement.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.LinkToAchievement" %>

<div class="container">
    <div class="row">
        <div class="col-lg-12">

            <div class="card card-hover card-tile mb-4">
                <div class="card-body">
                    <h5 class="float-end"><i class="fas fa-award me-3"></i></h5>
                    <h5 class="card-title" runat="server" id="BlockHeading"></h5>
                    <div class="card-text" runat="server" ID="BlockDescription"></div>
                    <div class="card-text text-body-secondary mt-3 mb-3"><asp:Literal runat="server" ID="BlockDateGranted" /></div>
                    <div class="card-text text-body-secondary mt-3 mb-3"><asp:Literal runat="server" ID="BlockDateExpired" /></div>

                    <div class="d-flex">
                        <div class="flex-grow-1">
                            <a runat="server" ID="BlockDownloadUrl" href="#" class="btn btn-sm btn-primary disabled"><i class="fas fa-download me-1"></i> <%= Translate("Download") %></a>
                        </div>
                        <div class="">
                            <span runat="server" id="PendingLabel" visible="false" class="text-primary fw-bold"><i class="far fa-hourglass-start me-1"></i> <%= Translate("Pending") %></span>
                            <span runat="server" id="GrantedLabel" visible="false" class="text-success fw-bold"><i class="far fa-check me-1"></i> <%= Translate("Valid") %></span>
                            <span runat="server" id="ExpiredLabel" visible="false" class="text-danger fw-bold"><i class="far fa-alarm-clock me-1"></i> <%= Translate("Expired") %></span>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkToForm.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.LinkToForm" %>

<div class="container">
    <div class="row">
        <div class="col-lg-12">

            <div class="card card-hover card-tile mb-4">
              <div class="card-body">
                  
                <h5 class="card-title" runat="server" id="BlockTitle"><insite:Literal runat="server" Text="Title" /></h5>
                <div class="card-text" runat="server" id="BlockDescription"><insite:Literal runat="server" Text="Description" /></div>
                <div class="card-text text-body-secondary mt-3 mb-3"><asp:Literal runat="server" ID="BlockTimeRequired" /></div>

                  <div class="d-flex">
                    <div class="flex-grow-1">
                        <a runat="server" id="BlockStartUrl" visible="false" href="#" class="btn btn-sm btn-primary"><i class="far fa-rocket-launch me-1"></i> <%= Translate("Start") %></a>
                    </div>
                    <div class="">
                        <span runat="server" id="StartedLabel" visible="false" class="text-info fw-bold"><i class="far fa-hourglass me-1"></i> <%= Translate("Started") %></span>
                        <span runat="server" id="CompletedLabel" visible="false" class="text-success fw-bold"><i class="far fa-check me-1"></i> <%= Translate("Completed") %></span>
                    </div>
                  </div>

              </div>
            </div>

        </div>
    </div>
</div>
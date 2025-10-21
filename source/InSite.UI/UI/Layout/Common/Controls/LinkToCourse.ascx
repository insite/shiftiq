<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkToCourse.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.LinkToCourse" %>

<div class="container">
    <div class="row">
        <div class="col-lg-12">

            <div class="card card-hover card-tile mb-4">
              <div class="card-body">
                <h5 class="card-title" runat="server" id="BlockTitle"><insite:Literal runat="server" Text="Title" /></h5>
                <div class="card-text" runat="server" ID="BlockDescription"><insite:Literal runat="server" Text="Description" /></div>
                <div class="card-text text-body-secondary mt-3 mb-3"><asp:Literal runat="server" ID="BlockTimeRequired" /></div>
                
                  <script type='text/javascript'>

                      function showLaunchFrame() {
                          $("#launch-frame-panel").show();
                          return true;
                      }

                  </script>

                  <div class="d-flex">
                    <div class="flex-grow-1">
                        <a runat="server" ID="BlockStartUrl" visible="false" href="#" class="btn btn-sm btn-primary"><i class="far fa-rocket me-1"></i> <%= Translate("Launch") %></a>
                    </div>
                    <div class="">
                        <span runat="server" id="LaunchedLabel" visible="false" class="text-success fw-bold"><i class="far fa-check me-1"></i> <%= Translate("Launched") %></span>
                        <span runat="server" id="CompletedLabel" visible="false" class="text-success fw-bold"><i class="far fa-check me-1"></i> <%= Translate("Completed") %></span>
                    </div>
                  </div>

                  <div class="mt-4" id="launch-frame-panel" style="display:none;">
                  <iframe name="launch-frame" height="500">

                  </iframe>
                  </div>

                  <asp:Repeater runat="server" ID="StatementRepeater">
                      <HeaderTemplate>
                          <ul>
                      </HeaderTemplate>
                      <ItemTemplate>
                          <li>
                              x
                          </li>
                      </ItemTemplate>
                      <FooterTemplate>
                          </ul>
                      </FooterTemplate>
                  </asp:Repeater>

              </div>
            </div>

        </div>
    </div>
</div>

    

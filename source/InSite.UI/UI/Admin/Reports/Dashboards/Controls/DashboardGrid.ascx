<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardGrid.ascx.cs" Inherits="InSite.UI.Admin.Reports.Dashboards.DashboardGrid" %>

<insite:UpdatePanel runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="MyDownload" />
        <asp:PostBackTrigger ControlID="MyDownloadRaw" />
    </Triggers>
    <ContentTemplate>
    
        <div class="d-flex flex-row bd-highlight mb-3">
          <div runat="server" id="MyHeading"></div>
          <div>
              <asp:LinkButton runat="server" ID="MyDownload" Visible="false"><i class="fas fa-file-excel me-1" title="Download Excel"></i></asp:LinkButton>
              <asp:LinkButton runat="server" ID="MyDownloadRaw" Visible="false"><i class="fas fa-table-list me-1" title="Download Raw Data"></i></asp:LinkButton>
              <asp:LinkButton runat="server" ID="MyClear"><i class="fas fa-eraser me-1" title="Clear Criteria"></i></asp:LinkButton>
              <span runat="server" id="MyCount" class="text-body-secondary me-1"></span>
          </div>
        </div>

        <asp:GridView runat="server" ID="MyGrid" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="grid-pager">

        </asp:GridView>

        <insite:Alert runat="server" ID="MyStatus" />

    </ContentTemplate>

</insite:UpdatePanel>